using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant.Classes.Statistics
{
    public class StatsInfo
    {
        public int summonerId { get; set; }
        public long modifyDate { get; set; }
        public List<Champion> champions { get; set; }
    }
}
