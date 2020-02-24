namespace Jeffistance.Models
{
    public class Player
    {
        public IFaction Faction { get; set; }
        public IRole Role { get; set; }
        public int ID { get; set; } = -1;
        public string Name { get; set; }
        public bool IsLeader { get; set; } = false;
    }
}
