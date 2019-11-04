using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            } else {
                CFG = GINI.ReadFromFile(ConfigFile);
            }
        }
    }
}
