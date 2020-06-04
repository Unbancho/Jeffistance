namespace Jeffistance.Common.Models
{
    public interface IFaction
    {
        string Name { get; }
    }

    public class ResistanceFaction : IFaction
    {
        public string Name { get; } = "Resistance";
    }

    public class SpiesFaction : IFaction
    {
        public string Name { get;} = "Spies";
    }

    public class FactionFactory
    {
        private ResistanceFaction resistance;
        private SpiesFaction spies;

        public IFaction GetResistance()
        {
            return resistance ??= new ResistanceFaction();
        }

        public IFaction GetSpies()
        {
            return spies ??= new SpiesFaction();
        }
    }
}
