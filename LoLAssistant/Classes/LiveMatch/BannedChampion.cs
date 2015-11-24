using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLAssistant.Classes.LiveMatch
{
    public class BannedChampion
    {
        public int championId { get; set; }
        public int teamId { get; set; }
        public int pickTurn { get; set; }
    }
}
