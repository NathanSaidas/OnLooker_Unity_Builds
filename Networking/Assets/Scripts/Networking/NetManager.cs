using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    

	public class NetManager : MonoBehaviour {

        public GameObject player;
        public static string levelName = "World";

        [SerializeField()]
        private List<NetClientInput> playerTracker = new List<NetClientInput>();
        [SerializeField()]
        private List<NetworkPlayer> schedueledSpawns = new List<NetworkPlayer>();

        private bool processSpawnRequest = false;

        


        [RPC]
        public void requestSpawn(NetworkPlayer aSender)
        {
            if (Network.isClient)
            {
                Debug.LogError("Client tried to spawn itself! Revise logic");
                return;
            }
            //If we arent processing spawn requests leave (Player Connected)
            if (processSpawnRequest == false)
            {
                return;
            }

            foreach (NetworkPlayer spawn in schedueledSpawns)
            {
                Debug.Log("Checking player " + spawn.guid);
                if (spawn == aSender)
                {
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
                Debug.LogError("Spawns empy");

                processSpawnRequest = false;
            }
        }

        //Everytime a player connects, Add their NetworkPlayer (ID) to the list of schedueled spawns.
        //Set the processSpawnRequest flag to true
        //Gets called on Server Only
        private void OnPlayerConnected(NetworkPlayer aPlayer)
        {
            schedueledSpawns.Add(aPlayer);
            processSpawnRequest = true;

            if (Network.isClient)
            {
                Debug.LogWarning("Client Called - OnPlayerConnected");
            }
        }
        //Everytime a player disconnects, Go through all of the NetClientInput's in playerTracker List
        //Remove the RPC's and Destroy objects that belong to the matching owner of the NetClientInput with disconnecting player.
        private void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {
            Debug.Log("Player: " + aPlayer.guid + " disconnected.");
            NetClientInput found = null;

            foreach (NetClientInput man in playerTracker)
            {
                if (man.getOwner() == aPlayer)
                {
                    //Clean Up
                    Network.RemoveRPCs(man.gameObject.networkView.viewID);
                    Network.Destroy(man.gameObject);
                }
            }

            if (found != null)
            {
                playerTracker.Remove(found);
            }
            else
            {

            }

            if (Network.isClient)
            {
                Debug.LogWarning("Client Called - OnPlayerConnected");
            }
        }

	}

}