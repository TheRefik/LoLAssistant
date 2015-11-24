using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant.Classes.LiveMatch
{
    public class Participant
    {
        public int teamId { get; set; }
        public int spell1Id { get; set; }
        public int spell2Id { get; set; }
        public int championId { get; set; }
        public int profileIconId { get; set; }
        public string summonerName { get; set; }
        public bool bot { get; set; }
        public int summonerId { get; set; }
        public List<Rune> runes { get; set; }
        public List<Mastery> masteries { get; set; }
    }
}
