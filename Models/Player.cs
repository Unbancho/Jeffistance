namespace Jeffistance.Models
{
    public class Player
    {
        private IFaction faction;
        private IRole role;

        public IFaction Faction { get => faction; set => faction = value; }
        public IRole Role { get => role; set => role = value; }
        public int ID { get; set; } = -1;
        public string Name { get; set; }
    }
}
