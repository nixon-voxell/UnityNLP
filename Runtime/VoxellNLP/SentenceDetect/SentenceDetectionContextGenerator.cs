//Copyright (C) 2005 Richard J. Northedge
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

//This file is based on the SDContextGenerator.java source file found in the
//original java implementation of OpenNLP. That source file contains the following header:

// Copyright (C) 2002 Jason Baldridge and Gann Bierner
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

using System;
using System.Collections.Generic;

namespace Voxell.NLP.SentenceDetect
{
  /// <summary> 
  /// Generate event contexts for maxent decisions for sentence detection.
  /// </summary>
    public class SentenceDetectionContextGenerator : SharpEntropy.IContextGenerator<Util.Pair<System.Text.StringBuilder, int>>
  {    
    private System.Text.StringBuilder mBuffer = new System.Text.StringBuilder();
        private List<string> mCollectFeatures = new List<string>();
    private Util.Set<string> mInducedAbbreviations;
    private char[] mEndOfSentenceCharacters;
    
    /// <summary>
    /// Creates a new <code>SentenceDetectionContextGenerator</code> instance with
    /// no induced abbreviations.
    /// </summary>
    public SentenceDetectionContextGenerator(char[] endOfSentenceCharacters) : this(new Util.Set<string>(), endOfSentenceCharacters)
    {
    }
    
    /// <summary> 
    /// Creates a new <code>SentenceDetectionContextGenerator</code> instance which uses
    /// the set of induced abbreviations.
    /// </summary>
    /// <param name="inducedAbbreviations">
    /// a <code>Set</code> of strings
    /// representing induced abbreviations in the training data.
    /// Example: Mr.
    /// </param>
    /// <param name="endOfSentenceCharacters">
    /// Character array of end of sentence characters.
    /// </param>
    public SentenceDetectionContextGenerator(Util.Set<string> inducedAbbreviations, char[] endOfSentenceCharacters)
    {
      mInducedAbbreviations = inducedAbbreviations;
      mEndOfSentenceCharacters = endOfSentenceCharacters;
    }
    
    /// <summary>
    /// Builds up the list of features, anchored around a position within the
    /// StringBuilder. 
    /// </summary>
        public virtual string[] GetContext(Util.Pair<System.Text.StringBuilder, int> pair)
    {
      string prefix;        //string preceeding the eos character in the eos token. 
      string previousToken;  //space delimited token preceding token containing eos character. 
      string suffix;        //string following the eos character in the eos token. 
      string nextToken;        //space delimited token following token containsing eos character. 

            System.Text.StringBuilder buffer = pair.FirstValue;
      int position = pair.SecondValue; //character offset of eos character in 

            //if (first is string[])
            //{
            //    string[] firstList = (string[])first;
            //    previousToken = firstList[0];
            //    string current = firstList[1];
            //    prefix = current.Substring(0, (position) - (0));
            //    suffix = current.Substring(position + 1);
            //    if (suffix.StartsWith(" "))
            //    {
            //        mCollectFeatures.Add("sn");
            //    }
            //    if (prefix.EndsWith(" "))
            //    {
            //        mCollectFeatures.Add("pn");
            //    }
            //    mCollectFeatures.Add("eos=" + current[position]);
            //    nextToken = firstList[2];
            //}
            //else
            //{
            //    //compute previous, next, prefix and suffix strings and space previous, space next features and eos features. 
            //    System.Text.StringBuilder buffer = (System.Text.StringBuilder)((Util.Pair)input).FirstValue;
      int lastIndex = buffer.Length - 1;
    
      // compute space previousToken and space next features.
      if (position > 0 && buffer[position - 1] == ' ')
      {
        mCollectFeatures.Add("sp");
      }
      if (position < lastIndex && buffer[position + 1] == ' ')
      {
        mCollectFeatures.Add("sn");
      }
      mCollectFeatures.Add("eos=" + buffer[position]);
    
      int prefixStart = PreviousSpaceIndex(buffer, position);
      
      int currentPosition = position;
    
      //assign prefix, stop if you run into a period though otherwise stop at space
      while (--currentPosition > prefixStart)
      {
        for (int currentEndOfSentenceCharacter = 0, endOfSentenceCharactersLength = mEndOfSentenceCharacters.Length; currentEndOfSentenceCharacter < endOfSentenceCharactersLength; currentEndOfSentenceCharacter++)
        {
          if (buffer[currentPosition] == mEndOfSentenceCharacters[currentEndOfSentenceCharacter])
          {
            prefixStart = currentPosition;
            currentPosition++; // this gets us out of while loop.
            break;
          }
        }
      }
      
      prefix = buffer.ToString(prefixStart, position - prefixStart).Trim();
  
      int previousStart = PreviousSpaceIndex(buffer, prefixStart);
      previousToken = buffer.ToString(previousStart, prefixStart - previousStart).Trim();
      
      int suffixEnd = NextSpaceIndex(buffer, position, lastIndex);
    
      currentPosition = position;
      while (++currentPosition < suffixEnd)
      {
        for (int currentEndOfSentenceCharacter = 0, endOfSentenceCharactersLength = mEndOfSentenceCharacters.Length; currentEndOfSentenceCharacter < endOfSentenceCharactersLength; currentEndOfSentenceCharacter++)
        {
          if (buffer[currentPosition] == mEndOfSentenceCharacters[currentEndOfSentenceCharacter])
          {
            suffixEnd = currentPosition;
            currentPosition--; // this gets us out of while loop.
            break;
          }
        }
      }
    
      int nextEnd = NextSpaceIndex(buffer, suffixEnd + 1, lastIndex + 1);
      if (position == lastIndex)
      {
        suffix = "";
        nextToken = "";
      }
      else
      {
        suffix = buffer.ToString(position + 1, suffixEnd - (position + 1)).Trim();
        nextToken = buffer.ToString(suffixEnd + 1, nextEnd - (suffixEnd + 1)).Trim();
      }
      
      mBuffer.Append("x=");
      mBuffer.Append(prefix);
      mCollectFeatures.Add(mBuffer.ToString());
      mBuffer.Length = 0;
      if (prefix.Length > 0)
      {
        mCollectFeatures.Add(System.Convert.ToString(prefix.Length, System.Globalization.CultureInfo.InvariantCulture));
        if (IsFirstUpper(prefix))
        {
          mCollectFeatures.Add("xcap");
        }
        if (mInducedAbbreviations.Contains(prefix))
        {
          mCollectFeatures.Add("xabbrev");
        }
      }
      
      mBuffer.Append("v=");
      mBuffer.Append(previousToken);
      mCollectFeatures.Add(mBuffer.ToString());
      mBuffer.Length = 0;
      if (previousToken.Length > 0)
      {
        if (IsFirstUpper(previousToken))
        {
          mCollectFeatures.Add("vcap");
        }
        if (mInducedAbbreviations.Contains(previousToken))
        {
          mCollectFeatures.Add("vabbrev");
        }
      }
      
      mBuffer.Append("s=");
      mBuffer.Append(suffix);
      mCollectFeatures.Add(mBuffer.ToString());
      mBuffer.Length = 0;
      if (suffix.Length > 0)
      {
        if (IsFirstUpper(suffix))
        {
          mCollectFeatures.Add("scap");
        }
        if (mInducedAbbreviations.Contains(suffix))
        {
          mCollectFeatures.Add("sabbrev");
        }
      }
      
      mBuffer.Append("n=");
      mBuffer.Append(nextToken);
      mCollectFeatures.Add(mBuffer.ToString());
      mBuffer.Length = 0;
      if (nextToken.Length > 0)
      {
        if (IsFirstUpper(nextToken))
        {
          mCollectFeatures.Add("ncap");
        }
        if (mInducedAbbreviations.Contains(nextToken))
        {
          mCollectFeatures.Add("nabbrev");
        }
      }
      
      string[] context = mCollectFeatures.ToArray();
      mCollectFeatures.Clear();
      return context;
    }
    
    private static bool IsFirstUpper(string input)
    {
      return char.IsUpper(input[0]);
    }
    
    /// <summary> 
    /// Finds the index of the nearest space before a specified index.
    /// </summary>
    /// <param name="buffer">
    /// The string buffer which contains the text being examined.
    /// </param>
    /// <param name="seek">
    /// The index to begin searching from.
    /// </param>
    /// <returns>
    /// The index which contains the nearest space.
    /// </returns>
    private static int PreviousSpaceIndex(System.Text.StringBuilder buffer, int seek)
    {
      seek--;
      while (seek > 0)
      {
        if (buffer[seek] == ' ')
        {
          while (seek > 0 && buffer[seek - 1] == ' ')
            seek--;
          return seek;
        }
        seek--;
      }
      return 0;
    }
    
    /// <summary>
    ///  Finds the index of the nearest space after a specified index.
    /// </summary>
    /// <param name="buffer">
    /// The string buffer which contains the text being examined.
    /// </param>
    /// <param name="seek">
    /// The index to begin searching from.
    /// </param>
    /// <param name="lastIndex">
    /// The highest index of the StringBuffer sb.
    /// </param>
    /// <returns>
    /// The index which contains the nearest space.
    /// </returns>
    private static int NextSpaceIndex(System.Text.StringBuilder buffer, int seek, int lastIndex)
    {
      seek++;
      char currentChar;
      while (seek < lastIndex)
      {
        currentChar = buffer[seek];
        if (currentChar == ' ' || currentChar == '\n')
        {
          while (buffer.Length > seek + 1 && buffer[seek + 1] == ' ')
            seek++;
          return seek;
        }
        seek++;
      }
      return lastIndex;
    }
  }
}
