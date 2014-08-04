using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OnLooker
{
        [Serializable]
        public abstract class CustomSaveData : SaveData, ISerializable
        {
            
            public override void save(Stream aStream, BinaryFormatter aFormatter)
            {
                if (aStream != null && aFormatter != null)
                {
                    aFormatter.Serialize(aStream, this);
                }
            }
            //Override this method for additional data make sure to call the base
            public virtual void GetObjectData(SerializationInfo aInfo, StreamingContext aContext)
            {
                aInfo.AddValue("Name", name);
            }
        }
}
