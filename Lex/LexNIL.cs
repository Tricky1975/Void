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

namespace Void.Lex {
    class LexNIL:LexPL {

        public LexNIL() {
            KeyWords = "and break do else elseif end false for function if in local nil not or repeat return then true until while void number int table string bool boolean class CONSTRUCTOR NEW userdata switch case private global NIL static get set forever #use #localuse #globaluse readonly var self default module DESTRUCTOR delegate NIL new group link with extends abstract final quickmeta".Split(' ');
        }
    }
}

