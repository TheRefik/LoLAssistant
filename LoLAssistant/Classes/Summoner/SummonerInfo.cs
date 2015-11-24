using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant.Classes.Summoner
{
    public class SummonerInfo
    {
        public int id { get; set; }
        public string name { get; set; }
        public int profileIconId { get; set; }
        public int summonerLevel { get; set; }
        public long revisionDate { get; set; }
    }
}
