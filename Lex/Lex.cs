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
using System.Threading.Tasks;

using Void;
using Void.Parts;

using Microsoft.Xna.Framework;

using TrickyUnits;

namespace Void.Lex {
    abstract class LexBase {
        abstract public void Chop(Document.Line line);
        abstract public void Outline(Document doc);
        readonly static public TMap<string, LexBase> Reg = new TMap<string, LexBase>();
    }

    class LexPL : LexBase {

        Color c_KeyWord = new Color(255, 255, 0);
        Color c_Gen = new Color(255, 255, 255);
        Color c_Comment = new Color(100, 100, 100);
        Color c_String = new Color(255, 0, 255);
        Color c_Number = new Color(0, 180, 255);


        // PL = Programming language... For most programming languages this will work.
        public override void Chop(Document.Line line) {
            try {
                var Words = new List<string>();
                var curword = new StringBuilder();
                var instring = false;
                var incomment = false;
                void endword() { if (curword.Length > 0) Words.Add(curword.ToString()); curword.Clear(); }
                for (int i = 0; i < line.Rawline.Length; i++) {
                    if (instring) {
                        if (line.Rawline[i] == curword[0] && line.Rawline[i - 1] != escape) {
                            instring = false;
                            curword.Append(line.Rawline[i]);
                            endword();
                        } else curword.Append(line.Rawline[i]);
                    } else if (incomment) {
                        curword.Append(line.Rawline[i]);
                    } else if (i + SingleComment.Length < line.Rawline.Length && qstr.Mid(line.Rawline, i+1, SingleComment.Length) == SingleComment) {
                        incomment = true;
                        endword();
                        curword.Append(line.Rawline[i]);
                    } else {
                        switch (line.Rawline[i]) {
                            case ' ':
                            case '\t':
                                endword();
                                curword.Append(line.Rawline[i]);
                                break;
                            // Not the cleanest method, but it works :P
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                            case '_':
                            case 'A':
                            case 'B':
                            case 'C':
                            case 'D':
                            case 'E':
                            case 'F':
                            case 'G':
                            case 'H':
                            case 'I':
                            case 'J':
                            case 'K':
                            case 'L':
                            case 'M':
                            case 'N':
                            case 'O':
                            case 'P':
                            case 'Q':
                            case 'R':
                            case 'S':
                            case 'T':
                            case 'U':
                            case 'V':
                            case 'W':
                            case 'X':
                            case 'Y':
                            case 'Z':
                            case 'a':
                            case 'b':
                            case 'c':
                            case 'd':
                            case 'e':
                            case 'f':
                            case 'g':
                            case 'h':
                            case 'i':
                            case 'j':
                            case 'k':
                            case 'l':
                            case 'm':
                            case 'n':
                            case 'o':
                            case 'p':
                            case 'q':
                            case 'r':
                            case 's':
                            case 't':
                            case 'u':
                            case 'v':
                            case 'w':
                            case 'x':
                            case 'y':
                            case 'z':
                                curword.Append(line.Rawline[i]);
                                break;
                            case '"':
                                if (doubquotestring) {
                                    endword();
                                    curword.Append('"');
                                    instring = true;
                                } else {
                                    endword();
                                    Words.Add($"{line.Rawline[i]}");

                                }
                                break;
                            case '\'':
                                if (singquotestring) {
                                    endword();
                                    curword.Append('"');
                                    instring = true;
                                } else {
                                    endword();
                                    Words.Add($"{line.Rawline[i]}");
                                }
                                break;
                            default:
                                endword();
                                Words.Add($"{line.Rawline[i]}");
                                break;
                        }
                    }
                }
                endword();
                var Chars = new List<Document.Line.Char>();
                foreach (string w in Words) {
                    var c = c_Gen;
                    var l = 1;
                    if ((byte)w[0] >= 48 && (byte)w[0] <= 57)
                        c = c_Number;
                    else if (KeyWords.Contains(w.Trim()))
                        c = c_KeyWord;
                    else if (qstr.Prefixed(w, SingleComment))
                        c = c_Comment;
                    else if ((w[0] == '"' && doubquotestring) || (w[0] == '\'' && singquotestring))
                        c = c_String;
                    for (int i = 0; i < w.Length; i++) {
                        if (w[i] == '\t') l = 5 - (Chars.Count % 5);
                        Chars.Add(new Document.Line.Char(w[i], c));
                    }
                }
                line.Letters = Chars.ToArray(); // Needed for display
                line.Words = Words.ToArray(); //   Needed for outline
            } catch (Exception E) {
#if DEBUG
                Void.FatalError($"{E.Message}\n\n{E.StackTrace}");
#else
                Void.FatalError($"{E.Message}");
#endif
                Environment.Exit(999);
            }
        }

        public override void Outline(Document doc) {            
            throw new NotImplementedException();
        }

        protected string[] KeyWords;
        protected string SingleComment = "//";        
        protected bool singquotestring = true;
        protected bool doubquotestring = true;
        protected char escape = '\\';

    }

    class LexNothing : LexBase {

        static public readonly LexNothing Me = new LexNothing();

        public override void Outline(Document doc) {
        }

        public override void Chop(Document.Line line) {
            var N = new Document.Line.Char[line.Rawline.Length];
            for (int i = 0;  i < line.Rawline.Length; i++){
                if (line.Rawline[i] == '\t')
                    N[i] = new Document.Line.Char(line.Rawline[i], Color.Aquamarine, 5 - (i % 5));
                else
                    N[i] = new Document.Line.Char(line.Rawline[i], Color.Aquamarine);
            }
            line.Letters = N;
        }

    }
}

