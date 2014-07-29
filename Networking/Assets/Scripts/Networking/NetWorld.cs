using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class NetWorld : MonoBehaviour {

        [SerializeField()]
        private Camera m_Camera;
        [SerializeField()]
        private Transform m_Spawn;
        [SerializeField()]
        private GameObject m_PlayerPrefab;

        void OnLevelWasLoaded(int level)
        {
            if (level == 1)
            {
                Debug.Log("Loading World Objects");
                if (Network.peerType == NetworkPeerType.Client)
                {
                    GameObject player = (GameObject)Network.Instantiate(m_PlayerPrefab, m_Spawn.position, m_Spawn.rotation, 0);
                    player.name = player.name.Replace("(Clone)", "(Client)");

                    m_Camera.transform.position = player.transform.position;
                    m_Camera.transform.rotation = player.transform.rotation;
                    m_Camera.transform.parent = player.transform;
                    m_Camera.transform.Translate(0.0f, 2.0f, -3.0f);
                    m_Camera.transform.LookAt(player.transform);

                    player.GetComponent<PlayerControl>().cam = m_Camera;
                }
                else if(Network.peerType == NetworkPeerType.Server)
                {
                    GameObject player = (GameObject)Network.Instantiate(m_PlayerPrefab, m_Spawn.position, m_Spawn.rotation, 0);
                    player.name = player.name.Replace("(Clone)", "(Server)");

                    m_Camera.transform.position = player.transform.position;
                    m_Camera.transform.rotation = player.transform.rotation;
                    m_Camera.transform.parent = player.transform;
                    m_Camera.transform.Translate(0.0f, 2.0f, -3.0f);
                    m_Camera.transform.LookAt(player.transform);
                    player.GetComponent<PlayerControl>().cam = m_Camera;
                }
            }
            else
            {
                Debug.Log("Loaded " + level.ToString());
            }
        }

        void OnPlayerConnected(NetworkPlayer aPlayer)
        {
            Debug.Log("Player Connected");
            Debug.Log("External IP: " + aPlayer.externalIP);
            Debug.Log("GUID" + aPlayer.guid);
        }
        void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {
            Debug.Log("Player Disconnected");
            Debug.Log("External IP: " + aPlayer.externalIP);
            Debug.Log("GUID" + aPlayer.guid);
        }
        void OnNetworkInstantiate(NetworkMessageInfo aInfo)
        {
            Debug.Log("Instantiation");
            Debug.Log("External IP: " + aInfo.sender.externalIP);
            Debug.Log("GUID" + aInfo.sender.guid);
        }
	}

}