using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Jeffistance.Services.Messaging
{
    [Flags]
    public enum MessageFlags
    {
        None = 0,
        Greeting = 1,
        Broadcast = 2,
    }

    [Serializable]
    public class Message : ISerializable
    {
        public Dictionary<string, object> PackedObjects;

        public object this[string name]
        {
            get{ return UnpackObject(name, pop:false); }
            set{ PackObject(value, name); }
        }
        public Enum Flags
        {
            get{ return (Enum) this["Flags"]; }
            set{ this["Flags"] = value; }
        }
        public string Text
        {
            get{ return (string) this["Text"]; }
            set{ this["Text"] = value; }
        }

        public Message(string text=null, Enum flags=null, params object[] objectsToPack)
        {
            PackedObjects = new Dictionary<string, object>();
            Text = text == null ? String.Empty : text;
            Flags = flags == null ? MessageFlags.None : flags;
            foreach (var obj in objectsToPack)
            {
                PackObject(obj);
            }
        }

         public void PackObject<T>(T obj, string name=null)
        {
            if(name == null)
            {
                name = obj.GetType().Name;
            }
            PackedObjects[name] = obj;
        }

        public object UnpackObject(string name, bool pop=false)
        {
            object obj = PackedObjects[name];
            if(pop) PackedObjects.Remove(name);
            return obj;
        }

        public object Pop(string name)
        {
            return UnpackObject(name, pop:true);
        }

        public bool HasFlag(Enum flag)
        {
            return Flags.HasFlag(flag);
        }

        protected Message(SerializationInfo info, StreamingContext context)
        {
            PackedObjects = new Dictionary<string, object>();
            foreach (SerializationEntry entry in info)
            {
                PackedObjects.Add(entry.Name, entry.Value);
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (KeyValuePair<string, object> entry in PackedObjects)
            {
                info.AddValue(entry.Key, entry.Value);
            }
        }
    }
}