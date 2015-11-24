using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant.Classes.Division
{
    public class Division
    {
        public string name { get; set; }
        public string tier { get; set; }
        public string queue { get; set; }
        public List<Entry> entries { get; set; }
    }
}
