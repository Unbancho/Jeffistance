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
        public override string ToString(){ return Text;}
        public Dictionary<string, object> PackedObjects;

        public object this[string name]
        {
            get{ return PackedObjects[name]; }
            set{ PackObject(value, name); }
        }
        public Enum Flags;
        public string Text;

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

        public object Pop(string name=null)
        {
            if(name == null)
            {
                foreach (KeyValuePair<string, object> entry in PackedObjects)
                {
                   return (obj:UnpackObject(entry.Key, pop:true), name:entry.Key);
                }
                throw new EmptyMessageException();
            }
            return UnpackObject(name, pop:true);
        }

        public bool TryPop(out object result, string name=null)
        {
            result = null;
            try
            {
                result = Pop(name);
                return true;
            }
            catch(Exception e) when (e is EmptyMessageException || e is KeyNotFoundException)
            {
                return false;
            }
        }

        public bool HasFlag(Enum flag)
        {
            return Flags.HasFlag((MessageFlags)flag);
        }

        protected Message(SerializationInfo info, StreamingContext context)
        {
            PackedObjects = new Dictionary<string, object>();
            foreach (SerializationEntry entry in info)
            {
                PackObject(entry.Value, entry.Name);
            }
            Text = (string) Pop("Text");
            Flags = (Enum) Pop("Flags");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (KeyValuePair<string, object> entry in PackedObjects)
            {
                info.AddValue(entry.Key, entry.Value);
            }
            info.AddValue("Text", Text);
            info.AddValue("Flags", Flags);
        }
    }

    public class EmptyMessageException : Exception {}

}