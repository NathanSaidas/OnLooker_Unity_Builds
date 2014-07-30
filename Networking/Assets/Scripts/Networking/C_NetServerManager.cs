using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    [RequireComponent(typeof(NetworkView))]
	public class C_NetServerManager : MonoBehaviour {

        private static string s_Username = string.Empty;
        public static string username
        {
            get { return s_Username; }
            set { s_Username = value; }
        }

        void OnConnectedToServer()
        {
            Network.isMessageQueueRunning = false;
            Application.LoadLevel(LevelManager.LEVEL_WORLD);
        }

        void OnLevelWasLoaded(int aLevel)
        {
            if (aLevel == LevelManager.LEVEL_WORLD && Network.isClient)
            {
                Debug.Log("World Loaded");
                Network.isMessageQueueRunning = true;

                //Send a message to the server registering the player
                networkView.RPC("registerPlayer", RPCMode.Server, Network.player, s_Username);
            }
            else if(aLevel == LevelManager.LEVEL_WORLD && Network.isServer)
            {
                Debug.Log("Server Loaded");
            }
        }
	}

}