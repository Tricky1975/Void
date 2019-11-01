using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TrickyUnits;

namespace Void.Parts {
    class Project {

        readonly internal TMap<string, Item> ItemMap = new TMap<string, Item>();
        readonly internal TMap<string, Item> TreeItemMap = new TMap<string, Item>();
        static readonly internal TMap<string, Item> ProjMap = new TMap<string, Item>();
        string ProjectDirectory;

        enum ItemType { NonExistent,File,Directory }

        internal class Item {
            public Document Doc=null;
            readonly public TMap<string, Item> SubDirectory = new TMap<string, Item>();
            public Item Parent = null;
            public Project Prj = null;
            public string filename = "";
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
                    for (int ak = 0; ak < dc.Length - 1; ak++) cd = cd[dc[ak]].SubDirectory;
                    cd[qstr.StripDir(f)] = i;
                }
            }
        }

        internal static void OpenProject() {
            var r = FFS.RequestDir();
        }
    }
}
