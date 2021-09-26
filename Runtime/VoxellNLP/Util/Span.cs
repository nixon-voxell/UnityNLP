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

//This file is based on the Span.java source file found in the
//original java implementation of OpenNLP. That source file contains the following header:

// Copyright (C) 2002 Tom Morton
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

namespace Voxell.NLP.Util
{
  /// <summary>
  /// Class for storing start and end integer offsets.  
  /// </summary>
  
  public class Span : System.IComparable
  {
    private int mStart;
    private int mEnd;
    
    /// <summary>
    /// Return the start of a span.
    /// </summary>
    /// <returns> 
    /// the start of a span.
    /// </returns>
    virtual public int Start
    {
      get
      {
        return mStart;
      }
      
    }
    /// <summary>
    /// Return the end of a span.
    /// </summary>
    /// <returns> 
    /// the end of a span.
    /// </returns>
    virtual public int End
    {
      get
      {
        return mEnd;
      }
      
    }
    
    
    /// <summary>Constructs a new Span object.
    /// </summary>
    /// <param name="startOfSpan">
    /// start of span.
    /// </param>
    /// <param name="endOfSpan">
    /// end of span.
    /// </param>
    public Span(int startOfSpan, int endOfSpan)
    {
      mStart = startOfSpan;
      mEnd = endOfSpan;
    }
    
    public virtual int Length()
    {
      return (mEnd - mStart);
    }
    
    /// <summary>
    /// Returns true is the specified span is contained by this span.  
    /// Identical spans are considered to contain each other. 
    /// </summary>
    /// <param name="span">
    /// The span to compare with this span.
    /// </param>
    /// <returns>
    /// true if the specified span is contained by this span; false otherwise. 
    /// </returns>
    public virtual bool Contains(Span span)
    {
      return (mStart <= span.Start && span.End <= mEnd);
    }
    
    /// <summary>
    /// Returns true if the specified span intersects with this span.
    /// </summary>
    /// <param name="span">
    /// The span to compare with this span. 
    /// </param>
    /// <returns>
    /// true is the spans overlap; false otherwise. 
    /// </returns>
    public bool Intersects(Span span) 
    {
      int spanStart = span.Start;
      //either span's start is in this or this's start is in span
      return (this.Contains(span) || span.Contains(this) ||
        (mStart <= spanStart && spanStart < mEnd ||
        spanStart <= mStart && mStart < span.End));
    }
  
    /// <summary>
    /// Returns true if the specified span crosses this span.
    /// </summary>
    /// <param name="span">
    /// The span to compare with this span.
    /// </param>
    /// <returns>
    /// true if the specified span overlaps this span and contains a non-overlapping section; false otherwise.
    /// </returns>
    public bool Crosses(Span span) 
    {
      int spanStart = span.Start;
      //either span's Start is in this or this's Start is in span
      return (!this.Contains(span) && !span.Contains(this) && 
        (mStart <= spanStart && spanStart < mEnd ||
        spanStart <= mStart && mStart < span.End));
    }

    public virtual int CompareTo(object o) 
    { 
      Span compareSpan = (Span) o;
      if (Start < compareSpan.Start) 
      {
        return -1;
      }
      else if (Start == compareSpan.Start) 
      {
        if (End > compareSpan.End) 
        {
          return -1;
        }
        else if (End < compareSpan.End) 
        {
          return 1;
        }
        else 
        {
          return 0;
        }
      }
      else 
      {
        return 1;
      }
    }

    public override int GetHashCode() 
    {
      return((Start << 16) | (0x0000FFFF | this.End));
    }
  
    public override bool Equals(object o) 
    {
      if (!(o is Span))
      {
        return false;
      }
      Span currentSpan = (Span) o;
      return(Start == currentSpan.Start && End == currentSpan.End);
    }

    public override string ToString()
    {
      System.Text.StringBuilder buffer = new System.Text.StringBuilder(15);
      return (buffer.Append(Start).Append("..").Append(End).ToString());
    }
  }
}
