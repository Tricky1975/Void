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

using TrickyUnits;

namespace Void.Stages {
    abstract class Stage {
        readonly protected static TMap<string, Stage> Stages = new TMap<string, Stage>();
        protected static Stage Current = null;

        abstract public void Draw();
        abstract public void Update();

        static public void DrawStage() {
            if (Current != null) Current.Draw();
        }

        static public void UpdateStage() {
            if (Current != null) Current.Update();
        }


        protected void Register(string name) { Stages[name] = this; }
        public static void GoTo(string name) {
            Void.Assert(Stages[name], $"Stage \"{name}\" apparently doesn't exist!");
            GoTo(Stages[name]);
        }
        public static void GoTo(Stage s) { if (s != null) Current = s; }
    }
}




