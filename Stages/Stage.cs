using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrickyUnits;

namespace Void.Stages {
    abstract class Stage {
        readonly protected static TMap<string, Stage> Stages = new TMap<string, Stage>();

        abstract public void Draw();
        abstract public void Update();
        
    }
}
