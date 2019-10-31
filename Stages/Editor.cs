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

using Void.Parts;

namespace Void.Stages {
    class Editor:Stage {

        int TextX => 0;
        int TextY => 20;

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

            // Project FileList + Outline
            Void.Back.Draw(TextX + TextW, TextY, TextX + TextW, TextY, TQMG.ScrWidth - (TextX + TextW), TextW);


            // PullDown
            PullDownMenus.Draw();

        }

        public override void Update() {
            
        }

        public override string ToString() => "Editor!";
        #endregion



    }
}

