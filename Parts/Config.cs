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
// Version: 19.11.12
// EndLic

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using TrickyUnits;

namespace Void.Parts {
    static class Config {

        static TGINI CFG;
        static internal string ConfigFile => Dirry.C("$AppSupport$/Void.GINI");

        internal class ConfigDefine {
            internal string this[string key] {
                get => CFG.C(key);
                set {
                    CFG.D(key, value);
                    CFG.SaveSource(ConfigFile);
                }
            }
        }

        static internal ConfigDefine VAL = new ConfigDefine();
        
        static public void Add(string list,string value) {
            CFG.List(list).Add(value);
            CFG.SaveSource(ConfigFile);
        }

        static public List<string> GetL(string list) => CFG.List(list);

        static public bool Has(string list, string value) => CFG.List(list).Contains(value);
        

        static Config() {
            Dirry.InitAltDrives();
            if (!File.Exists(ConfigFile)) {
                CFG = new TGINI();
                VAL["FILE_CREATED"] = $"{DateTime.Now}";
                Debug.WriteLine($"Config file \"{ConfigFile}\" created!");
            } else {
                CFG = GINI.ReadFromFile(ConfigFile);
                Debug.WriteLine($"Config file \"{ConfigFile}\" loaded!");
            }
        }
    }
}


