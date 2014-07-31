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
        private string m_Username;
        [SerializeField()]
        private int m_UniqueID;

        [RPC]
        public void setOwner(NetworkPlayer aPlayer, string aUsername)
        {
            m_Owner = aPlayer;
            m_Username = aUsername;
            if (aPlayer == Network.player)
            {
                enabled = true;
            }
        }

        public NetworkPlayer owner
        {
            get { return m_Owner; }
        }
        [RPC]
        public string getUsername()
        {
            return m_Username;
        }

        public string username
        {
            get { return m_Username; }
            set { m_Username = value; }
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