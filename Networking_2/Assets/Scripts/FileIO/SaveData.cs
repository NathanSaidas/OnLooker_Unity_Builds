using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OnLooker
{
        /// <summary>
        /// This is the base class for all save data for OnLooker.FileIO
        /// </summary>
    [Serializable]
    abstract public class SaveData
    {
        protected string m_Name;
        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public SaveData()
        {
            m_Name = string.Empty;
        }
        public SaveData(string aName)
        {
            m_Name = aName;
        }
        abstract public void save(Stream aStream, BinaryFormatter aFormatter);
    }
}
