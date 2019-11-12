// Lic:
// Void
// ....
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

using Void.Lex;

namespace Void.Parts {
    class Project {

        readonly internal TMap<string, Item> ItemMap = new TMap<string, Item>();
        readonly internal TMap<string, Item> TreeItemMap = new TMap<string, Item>();
        static readonly internal TMap<string, Project> ProjMap = new TMap<string, Project>();
        static internal string ChosenProjectID = "";
        static internal Project ChosenProject {
            get {
                if (ChosenProjectID.Trim() == "") return null;
                return ProjMap[ChosenProjectID];
            }
        }
        string ProjectDirectory;

        internal enum ItemType { NonExistent, File, Directory }
        TGINI GINIConfig;
        string ConfigFile => $"{ProjectDirectory}/Void_Project.GINI";
        internal class prjcfg {
            Project Parent;
            internal string this[string key] { get => Parent.GINIConfig.C(key); set { Parent.GINIConfig.D(key, value); Parent.GINIConfig.SaveSource(Parent.ConfigFile); } }
            internal prjcfg(Project Ouder) { Parent = Ouder; }
        }
        internal prjcfg CFG;

        internal class Item {
            public Document Doc = null;
            readonly public TMap<string, Item> SubDirectory = new TMap<string, Item>();
            public Item Parent = null;
            public Project Prj = null;
            public string filename = "";
            internal ItemType Type;
            internal void Load() {
                Doc = Document.Load(filename);
            }
        }

        internal Item CurrentItem = null;
        internal Document CurrentDoc {
            get {
                if (CurrentItem == null)
                    return null;
                else {
                    if (CurrentItem.Doc == null) {
                        var t = "";
                        try {
                            t = QuickStream.LoadString(CurrentItem.filename);
                        } catch (Exception FoutjeBedankt) {
                            Confirm.Annoy($"Failed to load document:\n\n{CurrentItem.filename}\n\n{FoutjeBedankt.Message}");
                            return null;
                        }
                        CurrentItem.Doc = new Document(t,LexBase.Reg[qstr.ExtractExt(CurrentItem.filename.ToLower())]);
                    }
                    return CurrentItem.Doc;
                }
            }
        }

        public Project(string dir) {
            if (!Void.Assert(Directory.Exists(dir),$"Directory {dir} does not exist")) return;
            ProjectDirectory = dir;
            var Lijst = FileList.GetTree(dir);
            foreach(string f in Lijst) {
                if (qstr.ExtractExt(f.ToLower()) == "nil") {
                    var i = new Item();
                    i.filename = $"{dir}/{f}";
                    TreeItemMap[f] = i;
                    var dc = f.Split('/');
                    var cd = ItemMap;
                    Item ci = null;
                    for (int ak = 0; ak < dc.Length - 1; ak++) {
                        if (cd[dc[ak]] == null) {
                            var ni = new Item();
                            cd[dc[ak]] = ni;
                            ni.Parent = ci;
                            ni.Type = ItemType.Directory;
                            ni.Prj = this;
                        }
                        ci = cd[dc[ak]];
                        cd = cd[dc[ak]].SubDirectory;
                        i.Parent = ci;
                    }
                    i.Type = ItemType.File;
                    cd[qstr.StripDir(f)] = i;
                    Debug.WriteLine($"Project: {dir};\t\t{f} found and added!   ({i.Type})");
                }
            }
            if (!Config.Has("Projects", dir)) Config.Add("Projects", dir);
            CFG = new prjcfg(this);
            if (!File.Exists(ConfigFile)) {
                GINIConfig = new TGINI();
                CFG["Title"] = dir;
                CFG["FILE_Created"] = $"{DateTime.Now}";
            } else {
                GINIConfig = GINI.ReadFromFile(ConfigFile);
            }
        }

        internal static void OpenProject() {
            var r = FFS.RequestDir().Replace("\\","/");
            if (r == "") return;
            if (ProjMap[r] == null) ProjMap[r] = new Project(r); else Debug.WriteLine($"Project  \"{r}\" already loaded, so won't load again!");
            
        }
    }
}


