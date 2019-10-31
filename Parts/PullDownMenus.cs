using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrickyUnits;

namespace Void.Parts {





    class PullDownMenus {

        public enum Heads { File, Edit, Search, Build, Recent }

        // Top bar
        readonly static TMap<Heads, PullDownMenus> Top = new TMap<Heads, PullDownMenus>();
        int TopX = 0;
        PullDownMenus LinkMenu = null;

        // Gen
        public string Caption = "";
        public TQMGText CaptGraph;

        // Work
        static PullDownMenus Selected;
        static public void Draw() {
            TQMG.Color(Microsoft.Xna.Framework.Color.White);
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
                    // TODO: Mark Selected
                } else {
                    TQMG.Color(0, 255, 255);
                }
                Top[h].CaptGraph.Draw(nix, 4);
                nix += Top[h].CaptGraph.Width + 10;
            }
        }
    }
}
