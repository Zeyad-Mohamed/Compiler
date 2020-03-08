using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compiler
{
    public class Variable
    {
        public Variable()
        {
            this.name = "";
            this.value = "0";
            this.type = "";
        }
        public string name;
        public string type;
        public string value;
    }
}
