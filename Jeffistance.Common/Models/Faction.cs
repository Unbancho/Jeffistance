using System;
using System.Runtime.Serialization;

namespace Jeffistance.Common.Models
{
    public interface IFaction
    {
        string Name { get; }
    }

    [Serializable]
    public class ResistanceFaction : IFaction, ISerializable
    {
        public string Name { get; } = "Resistance";

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
        }

        public ResistanceFaction(){}

        protected ResistanceFaction(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
        }
    }

    [Serializable]
    public class SpiesFaction : IFaction, ISerializable
    {
        public string Name { get;} = "Spies";

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
        }

        public SpiesFaction(){}

        protected SpiesFaction(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
        }
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
