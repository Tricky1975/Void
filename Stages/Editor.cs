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
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using TrickyUnits;

using Void.Parts;

namespace Void.Stages {
    class Editor : Stage {

        int lchr = 0;

        static public int TextX => 0;
        static public int TextY => 20;

        int ProjectScroll = 0;
        int FileScroll = 0;
        int OutLineScroll = 0;

        bool Insert = true;

        static public int TextW;
        static public int TextH = TQMG.ScrHeight - 40;

        Document Doc {
            get {
                if (Project.ChosenProject == null || Project.ChosenProject.CurrentItem == null || Project.ChosenProject.CurrentItem.Doc == null) 
                    return null;
                return Project.ChosenProject.CurrentItem.Doc;
            }
        }

        #region Init
        public Editor() {
            Register("Editor");
            TextW = (int)Math.Floor(TQMG.ScrWidth * .75);
        }
        #endregion

        #region Callbacks
        public override void Draw() {

            if (lchr < 256) {
                Void.Font.DrawText($"{(char)lchr}!",0,0);
                lchr++;
            }

            // Positions
            var StatY = TQMG.ScrHeight - 20;
            var OutX = TextX + TextW;
            var OutW = TQMG.ScrWidth - (TextX + TextW);

            // Document Content
            if (Doc != null)
                TQMG.Color(127, 127, 127);
            Void.VoidBack.Draw((TextX + (TextW / 2)) - (Void.VoidBack.Width / 2), (TextY + (TextH / 2)) - (Void.VoidBack.Height / 2));
            if (Doc != null) {
                for (int lnnr = 0; lnnr < Doc.Lines.Count; lnnr++) {
                    var py = lnnr - Doc.scrolly;
                    var ty = TextY + (py * 16);
                    if (ty >= TextY && ty < StatY) {
                        TQMG.Color(127, 127, 127);
                        Void.Font.DrawText($"{lnnr + 1} {(char)186}", TextX + (144), ty, TQMG_TextAlign.Right);
                    }
                    var tx = 172;
                    if (Doc.Lines[lnnr].Letters == null) Doc.Lexer.Chop(Doc.Lines[lnnr]);
                    if (Doc.Lines[lnnr].Letters != null) {
                        for (int psnr = 0; psnr < Doc.Lines[lnnr].Letters.Count(); psnr++) {
                            if (psnr >= Doc.scrollx && tx < TextW) {
                                var let = Doc.Lines[lnnr].Letters[psnr];
                                TQMG.Color(let.Col);
                                if (psnr == Doc.posx && lnnr == Doc.posy) {
                                    TQMG.DrawRectangle(tx, ty, let.cl * 8, 16);
                                    TQMG.Color((byte)(255 - let.Col.R), (byte)(255 - let.Col.G), (byte)(255 - let.Col.B));
                                }
                                if (let.str != ' ' && let.str != '\t') Void.Font.fimg.Draw(tx, ty, (byte)let.str);
                                tx += let.cl * 8;
                            }
                        }
                        if (Doc.PosY==lnnr && Doc.PosX == Doc.Lines[lnnr].Letters.Count()) {
                            TQMG.Color(Color.Aquamarine);
                            TQMG.DrawRectangle(tx, ty,  8, 16);
                        }
                    }
                }
            }


            // Project, FileList and Outline
            TQMG.Color(255, 255, 255);
            Void.Back.Draw(OutX, TextY, TextX + TextW, TextY, OutW, TextW);
            // Project list
            TQMG.Color(255, 0, 0);
            TQMG.SetAlpha(50);
            TQMG.DrawRectangle(OutX, TextY, OutW, 64);
            TQMG.Color(255, 255, 0);
            TQMG.SetAlpha(255);
            {
                var y = 0 - ProjectScroll;
                foreach(string n in Project.ProjMap.Keys) {
                    if (y>=0 && y < 4) {
                        var n2 = Project.ProjMap[n].CFG["Title"]; if (n2.Length > 40) n2 = $"{qstr.Left(n2, 16)}...{qstr.Right(n2, 16)}";
                        var iy = TextY + (y * 16);
                        var Txt = Void.Font.Text(n2);
                        TQMG.Color(255, 255, 0);
                        if (n == Project.ChosenProjectID) {
                            TQMG.DrawRectangle(OutX, iy, Txt.Width, 16);
                            TQMG.Color(255, 0, 0);
                        }
                        Txt.Draw(OutX, iy);
                        if (Void.ms.X > OutX && Void.ms.Y > iy && Void.ms.Y < iy + 16 && Void.ms.LeftButton==Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
                            Project.ChosenProjectID = n;
                        }
                        y++;
                    }
                }
            }

            // File List
            TQMG.Color(0, 255, 0);
            TQMG.SetAlpha(50);
            TQMG.DrawRectangle(OutX, TextY + 64, OutW, 128);
            TQMG.Color(180, 255, 0);
            TQMG.SetAlpha(255);
            {
                var y = 0 - FileScroll;
                void PS(int tab = 0, Project.Item Item = null) {
                    TMap<string, Project.Item> IL;
                    if (Item == null) IL = Project.ChosenProject.ItemMap; else IL = Item.SubDirectory;
                    foreach (string key in IL.Keys) {
                        var V = IL[key];
                        var iy = TextY + 64 + (y * 16);
                        switch (V.Type) {
                            case Project.ItemType.NonExistent:  
                                throw new Exception($"File '{key}' appears to be marked as Non-Existent!");
                            case Project.ItemType.File:
                                TQMG.Color(180, 255, 0);
                                if (y >= 0 && y < 8) {
                                    if (V == Project.ChosenProject.CurrentItem) {
                                        TQMG.DrawRectangle(OutX, iy, TQMG.ScrWidth - OutX - 10, 16);
                                        TQMG.Color(0, 25, 0);
                                    }
                                    Void.Font.DrawText($"F> {qstr.Str(" ", tab)}{key}", OutX, iy);
                                    if (Void.ms.Y > iy && Void.ms.Y < iy + 16 && Void.ms.X > OutX && Void.ms.X < TQMG.ScrWidth - 10 && Void.ms.LeftButton==Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
                                        Project.ChosenProject.CurrentItem = V;
                                    }
                                }
                                y++;
                                break;
                            case Project.ItemType.Directory:
                                TQMG.Color(255, 255, 0);
                                if (y >= 0 && y < 8) Void.Font.DrawText($"D> {qstr.Str(" ", tab)}{key}/", OutX, TextY + 64 + (y * 16));
                                y++;
                                PS(tab + 1, V);
                                break;
                            default:
                                throw new Exception("Fatal Internal Error! Unknown filetype in file outline");
                        }
                       
                        
                    }
                }
                if (Project.ChosenProject != null) PS();
            }

            // Outline            
            if (Project.ChosenProject != null && Project.ChosenProject.CurrentItem != null) {
                TQMG.Color(0, 180, 255);
                var y = 0;
                var ty = TextY + 64 + 128;
                if (Project.ChosenProject.CurrentDoc.Outline.Count==0) {
                    TQMG.Color(255, 0, 180);
                    Void.Font.DrawText("Nothing to outline", OutX, ty);
                }
                foreach(string n in Project.ChosenProject.CurrentDoc.Outline.Keys) {                    
                    if(y>=OutLineScroll && ty < StatY-16) {
                        Void.Font.DrawText(n, OutX, ty);
                        ty += 16;
                    }
                }
            }

                // Status bar
                TQMG.Color(Color.White);
            Void.Back.Draw(0, StatY, TQMG.ScrWidth, 20);
            if (Project.ChosenProject == null || Project.ChosenProject.CurrentItem == null) {
                TQMG.Color(Color.Red);
                Void.Font.DrawText("No document", 0, StatY);
            } else {
                Void.Font.DrawText(Project.ChosenProject.CurrentItem.filename, 0, StatY);
                if (Project.ChosenProject.CurrentDoc != null)
                    Void.Font.DrawText($"(Ln:{Project.ChosenProject.CurrentDoc.posy + 1}; Pos:{Project.ChosenProject.CurrentDoc.posx + 1})", TQMG.ScrWidth - 10, StatY, TQMG_TextAlign.Right);
                else {
                    TQMG.Color(Color.Red);
                    Void.Font.DrawText("Doc Failure!",TQMG.ScrWidth - 10, StatY, TQMG_TextAlign.Right);
                }
                if (Insert) {
                    TQMG.Color(Color.White);
                    Void.Font.DrawText("Insert", TQMG.ScrWidth - 300, StatY, TQMG_TextAlign.Center);
                } else {
                    TQMG.Color(Color.Violet);
                    Void.Font.DrawText("Overwrite", TQMG.ScrWidth - 300, StatY, TQMG_TextAlign.Center);
                }
                if (Void.kb.CapsLock) {
                    TQMG.Color(Color.Tomato);
                    Void.Font.DrawText("Caps Lock", TQMG.ScrWidth - 400, StatY, TQMG_TextAlign.Center);
                }
            }



            // PullDown
            PullDownMenus.Draw();

        }

        Keys ReadKey => Void.ReadKey;
        char ReadChar => Void.ReadChar;

        int ChangedTimer = 0;
        List<Document.Line> ChangedLines = new List<Document.Line>();
        int OutLineCD = 0;

        public override void Update() {

            //if (ReadKey!=Keys.None) Debug.WriteLine($"ReadKey = {ReadKey} / {ReadChar}");
            if (ReadKey == Keys.Insert) Insert = !Insert;
            if (Doc != null) {
                switch (ReadKey) {
                    case Keys.Up: Doc.PosY--; break;
                    case Keys.Down: Doc.PosY++; break;
                    case Keys.Left: Doc.PosX--; break;
                    case Keys.Right: Doc.PosX++; break;
                    case Keys.PageDown: Doc.PosY += ((Editor.TextH - Editor.TextY) / 16); break;
                    case Keys.PageUp: Doc.PosY -= ((Editor.TextH - Editor.TextY) / 16); break;
                    case Keys.End: Doc.PosX = Doc.Lines[Doc.PosY].Rawline.Length; break;
                    case Keys.Home: Doc.PosX = 0; break;
                    case Keys.Back: {
                            if (Doc.PosX == Doc.Lines[Doc.PosY].Rawline.Length && Doc[Doc.PosY]!="") {
                                Doc[Doc.PosY] = qstr.Left(Doc[Doc.PosY], Doc[Doc.PosY].Length - 1);
                                Doc.PosX--;
                                ChangedTimer = 3;
                                ChangedLines.Add(Doc.Lines[Doc.PosY]);
                            }
                        } break;
                }
                if (ReadChar != '\0') {
                    if (Doc.PosX == Doc.Lines[Doc.PosY].Rawline.Length) {
                        ChangedTimer = 5;
                        ChangedLines.Add(Doc.Lines[Doc.PosY]);
                        Doc[Doc.PosY] += ReadChar;
                        Doc.PosX++;
                    } else {
                        var plus = 0;
                        if (!Insert) plus = 1;
                        Doc[Doc.PosY] = $"{Doc[Doc.PosY].Substring(0, Doc.PosX)}{ReadChar}{Doc[Doc.PosY].Substring(Doc.PosX + plus)}";
                        ChangedTimer = 6;
                        ChangedLines.Add(Doc.Lines[Doc.PosY]);
                        Doc.PosX++;
                    }
                }
                if (ChangedTimer > 0) {
                    OutLineCD += ChangedTimer;
                    --ChangedTimer;
                    if (ChangedTimer == 0) {
                        foreach (Document.Line L in ChangedLines) Doc.Lexer.Chop(L);
                        ChangedLines.Clear();
                    }
                }
                if (OutLineCD > 0) {
                    --OutLineCD;
                    if (OutLineCD == 0) Doc.Lexer.Outline(Doc);
                }
            }
            PullDownMenus.Update();
        }

        public override string ToString() => "Editor!";
        #endregion



    }
}



