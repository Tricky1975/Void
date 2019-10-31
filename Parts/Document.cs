// Lic:
// Void
// ...
// 
// 
// 
// (c) Jeroen P. Broks, 
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 19.10.31
// EndLic

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrickyUnits;

namespace Void.Parts {

    class Document {
        
        public class Line {
            string Rawline = "";
            string[] Words = null;
            public override string ToString() => Rawline;
            public override bool Equals(object obj) {
                if (obj.GetType() == typeof(Line))
                    return obj.ToString() == Rawline;
                else if (obj.GetType() == typeof(string))
                    return (string)obj == Rawline;
                else
                    throw new Exception("Illegal Line comparing");
            }
            public void Define(string str) {
                Rawline = str;
                // TODO: Chop up!
            }
        }

        List<Line> Lines = new List<Line>();
        public string this[int l] {
            get => Lines.ElementAt(l).ToString();
            set => Lines.ElementAt(l).Define(value);            
        }

        public override string ToString() {
            var ret = new StringBuilder(1);
            foreach (Line l in Lines) ret.Append($"{l}");
            return $"{ret}";
        }

        public void Save(string file) => QuickStream.SaveString(file, $"{this}");

    }
}

