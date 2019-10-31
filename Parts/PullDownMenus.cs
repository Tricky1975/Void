using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrickyUnits;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Void.Parts {





    class PullDownMenus {

        public delegate void CallBackVoid();
        public enum Heads { File, Edit, Search, Build, Recent }

        // Top bar
        readonly static TMap<Heads, PullDownMenus> Top = new TMap<Heads, PullDownMenus>();
        int TopX = 0;
        PullDownMenus LinkMenu = null;

        // Item
        readonly Heads ParentTop;
        readonly PullDownMenus Parent;
        readonly CallBackVoid CallBack;
        readonly Keys QuickKey = Keys.None;
        



        // Gen
        public string Caption = "";
        public TQMGText CaptGraph;

        // Work
        static PullDownMenus Selected;
        static public void Draw() {
            TQMG.Color(Color.White);
            Void.Back.Draw(0, 0, 0, 0, TQMG.ScrWidth, 20);
            var nix = 5;
            foreach (Heads h in (Heads[])Enum.GetValues(typeof(Heads))) {
                if (Top[h] == null) {
                    Top[h] = new PullDownMenus();
                    Top[h].TopX = nix;
                    Top[h].Caption = $"{h}";
                    Top[h].CaptGraph = Void.Font.Text(Top[h].Caption, true);
                    Top[h].LinkMenu = new PullDownMenus();
                }
                if (Selected == Top[h]) {
                    TQMG.Color(0, 255, 255);
                    TQMG.DrawRectangle(nix - 5, 0, Top[h].CaptGraph.Width + 10,20);
                    TQMG.Color(Color.Black);
                } else {
                    TQMG.Color(0, 255, 255);
                }
                Top[h].CaptGraph.Draw(nix, 4);
                if (Void.ms.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed) {
                    if (Void.ms.X > nix && Void.ms.Y < 20 && Void.ms.X < nix + Top[h].CaptGraph.Width + 10) Selected = Top[h];
                }
                nix += Top[h].CaptGraph.Width + 10;
            }
        }
    }
}
