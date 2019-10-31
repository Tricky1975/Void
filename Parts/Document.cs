using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Void.Parts {

    class Document {
        
        public class Line {
            string Rawline = "";
            string[] Words = null;
            public override string ToString() => Rawline;
            public override bool Equals(object obj) {
                if (obj.GetType() == typeof(Line))
                    return obj.ToString() == Rawline;
                else if (obj.GetType() == typeof(string))
                    return (string)obj == Rawline;
                else
                    throw new Exception("Illegal Line comparing");
            }
            public void Define(string str) {
                Rawline = str;
                // TODO: Chop up!
            }
        }

        List<Line> Lines = new List<Line>();
        public string this[int l] {
            get => Lines.ElementAt(l).ToString();
            set => Lines.ElementAt(l).Define(value);            
        }

    }
}
