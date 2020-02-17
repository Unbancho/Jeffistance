using System.Collections.Generic;

namespace Jeffistance.Models
{
    public interface IGamemode
    {
        FactionFactory Factory { get; set; }
        void AssignFactions(IEnumerable<Player> players);
    }

    public class StandardGamemode : IGamemode
    {
        public FactionFactory Factory { get; set; } = new FactionFactory();

        public void AssignFactions(IEnumerable<Player> players)
        {
            foreach (Player player in players)
            {
                player.Faction = Factory.MakeResistance();
            }
        }
    }
}
