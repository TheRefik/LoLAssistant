using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant.Classes.LiveMatch
{
    public class Entry
    {
        public int leaguePoints { get; set; }
        public bool isFreshBlood { get; set; }
        public bool isHotStreak { get; set; }
        public string division { get; set; }
        public bool isInactive { get; set; }
        public bool isVeteran { get; set; }
        public int losses { get; set; }
        public string playerOrTeamName { get; set; }
        public string playerOrTeamId { get; set; }
        public int wins { get; set; }
    }

    public class part
    {
        public string queue { get; set; }
        public string name { get; set; }
        public List<Entry> entries { get; set; }
        public string tier { get; set; }
    }

    public class Divisions
    {
        public List<part> part { get; set; }
    }
}
