using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    //A network handle is a reference to match a player with an object as well as provide a way to
    //retrieve object data
    [System.Serializable]
	public class NetworkHandle
    {
        //Reference ownership by networkPlayer and username
        //Reference instance_id on server by id
        [SerializeField]
        private NetworkPlayer m_NetworkPlayer;  //Who owns this (GUID/IP)
        [SerializeField]
        private string m_Username;              //Who owns this handle (Whats the users name)
        //private string m_Password;            //Later for Security
        [SerializeField]
        private int m_UniqueID;                 //Unique number ID shared across all clients and the server

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
        public int id
        {
            get { return m_UniqueID; }
            set { m_UniqueID = value; }
        }
    }

}