using UnityEngine;
using System;
using System.Collections.Generic;

namespace OnLooker
{
    [Serializable()]
	public class C_NetPlayer : MonoBehaviour{

        [SerializeField()]
        private NetworkPlayer m_Owner;
        [SerializeField()]
        private string m_PlayerName;
        private string m_PlayerPassword;

        [RPC]
        public void setOwner(NetworkPlayer aPlayer, string aUsername)
        {
            m_Owner = aPlayer;
            m_PlayerName = aUsername;
            if (aPlayer == Network.player)
            {
                enabled = true;
            }
        }
        [RPC]
        public NetworkPlayer getOwner()
        {
            return m_Owner;
        }
        [RPC]
        public string getUsername()
        {
            return m_PlayerName;
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

        private void Update()
        {
            //If this is a server update call return.
            if (Network.isServer)
            {
                return;
            }
            //Client input state changes go here
            if (m_Owner != null && m_Owner == Network.player)
            {

            }  
        }

       
        
		
	}

}