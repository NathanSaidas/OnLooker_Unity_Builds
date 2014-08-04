using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OnLooker
{
    namespace Framework
    {

        [Serializable]
        public class OptionsSaveData : CustomSaveData
        {
            private int m_Difficulty;
            private string m_CurrentQualityLevel;
            private float m_CurrentVolume;
            private float m_MutedVolume;
            private bool m_Muted;

            public OptionsSaveData()
            {
                name = "Options";
            }
            public OptionsSaveData(string aName)
            {
                name = aName;
            }

            public OptionsSaveData(SerializationInfo aInfo, StreamingContext aContext)
            {
                name = (string)aInfo.GetValue("Name", typeof(string));
                m_Difficulty = (int)aInfo.GetValue("Difficulty", typeof(int));
                m_CurrentQualityLevel = (string)aInfo.GetValue("CurrentQualityLevel", typeof(string));
                m_CurrentVolume = (float)aInfo.GetValue("CurrentVolume", typeof(float));
                m_MutedVolume = (float)aInfo.GetValue("MutedVolume", typeof(float));
                m_Muted = (bool)aInfo.GetValue("Muted", typeof(bool));
            }

            public override void GetObjectData(SerializationInfo aInfo, StreamingContext aContext)
            {
                base.GetObjectData(aInfo, aContext);
                aInfo.AddValue("Difficulty", m_Difficulty);
                aInfo.AddValue("CurrentQualityLevel", m_CurrentQualityLevel);
                aInfo.AddValue("CurrentVolume", m_CurrentVolume);
                aInfo.AddValue("MutedVolume", m_MutedVolume);
                aInfo.AddValue("Muted", m_Muted);
            }

            public int difficulty
            {
                get { return m_Difficulty; }
                set { m_Difficulty = value; }
            }
            public string currentQualityLevel
            {
                get { return m_CurrentQualityLevel; }
                set { m_CurrentQualityLevel = value; }
            }
            public float currentVolume
            {
                get { return m_CurrentVolume; }
                set { m_CurrentVolume = value; }
            }
            public float mutedVolume
            {
                get { return m_MutedVolume; }
                set { m_MutedVolume = value; }
            }
            public bool muted
            {
                get { return m_Muted; }
                set { m_Muted = value; }
            }
        }
    }
}
