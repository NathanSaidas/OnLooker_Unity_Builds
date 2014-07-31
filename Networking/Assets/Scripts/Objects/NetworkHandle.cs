using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    [System.Serializable]
	public class NetworkHandle
    {
        [SerializeField]
        private NetworkPlayer m_NetworkPlayer;  //Who owns this (GUID/IP)
        [SerializeField]
        private string m_Username;              //Who owns this handle (Whats the users name)
        //private string m_Password;            //Later for Security
        [SerializeField]
        private int m_UniqueID;                 //Unique number ID shared across all clients and the server
        [SerializeField]
        private S_ObjectSpawner m_Server;       //A reference to the server
        //private NetworkPlayer m_NetworkServer;  //A handle to the server's ID

        public NetworkPlayer networkPlayer
        {
            get { return m_NetworkPlayer; }
            set { m_NetworkPlayer = value; }
        }
        public string username
        {
            get { return m_Username; }
            set { m_Username = value; }
        }
        public int uniqueID
        {
            get { return m_UniqueID; }
            set { m_UniqueID = value; }
        }
        public S_ObjectSpawner server
        {
            get { return m_Server; }
            set { m_Server = value; }
        }
    }

}