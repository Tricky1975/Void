using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void.Lex {
    class LexNIL:LexPL {

        public LexNIL() {
            KeyWords = "and break do else elseif end false for function if in local nil not or repeat return then true until while void number int table string bool boolean class CONSTRUCTOR NEW userdata switch case private global NIL static get set forever #use #localuse #globaluse readonly var self default module DESTRUCTOR delegate NIL new group link with extends abstract final quickmeta".Split(' ');
        }
    }
}
