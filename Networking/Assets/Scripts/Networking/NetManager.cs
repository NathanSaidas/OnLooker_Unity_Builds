using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    public class NetworkGroup
    {
        public const int DEFAULT = 0;
        public const int PLAYER = 1;
        public const int SERVER = 2;
    }

	public class NetManager : MonoBehaviour {

        public GameObject player;
        public static string levelName = "World";

        private List<NetClientInput> playerTracker = new List<NetClientInput>();
        private List<NetworkPlayer> schedueledSpawns = new List<NetworkPlayer>();

        private bool processSpawnRequest = false;

        private void OnPlayerConnected(NetworkPlayer aPlayer)
        {
            schedueledSpawns.Add(aPlayer);
            processSpawnRequest = true;
        }


        [RPC]
        public void requestSpawn(NetworkPlayer aSender)
        {
            if (Network.isClient)
            {
                Debug.LogError("Client tried to spawn itself! Revise logic");
                return;
            }
            if (!processSpawnRequest)
            {
                return;
            }

            foreach (NetworkPlayer spawn in schedueledSpawns)
            {
                Debug.Log("Checking player " + spawn.guid);
                if (spawn == aSender)
                {
                    int num = 0;
                    if (int.TryParse(spawn + "", out num))
                    {

                    }
                    else
                    {
                        //Error
                    }

                    GameObject handle = (GameObject)Network.Instantiate(player, transform.position, Quaternion.identity, NetworkGroup.PLAYER);
                    NetClientInput sc = handle.GetComponent<NetClientInput>();
                    if (sc == null)
                    {
                        Debug.LogError("The prefab has no C_PlayerManager attached!");
                    }

                    playerTracker.Add(sc);
                    NetworkView netview = handle.GetComponent<NetworkView>();
                    netview.RPC("setOwner", RPCMode.AllBuffered, spawn);
                }
            }

            schedueledSpawns.Remove(aSender);
            if (schedueledSpawns.Count == 0)
            {
                Debug.LogError("Spawns empty");

                processSpawnRequest = false;
            }
        }

        void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {
            Debug.Log("Player: " + aPlayer.guid + " disconnected.");
            NetClientInput found = null;
            foreach (NetClientInput man in playerTracker)
            {
                if (man.getOwner() == aPlayer)
                {
                    Network.RemoveRPCs(man.gameObject.networkView.viewID);
                    Network.Destroy(man.gameObject);
                }
            }

            if (found != null)
            {
                playerTracker.Remove(found);
            }
        }

	}

}