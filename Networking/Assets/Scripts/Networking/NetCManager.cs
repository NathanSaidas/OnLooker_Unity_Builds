using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class NetCManager : MonoBehaviour {

        void OnConnectedToServer()
        {
            //Debug.LogError("Disabling message queue!");
            //Network.isMessageQueueRunning = false;
            //Application.LoadLevel(NetManager.levelName);
        }
        void OnLevelWasLoaded(int level)
        {

            if (level != 0 && Network.isClient)
            {
                Network.isMessageQueueRunning = true;
                Debug.Log("Loaded and requesting spawn");
                //Request a spawn from the server
                networkView.RPC("requestSpawn", RPCMode.Server, Network.player);
            }
            else
            {
                Debug.Log("Server Loade");
            }
            
        }
        
	}

}