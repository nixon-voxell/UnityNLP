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

//This file is based on the NumberEnum.java source file found in the
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
namespace Voxell.NLP.Coreference.Similarity
{
  
  /// <summary> Enumeration of number types. </summary>
  public class NumberEnum
  {
    private string mName;

        private NumberEnum(string name)
        {
            mName = name;
        }

        public override string ToString()
        {
            return mName;
        }

    /// <summary>
        /// Singular number type. 
        /// </summary>
    public static readonly NumberEnum Singular = new NumberEnum("singular");
    /// <summary>
        /// Plural number type.
        /// </summary>
    public static readonly NumberEnum Plural = new NumberEnum("plural");
    /// <summary>
        /// Unknown number type. 
        /// </summary>
    public static readonly NumberEnum Unknown = new NumberEnum("unknown");
  }
}