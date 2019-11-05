using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Void;
using Void.Parts;

using TrickyUnits;

namespace Void.Lex {
    abstract class LexBase {
        abstract public void Chop(Document.Line line);
        abstract public void Outline(Document doc);
        readonly static public TMap<string, LexBase> Reg = new TMap<string, LexBase>();
    }

    class LexPL : LexBase {
        // PL = Programming language... For most programming languages this will work.
        public override void Chop(Document.Line line) {
            throw new NotImplementedException();
        }

        public override void Outline(Document doc) {            
            throw new NotImplementedException();
        }

        protected string[] KeyWords;

    }
}
