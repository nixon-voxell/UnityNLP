//Copyright (C) 2006 Richard J. Northedge
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

//This file is based on the TreebankLinker.java source file found in the
//original java implementation of OpenNLP. That source file contains the following header:

//Copyright (C) 2003 Thomas Morton
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this program; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections.Generic;
using Voxell.NLP.Coreference;
using Voxell.NLP.Coreference.Mention;
using Voxell.NLP.Parser;

using System.Text;

namespace Voxell.NLP.Lang.English
{
  /// <summary>
    /// This class perform coreference for treebank style parses.  
  /// It will only perform coreference over constituents defined in the trees and
  /// will not generate new constituents for pre-nominal entities or sub-entities in 
  /// simple coordinated noun phrases.  This linker requires that named-entity information also be provided.  
  /// </summary>
  public class TreebankLinker : DefaultLinker
  {
        public TreebankLinker(string project) : base(project, LinkerMode.Test)
        {
        }

    public TreebankLinker(string project, LinkerMode mode) : base(project, mode)
    {
    }
    
    public TreebankLinker(string project, LinkerMode mode, bool useDiscourseModel) : base(project, mode, useDiscourseModel)
    {
    }
    
    public TreebankLinker(string project, LinkerMode mode, bool useDiscourseModel, double fixedNonReferentialProbability) : base(project, mode, useDiscourseModel, fixedNonReferentialProbability)
    {
    }
    
    protected internal override void InitializeMentionFinder()
    {
            MentionFinder = PennTreebankMentionFinder.GetInstance(HeadFinder);
    }
    
    private static string ShowEntities(DiscourseEntity[] entities)
    {
            StringBuilder output = new StringBuilder();
      for (int currentEntity = 0; currentEntity < entities.Length; currentEntity++)
      {
        output.Append(currentEntity + " " + entities[currentEntity]).Append("\r\n");
      }
            return output.ToString();
    }

        /// <summary>
        /// Identitifies coreference relationships for parsed input.
        /// </summary>
        /// <param name="parsedSentences">Array of parsed sentences.</param>
        /// <returns></returns>
        public string GetCoreferenceParse(string[] parsedSentences)
        {
            List<Parse> parses = new List<Parse>(parsedSentences.Length);
            foreach (string line in parsedSentences)
            {
                if (line == string.Empty)
                {
                    parses.Add(null);
                }
                else
                {
                    parses.Add(Parse.FromParseString(line));
                }
            }
            return GetCoreferenceParse(parses.ToArray());
        }

    /// <summary>
        /// Identitifies coreference relationships for parsed input.
    /// </summary>
    /// <param name="parsedSentences">Array of parsed sentences.</param>
    /// <returns></returns>
        public string GetCoreferenceParse(Parse[] parsedSentences)
    {
      int sentenceNumber = 0;
            List<Mention> document = new List<Mention>();
            List<Parse> parses = new List<Parse>();
            StringBuilder output = new StringBuilder();

            foreach (Parse lineParse in parsedSentences)
      {
        if (lineParse == null)
        {
          DiscourseEntity[] entities = GetEntitiesFromMentions(document.ToArray());
          output.Append(new CoreferenceParse(parses, entities).Show());
          sentenceNumber = 0;
          document.Clear();
          parses.Clear();
        }
        else
        {
          parses.Add(lineParse);
          Mention[] extents = MentionFinder.GetMentions(new DefaultParse(lineParse, sentenceNumber));
          
                    //construct new parses for mentions which don't have constituents.
          for (int currentExtent = 0; currentExtent < extents.Length; currentExtent++)
          {
            if (extents[currentExtent].Parse == null)
            {
              Parse snp = new Parse(lineParse.Text, extents[currentExtent].Span, "NML", 1.0);
              lineParse.Insert(snp);
              extents[currentExtent].Parse = new DefaultParse(snp, sentenceNumber);
            }
          }
                    document.AddRange(extents);
                    sentenceNumber++;
        }
      }
      if (document.Count > 0)
      {
        DiscourseEntity[] entities = GetEntitiesFromMentions(document.ToArray());
        //showEntities(entities);
        output.Append((new CoreferenceParse(parses, entities)).Show());
      }
            return output.ToString();
    }
  }
  
  internal class CoreferenceParse
  {
    private Dictionary<Parse, int> mParseMap;
        private List<Parse> mParses;
    
    public CoreferenceParse(List<Parse> parses, DiscourseEntity[] entities)
    {
      mParses = parses;
            mParseMap = new Dictionary<Parse, int>();
      for (int currentEntity = 0; currentEntity < entities.Length; currentEntity++)
      {
        if (entities[currentEntity].MentionCount > 1)
        {
                    foreach (MentionContext mc in entities[currentEntity].Mentions)
                    {
                        Parse mentionParse = ((DefaultParse)mc.Parse).Parse;
                        mParseMap[mentionParse] = currentEntity + 1;
                        //System.err.println("CorefParse: "+mc.getParse().hashCode()+" -> "+ (ei+1));
                    }
        }
      }
    }
    
    public virtual string Show()
    {
            StringBuilder buffer = new StringBuilder();

      for (int currentParse = 0; currentParse < mParses.Count; currentParse++)
      {
        Show(mParses[currentParse], buffer);
        buffer.Append("\r\n");
      }
            return buffer.ToString();
    }
    
    private void Show(Parse p, StringBuilder buffer)
    {
      int start = p.Span.Start;
      if (p.Type != MaximumEntropyParser.TokenNode)
      {
        buffer.Append("(");
                buffer.Append(p.Type);
        if (mParseMap.ContainsKey(p))
        {
          buffer.Append("#" + mParseMap[p].ToString());
        }
        buffer.Append(" ");
      }
      Parse[] children = p.GetChildren();
      for (int currentParse = 0; currentParse < children.Length; currentParse++)
      {
        Parse c = children[currentParse];
        Util.Span s = c.Span;
        if (start < s.Start)
        {
                    buffer.Append(p.Text.Substring(start, (s.Start) - (start)));
        }
        Show(c, buffer);
        start = s.End;
      }
            buffer.Append(p.Text.Substring(start, p.Span.End - start));
            if (p.Type != MaximumEntropyParser.TokenNode)
      {
                buffer.Append(")");
      }
    }
  }
}