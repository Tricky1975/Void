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
// Version: 19.11.05
// EndLic

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrickyUnits;

using Microsoft.Xna.Framework;

using Void.Lex;
using Void.Parts;
using Void.Stages;

namespace Void.Parts {

    class Document {
        
        public class Line {

            public class Char {
                public char str;
                public override string ToString() => $"{str}";
                public Color Col;
                public int cl = 1;

                public Char(char s, byte R, byte G, byte B,int l=1) {
                    str = s;
                    Col = new Color(R, G, B);
                    cl = l;
                }
                public Char(char s,Color c,int l=1) {
                    str = s;
                    Col = c;
                    cl = l;
                }
            }


            public string Rawline = "";
            public Char[] Letters = null;
            public string[] Words = null;
            public string Word(int i) {
                if (Words != null) {
                    foreach (string w in Words) {
                        if (w != " " && w != "\t") {
                            if (i == 0) return w;
                            i--;
                        }
                    }
                }
                throw new Exception("Word call out of range!");
            }
            public int WordCount {
                get {
                    var c = 0;
                    if (Words != null) foreach (string w in Words) if (w != " " && w != "\t") c++;
                    return c;
                }
            }
            public string[] GetWords {
                get {
                    var ret = new List<string>();
                    if (Words!=null) foreach (string w in Words) if (w != " " && w != "\t") ret.Add(w);
                    return ret.ToArray();
                }
            }

            public override string ToString() => Rawline;
            /*
            public override bool Equals(object obj) {
                if (obj.GetType() == typeof(Line))
                    return obj.ToString() == Rawline;
                else if (obj.GetType() == typeof(string))
                    return (string)obj == Rawline;
                else
                    throw new Exception("Illegal Line comparing");
            }
            */


            /* Done differently... 
            public void Define(string str) {
                Rawline = str;
            }
            */

            public Line(string s) {
                Rawline=s;                
            }
        }

        public TMap<string, int> Outline = new TMap<string, int>();
        public readonly LexBase Lexer;
        public bool Modified = false;
        public int CleanCD;
        public readonly List<Line> Lines = new List<Line>();
        public int posx = 0, posy = 0;
        public int scrollx = 0, scrolly = 0;
        public int cols => (int)Math.Floor((decimal)Editor.TextW - 180 / 16);
        public int LineCount => Lines.Count;
        public int PosX {
            get => posx;
            set {
                posx = value;
                if (posx < 0) {
                    PosY--;
                    posx = Lines[posy].Rawline.Length;
                } else if (posx> Lines[posy].Rawline.Length) {
                    PosY++;
                    posx = 0;
                }
                if (posx < scrollx) scrollx = posx;
                while (posx - scrollx > cols) scrollx++;
            }
        }
        public int PosY {
            get => posy;
            set {
                posy = value;
                if (posy < 0) posy = 0;
                if (posy >= Lines.Count) posy = Lines.Count - 1;
                if (posy < scrolly) scrolly = posy;
                while (posy - scrolly > ((Editor.TextH - Editor.TextY) / 16)) scrolly++;
                if (posx > Lines[posy].Rawline.Length) posx = Lines[posy].Rawline.Length;
            }
        }

        public string this[int l] {
            get => Lines.ElementAt(l).ToString();
            set => Lines.ElementAt(l).Rawline = value;            
        }

        public override string ToString() {
            var ret = new StringBuilder(1);
            foreach (Line l in Lines) ret.Append($"{l}");
            return $"{ret}";
        }

        public void Save(string file) {
            QuickStream.SaveString(file, $"{this}");
            Modified = false;
            CleanCD = 20000;
            CleanCD = 20000;
        }
        public static Document Load(string file,LexBase L=null) {
            var s = QuickStream.LoadString(file);
            return new Document(s,L);
        }
        public Document(string txt,LexBase L=null) {
            if (L == null)
                Lexer = LexNothing.Me;
            else
                Lexer = L;
            txt = txt.Replace("\r\n", "\n");
            foreach(string l in txt.Split('\n')) {
                var nl = new Line(l);
                Lines.Add(nl);
            }
            Lexer.Outline(this);
        }

    }
}


