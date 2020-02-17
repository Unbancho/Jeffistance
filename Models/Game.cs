using System.Collections.Generic;

namespace Jeffistance.Models
{
    public class Game
    {
        private IGamemode gamemode;

        public bool InProgress = false;
        public List<Player> Players;
        public IGamemode Gamemode { get => gamemode; set => gamemode = value; }

        public Game(IGamemode gm)
        {
            Gamemode = gm;
        }

        public void Start(IEnumerable<Player> players)
        {
            Players = new List<Player>(players);
            InProgress = true;
            Gamemode.AssignFactions(Players);
            LeaderPhase();
        }

        private void LeaderPhase()
        {
            
        }
    }
}
