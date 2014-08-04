using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OnLooker
{
    [Serializable]
	public class NetworkHandle : ISerializable
    {
        [SerializeField]
        private NetworkPlayer m_Player;
        [SerializeField]
        private string m_Username;
        [SerializeField]
        private int m_UniqueID;


        public NetworkPlayer player
        {
            get { return m_Player; }
            set { m_Player = value; }
        }
        public string username
        {
            get { return m_Username; }
            set { m_Username = value; }
        }
        public int id
        {
            get { return m_UniqueID; }
            set { m_UniqueID = value; }
        }

        public NetworkHandle()
        {

        }

        public NetworkHandle(SerializationInfo info, StreamingContext context)
        {
            m_Username = (string)info.GetValue("Username", typeof(string));
            m_UniqueID = (int)info.GetValue("Id", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Username", m_Username);
            info.AddValue("Id", m_UniqueID);
        }
    }

}