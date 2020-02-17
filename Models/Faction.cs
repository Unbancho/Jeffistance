namespace Jeffistance.Models
{
    public class Faction
    {
        public string Name { get; set; }
    }

    public class FactionFactory
    {
        private Faction resistance;

        public Faction MakeResistance()
        {
            return resistance ??= new Faction() { Name = "Resistance" };
        }
    }
}
