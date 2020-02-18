namespace Jeffistance.Models
{
    public interface IFaction
    {
        string Name { get; }
    }

    public class ResistanceFaction : IFaction
    {
        public string Name { get; } = "Resistance";
    }

    public class FactionFactory
    {
        private ResistanceFaction resistance;

        public IFaction MakeResistance()
        {
            return resistance ??= new ResistanceFaction();
        }
    }
}
