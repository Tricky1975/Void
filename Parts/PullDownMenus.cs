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
using System.Diagnostics;

using TrickyUnits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Void.Parts {





    class PullDownMenus {

        public delegate void CallBackVoid();
        public delegate bool RequireBool();
        public enum Heads { File, Edit, Search, Build, Recent }
        readonly RequireBool Always = delegate { return true};


        // Top bar
        readonly static TMap<Heads, PullDownMenus> Top = new TMap<Heads, PullDownMenus>();
        int TopX = 0;
        //PullDownMenus LinkMenu = null;
        List<PullDownMenus> Kids = new List<PullDownMenus>();

        // Item
        readonly Heads ParentTop;
        readonly PullDownMenus Parent;
        readonly CallBackVoid CallBack;
        readonly RequireBool Require;
        readonly Keys QuickKey = Keys.None;
        



        // Gen
        public string Caption = "";
        public TQMGText CaptGraph;

        // Creation
        private static void NOTHING() { } // Place holder delegate
        public PullDownMenus(Heads id) {
            Top[id] = this;
            switch (id) {
                case Heads.File:
                    new PullDownMenus(id, "Open Project Folder", NOTHING, Keys.F);
                    new PullDownMenus(id, "Save", NOTHING, Keys.S);
                    new PullDownMenus(id, "Save As", NOTHING);
                    new PullDownMenus(id, "Save All", NOTHING);
                    new PullDownMenus(id, "Quit", NOTHING);
                    break;
                case Heads.Edit:
                    new PullDownMenus(id, "Cut", NOTHING, Keys.X);
                    new PullDownMenus(id, "Copy", NOTHING, Keys.C);
                    new PullDownMenus(id, "Paste", NOTHING, Keys.V);
                    break;
                case Heads.Search:
                    new PullDownMenus(id, "Search", NOTHING, Keys.F);
                    new PullDownMenus(id, "Replace", NOTHING, Keys.H);
                    break;
                case Heads.Build:
                    new PullDownMenus(id, "Build", NOTHING, Keys.B);
                    new PullDownMenus(id, "Build + Run", NOTHING, Keys.R);
                    break;
            }
        }

        public PullDownMenus(Heads parent, string Caption, CallBackVoid CB, Keys QuickKey=Keys.None,RequireBool Requirement=null) {
            ParentTop = parent;
            Parent = Top[parent];
            Parent.Kids.Add(this);
            this.Caption = Caption;
            CaptGraph = Void.Font.Text(Caption);
            CallBack = CB;
            this.QuickKey = QuickKey;
            if (Requirement != null) Require = Requirement; else Require = Always;
            Debug.WriteLine($"Created menu item {Caption} in parent {parent}");
        }


        // Work
        static PullDownMenus Selected;
        static public void Draw() {
            TQMG.Color(Color.White);
            Void.Back.Draw(0, 0, 0, 0, TQMG.ScrWidth, 20);
            var nix = 5;
            foreach (Heads h in (Heads[])Enum.GetValues(typeof(Heads))) {
                if (Top[h] == null) {
                    new PullDownMenus(h);
                    Top[h].TopX = nix;
                    Top[h].Caption = $"{h}";
                    Top[h].CaptGraph = Void.Font.Text(Top[h].Caption, true);                    
                }
                if (Selected == Top[h]) {
                    TQMG.Color(0, 255, 255);
                    TQMG.DrawRectangle(nix - 5, 0, Top[h].CaptGraph.Width + 10,20);
                    TQMG.Color(Color.Black);
                } else {
                    TQMG.Color(0, 255, 255);
                }
                Top[h].CaptGraph.Draw(nix, 4);
                if (Void.ms.LeftButton == ButtonState.Pressed) {
                    if (Void.ms.X > nix && Void.ms.Y < 20 && Void.ms.X < nix + Top[h].CaptGraph.Width + 10) Selected = Top[h];
                }
                nix += Top[h].CaptGraph.Width + 10;
            }
        }

        static public void Update() {
            foreach (Heads h in (Heads[])Enum.GetValues(typeof(Heads)))
                foreach(PullDownMenus item in Top[h].Kids) {
                    if (item.QuickKey != Keys.None && Void.kb.IsKeyDown(item.QuickKey) && (!Void.kb.IsKeyDown(item.QuickKey)) && (Void.kb.IsKeyDown(Keys.LeftControl) || Void.kb.IsKeyDown(Keys.RightControl) && item.Require())
                        item.CallBack();
                    
                }
        }
    }
}

