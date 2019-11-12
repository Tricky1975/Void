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

#define OverdrevenDebug

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Void.Parts;
using TrickyUnits;

namespace Void.Lex {
    class LexNIL:LexPL {

        readonly string[] ScopeStarters = "for repeat while if with switch".Split(' ');
        readonly string[] QuickMetaStarters = "index newindex len gc tostring".Split(' ');

        public LexNIL() {
            KeyWords = "and break do else elseif end false for function if in local nil not or repeat return then true until while void number int table string bool boolean class CONSTRUCTOR NEW userdata switch case private global NIL static get set forever #use #localuse #globaluse readonly var self default module DESTRUCTOR delegate NIL new group link with extends abstract final quickmeta".Split(' ');
#if DEBUG
            foreach (string kw in KeyWords) Debug.WriteLine($"Keyword NIL: {kw}");
#endif
            Reg["nil"] = this;
        }

void Chat(string msg) {
#if OverdrevenDebug
            Debug.WriteLine($"NIL OUTLINE DEBUG> {msg}");
#endif
        }

        public override void Outline(Document doc) {
            Chat("Begun!");
            string[] scope = new string[1];
            int scopelevel = 0;
            int lcount = doc.LineCount;
            bool fail = false;
            void incscope(int i=1) {
                scopelevel += i;
                if (scopelevel<0) {
                    Chat("Scope underrun");
                    foreach (string s in scope) Chat($"\tOS:{s}");
                    fail = true;
                    return;
                }
                while (scopelevel >= scope.Length) {
                    var ns = new string[scope.Length * 2];
                    for (int j = 0; j < scope.Length; j++) ns[j] = scope[j];
                    scope = ns;
                }
            }
            void newscope(string s) {
                incscope();
                scope[scopelevel] = s;
            }
            var R = new TMap<string, int>();
            void newID(string n,int id) {
                if (R[n] > 0)
                    fail = true;
                else
                    R[n] = id;
            }
            string cscope() => scope[scopelevel];
            var nclass = "";
            for(int ln = 0;  ln < lcount && (!fail); ln++) {
            var broken = false;
                Chat($"Line #{ln+1}/{lcount}");
                if (doc.Lines[ln].Words == null) doc.Lexer.Chop(doc.Lines[ln]);
                var words = doc.Lines[ln].GetWords;
                //Chat($"Words:{words.Length} after removing whitespaces. With whitespaces that is {doc.Lines[ln].Words.Length}");
                for(int i = 0; (!broken)&& i < words.Length &&(!fail); i++) {
                    //Chat($"\tWord {i + 1}/{words.Length}");
                    var w = words[i].Trim();
                    switch (w) {
                        case "class":
                        case "group":
                        case "module":
                            Chat($"class/{w}");
                            if (scopelevel > 0 || i >= words.Length - 1)
                                fail = true;
                            else if (i > 0 && words[i - 1] == "quickmeta") {
                                newscope("quickmeta");
                                nclass = $"{words[i + 1]}::";
                                newID(words[i + 1],ln);
                                broken = true;
                            } else {
                                newscope(w);
                                nclass = $"{words[i + 1]}.";
                                newID(words[i + 1], ln);
                            }
                            break;
                        case "void":
                        case "int":
                        case "number":
                        case "bool":
                        case "boolean":
                        case "table":
                        case "function":
                        case "delegate":
                        case "string":
                        case "var":
                            if (!(scopelevel >= 2 || (scopelevel >= 1 && nclass == ""))) {
                                if (i >= words.Length - 1)
                                    fail = true;
                                else {
                                    if (words[i + 1] == "(") {
                                        Chat("Quick delegate found! No outline, but a scope is needed");
                                        newscope("delegate function");
                                        broken = true;
                                    } else {
                                        newID($"{nclass}{words[i + 1]}", ln);
                                        if (words.Length > i + 2 && words[i + 2] == "(") {
                                            Chat($"{w}-function scope started");
                                            newscope("function");
                                            broken = true;
                                        } else {
                                            Chat($"{w}-variable so no scope");
                                        }
                                    }
                                }
                            } else {
                                if (words.Length > i + 2 && words[i + 2] == "(") {
                                    Chat($"{w}- local function scope started, but outline rejected!");
                                    newscope("function");
                                    broken = true;
                                } else {
                                    Chat($"Variable/Function declaration rejected! Scope level({scopelevel}) too high or not in class({nclass})");
                                }
                            }
                            break;
                        case "until":
                        case "forever":
                            if (cscope() == "repeat")
                                incscope(-1);
                            else
                                fail = true;
                            break;
                        case "end":
                            Chat($"Ending {cscope()} in line #{ln+1}");
                            if (cscope() == "class" || cscope() == "module" || cscope() == "group" || cscope() == "quickmeta") nclass = "";
                            incscope(-1);
                            break;
                        default:
                            if (ScopeStarters.Contains(w))
                                newscope(w);
                            if (cscope() == "quickmeta") { // && QuickMetaStarters.Contains(w.ToLower().Trim()))
                                Chat($"QuickMeta Tag: {w} ({ln+1}:{i+1})");
                                newscope($"quickmeta.{w}");
                                newID($"{nclass}{w}",ln);
                            }
                            break;
                    }
                    if (fail) Chat($"Fail on line #{ln + 1}; word #{i + 1}; '{w}'");
                }
            }
            if (fail) {
                Chat("Failed!");
                doc.Outline.Clear();
                doc.Outline["OUTLINE FAILED!"] = 0;
            } else {
                Chat($"Success -- {R.Count} item(s)");
                doc.Outline = R;
            }
        }
    }
}

