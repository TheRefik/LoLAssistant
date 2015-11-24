using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant.Classes.LiveMatch
{
    public class LiveMatch
    {
        public long gameId { get; set; }
        public long mapId { get; set; }
        public string gameMode { get; set; }
        public string gameType { get; set; }
        public int gameQueueConfigId { get; set; }
        public List<Participant> participants { get; set; }
        public Observers observers { get; set; }
        public string platformId { get; set; }
        public List<BannedChampion> bannedChampions { get; set; }
        public long gameStartTime { get; set; }
        public long gameLength { get; set; }
    }
}
