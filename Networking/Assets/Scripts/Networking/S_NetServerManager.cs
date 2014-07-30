using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class S_NetServerManager : MonoBehaviour 
    {
        //The prefab of the player spawned object
        public GameObject m_PlayerPrefab;

        [SerializeField()]
        private List<C_NetPlayer> m_CurrentPlayers = new List<C_NetPlayer>();
        [SerializeField()]
        private List<NetworkPlayer> m_QueuedPlayers = new List<NetworkPlayer>();

        private bool m_ProcessQueue = false;

        [RPC]
        public void registerPlayer(NetworkPlayer aSender, string aUsername)
        {
            if (Network.isClient)
            {
                return;
            }
            if (m_ProcessQueue == false)
            {
                return;
            }
            if (m_QueuedPlayers.Count == 0)
            {
                Debug.LogError("No players to process");
                return;
            }

            for (int i = 0; i < m_QueuedPlayers.Count; i++)
            {
                if (m_QueuedPlayers[i] == null)
                {
                    continue;
                }
                Debug.Log("Processing player request for " + m_QueuedPlayers[i].guid);
                if (m_QueuedPlayers[i] == aSender)
                {
                    GameObject playerBase = (GameObject)Network.Instantiate(m_PlayerPrefab, transform.position, Quaternion.identity, NetworkGroup.PLAYER);
                    C_NetPlayer playerComponent = playerBase.GetComponent<C_NetPlayer>();
                    if (playerComponent == null)
                    {
                        Debug.LogError("The prefab has no C_NetPlayer attached! Failed to register player");
                        
                    }
                    m_CurrentPlayers.Add(playerComponent);
                    NetworkView netView = playerBase.GetComponent<NetworkView>();
                    netView.RPC("setOwner", RPCMode.AllBuffered, m_QueuedPlayers[i], aUsername);
                }
            }

            m_QueuedPlayers.Remove(aSender);
            if (m_QueuedPlayers.Count == 0)
            {
                Debug.Log("Processed spawn requests");
                m_ProcessQueue = false;
            }
        }

        private void OnPlayerConnected(NetworkPlayer aPlayer)
        {
            m_QueuedPlayers.Add(aPlayer);
            m_ProcessQueue = true;

            if (Network.isClient)
            {
                Debug.LogWarning("Client called - OnPlayerConnected");
            }
            Debug.Log("Player Connected: " + aPlayer.guid);
        }
        private void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {
            Debug.Log("Player Disconnected: " + aPlayer.guid);

            for (int i = 0; i < m_CurrentPlayers.Count; i++)
            {
                if (m_CurrentPlayers[i].getOwner() == aPlayer)
                {
                    //Clean Up
                    //Invoke Clean Up Function
                    Network.RemoveRPCs(m_CurrentPlayers[i].gameObject.networkView.viewID);
                    Network.Destroy(m_CurrentPlayers[i].gameObject);
                }
            }

            if (Network.isClient)
            {
                Debug.LogWarning("Client called - OnPlayerDisconnected");
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(transform.position + m_SpawnPosition, 1.0f);
        }
	}

}