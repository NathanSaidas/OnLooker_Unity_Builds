using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class C_ObjectSpawner : MonoBehaviour 
    {
        static public C_ObjectSpawner spawner = null;

        [SerializeField()]
        private Rect m_GUI = new Rect();

        //private List<C_Object> m_Objects = new List<C_Object>();

        [SerializeField()]
        private C_Object m_CurrentObject = null;
        [SerializeField()]
        private int m_HandleID = 0;

		void Start () 
        {
            if (spawner == null)
            {
                spawner = this;
            }
		}
		
		// Update is called once per frame
		void Update () 
        {
		   
		}
        
        void OnGUI()
        {
            GUILayout.BeginArea(m_GUI);
            if (Network.isServer)
            {
                serverGUI();
            }
            else
            {
                clientGUI();
            }
            GUILayout.EndArea();
        }

        void clientGUI()
        {
            GUILayout.Label("Client");
            //If we currently do not have an object give the player the opprotunity to spawn one
            if (m_CurrentObject == null)
            {
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
            }
            //If we click spawn.
            //Make an RPC call to spawn t
            if (GUILayout.Button("Spawn"))
            {
                networkView.RPC("requestSpawn", RPCMode.Server, Network.player, C_NetServerManager.username);
            }
            GUI.enabled = true;
            m_HandleID = Utils.editorIntField("Handle ID: ", m_HandleID);
            if (GUILayout.Button("Despawn"))
            {
                if(m_CurrentObject != null)
                {
                    networkView.RPC("requestDespawn", RPCMode.Server, Network.player, C_NetServerManager.username, m_HandleID);
                }
            }

            GUILayout.Label("---------------------");

        }
        void serverGUI()
        {
            GUILayout.Label("Server");
            GUILayout.Label("---------------------");
        }

        public void onObjectSpawn(C_Object aObject)
        {
            if (aObject.handle.networkPlayer == Network.player)
            {

            }
        }


        public void onObjectDespawn(NetworkPlayer aPlayer)
        {
            m_CurrentObject = null;
        }
        
	}

}