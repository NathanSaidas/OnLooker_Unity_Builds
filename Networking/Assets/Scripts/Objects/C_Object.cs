using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class C_Object : MonoBehaviour {

        [SerializeField()]
        private NetworkHandle m_Handle = new NetworkHandle();



        [RPC]
        private void setHandle(NetworkPlayer aOwner, string aUsername, int aUniqueID,NetworkMessageInfo aInfo)
        {
            if (m_Handle != null)
            {
                m_Handle.networkPlayer = aOwner;
                m_Handle.username = aUsername;
                m_Handle.uniqueID = aUniqueID;
                C_Object sender = aInfo.networkView.GetComponent<C_Object>();
                if (sender != null)
                {
                    m_Handle.server = sender.handle.server;
                }
                else
                {
                    Debug.Log("No Sender!");
                }
                
                C_ObjectSpawner.spawner.onObjectSpawn(this);
            }
        }
        [RPC]
        private void flagDespawn()
        {
            C_ObjectSpawner.spawner.onObjectDespawn(Network.player);
            Network.RemoveRPCs(gameObject.networkView.viewID);
        }

        public void onSpawn(NetworkPlayer aOwner, string aUsername, int aUniqueID,S_ObjectSpawner aServer)
        {
            handle.server = aServer;
            networkView.RPC("setHandle", RPCMode.AllBuffered, aOwner, aUsername, aUniqueID);
        }
        public void onFlagDespawn()
        {
            networkView.RPC("flagDespawn", RPCMode.AllBuffered);
        }
        public NetworkHandle handle
        {
            get { return m_Handle; }
        }

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () 
        {
		
		}

        void OnGUI()
        {
            //some code
            
        }
	}

}