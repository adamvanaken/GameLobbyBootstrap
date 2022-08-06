using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Models.Messages
{
    public class Player
    {
        public Player(string playerName, int idx, Guid id)
        {
            name = playerName;
            index = idx;
            uuid = id;
        }

        public bool isReady { get; set; }
        public Guid uuid { get; set; }
        public string name { get; set; }
        public int index { get; set; }
    }
}
