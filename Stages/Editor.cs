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

using Microsoft.Xna.Framework;

using TrickyUnits;

using Void.Parts;

namespace Void.Stages {
    class Editor:Stage {

        int TextX => 0;
        int TextY => 20;

        int ProjectScroll = 0;
        int FileScroll = 0;

        readonly int TextW;
        int TextH = TQMG.ScrHeight - 40;

        #region Init
        public Editor() {
            Register("Editor");
            TextW = (int)Math.Floor(TQMG.ScrWidth * .75);
        }
        #endregion

        #region Callbacks
        public override void Draw() {

            // Document Content
            Void.VoidBack.Draw((TextX + (TextW / 2)) - (Void.VoidBack.Width / 2), (TextY + (TextH / 2)) - (Void.VoidBack.Height / 2));

            // Project, FileList and Outline
            var OutX = TextX + TextW;
            var OutW = TQMG.ScrWidth - (TextX + TextW);
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
                                    if (Void.ms.Y > iy && Void.ms.Y < iy + 16 && Void.ms.X > OutX && Void.ms.X < TQMG.ScrWidth - 10) {
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

            // Status bar
            var StatY = TQMG.ScrHeight - 20;
            TQMG.Color(Color.White);
            Void.Back.Draw(0, StatY, TQMG.ScrWidth, 20);
            if (Project.ChosenProject == null || Project.ChosenProject.CurrentItem == null) {
                TQMG.Color(Color.Red);
                Void.Font.DrawText("No document", 0, StatY);
            } else {
                Void.Font.DrawText(Project.ChosenProject.CurrentItem.filename,0,StatY);
            }

            // PullDown
            PullDownMenus.Draw();

        }

        public override void Update() {
            PullDownMenus.Update();
        }

        public override string ToString() => "Editor!";
        #endregion



    }
}



