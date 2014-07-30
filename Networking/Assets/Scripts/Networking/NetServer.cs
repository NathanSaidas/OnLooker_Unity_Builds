using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class NetServer : MonoBehaviour 
    {
        public const string GAME_TYPE = "OnLookerGameType";
        public const int DEFAULT_PORT = 25002;

        public const int GROUP_BASE_CLIENT = 1;

        public NetworkPeerType peerType
        {
            get { return Network.peerType; }
        }

        [SerializeField()]
        private List<C_NetPlayer> m_Player;

		// Use this for initialization
		void Start () {
		    
		}
		
		// Update is called once per frame
		void Update () {
		
		}


        void OnServerInitialized()
        {
            Debug.Log("Server Init");
        }

        void OnPlayerConnect(NetworkPlayer aPlayer)
        {
            Debug.Log("Player Connected");
            Debug.Log("External IP: " + aPlayer.externalIP);
            Debug.Log("GUID" + aPlayer.guid);
        }
        void OnPlayerDisconnect(NetworkPlayer aPlayer)
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