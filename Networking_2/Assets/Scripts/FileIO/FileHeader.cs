using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OnLooker
{
        /// <summary>
        /// The header that has data for all OnLooker.FileIO
        /// </summary>
        [Serializable]
        public sealed class FileHeader : SaveData, ISerializable
        {
            private string m_Filename = string.Empty;
            private int m_ItemCount = 0;


            public FileHeader()
            {
                
            }

            public FileHeader(string aFilename, int aItemCount)
            {
                m_Filename = aFilename;
                m_ItemCount = aItemCount;
            }

            public string filename
            {
                get { return m_Filename; }
                set { m_Filename = value; }
            }
            public int itemCount
            {
                get { return m_ItemCount; }
                set { m_ItemCount = value; }
            }

            public override void save(Stream aStream, BinaryFormatter aFormatter)
            {
                if (aStream != null && aFormatter != null)
                {
                    aFormatter.Serialize(aStream, this);
                }
            }


            public FileHeader(SerializationInfo aInfo, StreamingContext aContext)
            {
                name = (string)aInfo.GetValue("Name", typeof(string));
                m_Filename = (string)aInfo.GetValue("Filename", typeof(string));
                m_ItemCount = (int)aInfo.GetValue("ItemCount", typeof(int));
            }

            public void GetObjectData(SerializationInfo aInfo, StreamingContext aContext)
            {
                aInfo.AddValue("Name", name);
                aInfo.AddValue("Filename", m_Filename);
                aInfo.AddValue("ItemCount", m_ItemCount);
            }
        }
}
