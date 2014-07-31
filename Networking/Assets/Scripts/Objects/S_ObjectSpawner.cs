using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class S_ObjectSpawner : MonoBehaviour
    {

        [SerializeField()]
        private GameObject m_Prefab;

        private List<NetworkHandle> m_PlayersAwaitingSpawn = new List<NetworkHandle>();
        private List<NetworkHandle> m_PlayersAwaitingDespawn = new List<NetworkHandle>();
        private List<C_Object> m_SpawnedObjects = new List<C_Object>();

        private UniqueNumberGenerator m_UNG = new UniqueNumberGenerator(1);

        private bool m_ProcessSpawns = false;
        private bool m_ProcessDespawns = false;
		// Use this for initialization
		void Start () 
        {
		
		}
		
		// Update is called once per frame
		void Update () 
        {
            if (Network.isServer == false)
            {
                return; //Server Side Only
            }

            if (m_ProcessSpawns == true)
            {
                processSpawns();
            }
            if (m_ProcessDespawns == true)
            {
                processDespawns();
            }
		}

        private C_Object getObject(int aID)
        {
            for (int i = 0; i < m_SpawnedObjects.Count; i++)
            {
                if (m_SpawnedObjects[i] != null && m_SpawnedObjects[i].handle.uniqueID == aID)
                {
                    return m_SpawnedObjects[i];
                }
            }
            return null;
        }

        void processDespawns()
        {
            for (int i = 0; i < m_PlayersAwaitingDespawn.Count; i++)
            {
                if (m_PlayersAwaitingDespawn[i].uniqueID == 0)
                {
                    continue;
                }
                C_Object clientObject = getObject(m_PlayersAwaitingDespawn[i].uniqueID);
                //Destroy all RPC's attached to that game object on all clients
                clientObject.onFlagDespawn();
                Network.Destroy(clientObject.gameObject);

                //Free Up Unique Number
                m_UNG.freeNumber(m_PlayersAwaitingDespawn[i].uniqueID);
                m_PlayersAwaitingDespawn[i] = null;
            }

            //Cleanup List
            for (int i = m_PlayersAwaitingDespawn.Count; i >= 0; i--)
            {
                if (m_PlayersAwaitingDespawn[i] == null)
                {
                    m_PlayersAwaitingDespawn.RemoveAt(i);
                }
            }



            if (m_PlayersAwaitingDespawn.Count == 0)
            {
                Debug.Log("Processing Despawns Complete");
                m_ProcessDespawns = false;
            }
        }

        void processSpawns()
        {
            for (int i = 0; i < m_PlayersAwaitingSpawn.Count; i++)
            {
                GameObject go = (GameObject)Network.Instantiate(m_Prefab, transform.position, Quaternion.identity, NetworkGroup.SERVER);
                if (go == null)
                {
                    continue;
                }
                //Get the handle and set it
                C_Object clientObject = go.GetComponent<C_Object>();
                if (clientObject != null)
                {
                    clientObject.onSpawn(m_PlayersAwaitingSpawn[i].networkPlayer, m_PlayersAwaitingSpawn[i].username, m_UNG.getUniqueNumber(), this);
                }
                else
                {
                    Debug.Log("Prefab is missing C_Object component");
                }
                m_PlayersAwaitingSpawn[i] = null;
                m_SpawnedObjects.Add(clientObject);
            }

            //Clean up list
            for (int i = m_PlayersAwaitingSpawn.Count; i >= 0; i--)
            {
                if (m_PlayersAwaitingSpawn[i] == null)
                {
                    m_PlayersAwaitingSpawn.RemoveAt(i);
                }
            }


            if (m_PlayersAwaitingSpawn.Count == 0)
            {
                Debug.Log("Processing spawns complete");
                m_ProcessSpawns = false;
            }
        }


        //Players can request a spawn of an object.
        [RPC]
        public void requestSpawn(NetworkPlayer aPlayer, string aUsername)
        {

            if (Network.isServer)
            {
                S_NetServerManager serverManager = S_NetServerManager.instance;
                if (serverManager == null)
                {
                    //Server manager not present.
                    Debug.LogError("Server Manager not present in scene");
                    return;
                }

                C_NetPlayer player = serverManager.getPlayer(aUsername);
                if (player == null)
                {
                    Debug.LogError("Player with that name does not exist on the server!");
                    return;
                }

                NetworkHandle spawnHandle = new NetworkHandle();
                spawnHandle.networkPlayer = player.owner;
                spawnHandle.username = player.username;
                spawnHandle.uniqueID = 0;
                spawnHandle.server = this;

                m_PlayersAwaitingSpawn.Add(spawnHandle);
                m_ProcessSpawns = true;
            }
        }

        [RPC]
        public void requestDespawn(NetworkPlayer aPlayer, string aUsername, int aID)
        {
            if (Network.isServer)
            {
                S_NetServerManager serverManager = S_NetServerManager.instance;
                if (serverManager == null)
                {
                    //Server manager not present.
                    Debug.LogError("Server Manager not present in scene");
                    return;
                }

                C_NetPlayer player = serverManager.getPlayer(aUsername);
                if (player == null)
                {
                    Debug.LogError("Player with that name does not exist on the server!");
                    return;
                }

                NetworkHandle spawnHandle = new NetworkHandle();
                spawnHandle.networkPlayer = player.owner;
                spawnHandle.username = player.username;
                spawnHandle.uniqueID = aID;
                spawnHandle.server = this;

                m_PlayersAwaitingDespawn.Add(spawnHandle);
                m_ProcessDespawns = true;

            }
        }
	}

}