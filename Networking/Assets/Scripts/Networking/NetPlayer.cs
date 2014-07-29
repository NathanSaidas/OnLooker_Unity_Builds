using UnityEngine;
using System;
using System.Collections.Generic;

namespace OnLooker
{
    [Serializable()]
	public class NetPlayer {

        private NetworkPlayer m_NetworkID;
        private string m_PlayerName;
        private string m_PlayerPassword;

        public NetPlayer()
        {
            m_NetworkID = Network.player;
        }

        public NetworkPlayer networkID
        {
            get { return m_NetworkID; }
        }
        public string playerName
        {
            get { return m_PlayerName; }
            set { m_PlayerName = value; }
        }
        public string playerPassword
        {
            get { return m_PlayerPassword; }
            set { m_PlayerPassword = value; }
        }
        
		
	}

}