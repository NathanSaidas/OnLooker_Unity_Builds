using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace OnLooker
{
    [RequireComponent(typeof(NetworkView))]
	public class NetworkManager : MonoBehaviour 
    {
        [SerializeField()]
        private Prefab[] m_Prefabs;

        [SerializeField()]
        private List<string> m_GameNames;

        
        private NetworkUserConnectedCallback m_OnPlayerConnected;
        private NetworkUserConnectedCallback m_OnPlayerDisconnected;
        private NetworkCallback m_OnConnectedToServer;
        private NetworkSceneLoaded m_OnSceneLoaded;


        

        public void registerServerCallbacks(NetworkUserConnectedCallback aOnPlayerConnected, NetworkUserConnectedCallback aOnPlayerDisconnected)
        {
            m_OnPlayerConnected = aOnPlayerConnected;
            m_OnPlayerDisconnected = aOnPlayerDisconnected;
        }
        public void registerClientCallbacks(NetworkCallback aOnConnectedToServer, NetworkSceneLoaded aOnSceneLoaded)
        {
            m_OnConnectedToServer = aOnConnectedToServer;
            m_OnSceneLoaded = aOnSceneLoaded;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            //Assign a unique number to each prefab for lookup later
            for (int i = 0; i < m_Prefabs.Length; i++)
            {
                Type type = m_Prefabs[i].GetType();
                FieldInfo field =  type.GetField("m_ID", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(m_Prefabs[i], i);
                }
            }
            NetworkServer.instance.manager = this;
            NetworkClient.instance.manager = this;
        }
        private void Update()
        {

        }
        //Server Polling helper
        public void pollServerList()
        {
            m_GameNames.Clear();
            StartCoroutine(refreshServerList());
        }
        private IEnumerator refreshServerList()
        {
            //Request the Host List from the master server
            MasterServer.RequestHostList(NetworkServer.GAME_TYPE);

            //This is a hack to set a private field of class. Kind of like the friend keyword in c++.
            Type serverType = NetworkServer.instance.GetType();
            FieldInfo serverField = serverType.GetField("m_HostList", BindingFlags.NonPublic | BindingFlags.Instance);
            if (serverField == null)
            {
                Debug.LogError("m_HostList field not found. (Server)");
                yield return 0;
            }
            Type clientType = NetworkClient.instance.GetType();
            FieldInfo clientField = clientType.GetField("m_HostList", BindingFlags.NonPublic | BindingFlags.Instance);
            if (clientField == null)
            {
                Debug.LogError("m_HostList field not found. (Client)");
                yield return 0;
            }
            //Poll the list
            float timeEnd = Time.time + 3.0f;
            while (Time.time < timeEnd)
            {
                HostData[] hostData = MasterServer.PollHostList();
                serverField.SetValue(NetworkServer.instance, hostData);
                clientField.SetValue(NetworkClient.instance, hostData);
                yield return new WaitForEndOfFrame();
            }
        }
        private void OnServerInitialized()
        {
            Debug.Log("Server Started");
            Application.LoadLevel(LevelManager.LEVEL_SERVER_START);
        }

        //Server Callbacks
        //Called to Client when connected to server
        private void OnConnectedToServer()
        {
            //Load Server Scene
            //Upon finished loading, Request registration
            if (m_OnConnectedToServer != null)
            {
                m_OnConnectedToServer.Invoke();
            }
        }
        private void OnLevelWasLoaded(int aLevel)
        {
            if (m_OnSceneLoaded != null)
            {
                m_OnSceneLoaded.Invoke(aLevel);
            }
        }
        //Called on Server and Client when server is disconnected
        private void OnDisconnectedFromServer(NetworkDisconnection aInfo)
        {
            
            if (Network.isServer)
            {
                Debug.Log("Local server connection disconnected");
                //Return to Main screen
            }
            else
            {
                if (aInfo == NetworkDisconnection.LostConnection)
                {
                    Debug.Log("Lost connection to the server");
                    //Print Error
                    //Return to Main Screen
                }
                else
                {
                    Debug.Log("Successfully diconnected from the server");
                    //Return to Main Screen
                }
            }
        }
        private void OnPlayerConnected(NetworkPlayer aPlayer)
        {
            if (m_OnPlayerConnected != null)
            {
                m_OnPlayerConnected.Invoke(aPlayer);
            }
        }
        private void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {
            if (m_OnPlayerDisconnected != null)
            {
                m_OnPlayerDisconnected.Invoke(aPlayer);
            }
        }

        public void registerPlayer(string aUsername)
        {
            networkView.RPC("registerPlayer", RPCMode.Server, Network.player, aUsername);
        }

        [RPC]
        public void registerPlayer(NetworkPlayer aPlayer, string aUsername)
        {
            if (Network.isClient)
            {
                Debug.LogWarning("Client tried to register a player");
                return;
            }

            NetworkUser user = new NetworkUser();
            user.handle.networkPlayer = aPlayer;
            user.handle.username = aUsername;
            NetworkServer.instance.registerPlayer(user);
        }
        //Force a player to disconnect but first unregister them from the netowrk
        [RPC]
        public void unregisterPlayer(NetworkPlayer aPlayer, string aUsername)
        {
            if (Network.isClient)
            {
                Debug.LogWarning("Client tried to unregister a player");
                return;
            }

            NetworkUser user = new NetworkUser();
            user.handle.networkPlayer = aPlayer;
            user.handle.username = aUsername;
            if (NetworkServer.instance.unregisterPlayer(user) == false)
            {
                Debug.LogWarning("That player wasn't registered with the server!");
            }
            Network.CloseConnection(aPlayer, true);
        }

        public void requestSpawn(NetworkPlayer aSender, int aPrefabIndex)
        {
            networkView.RPC("networkSpawn", RPCMode.Server, aSender, aPrefabIndex);
        }

        [RPC]
        public void networkSpawn(NetworkPlayer aSender, int aPrefabIndex)
        {
            if (Network.isServer)
            {
                NetworkServer.instance.networkSpawn(aSender, aPrefabIndex);
            }
        }

        public void requestDespawn(int aHandle)
        {
            networkView.RPC("networkDespawn", RPCMode.Server, aHandle);
        }
        [RPC]
        public void networkDespawn(int aHandle)
        {
            if (Network.isServer)
            {
                NetworkComponent netComponent = NetworkServer.instance.getNetworkComponent(aHandle);
                if (netComponent != null)
                {
                    netComponent.networkDestroy();
                }
            }
        }

        [RPC]
        public void onObjectDestroy(int aHandle)
        {
            if (Network.isServer)
            {
                NetworkServer.instance.freeObject(aHandle);
            }
        }

        [RPC]
        public void callMethodInt(int aHandle, string aMethodName, int aValue)
        {
            NetworkServer.instance.callMethodInt(aHandle, aMethodName, aValue);
        }
        [RPC]
        public void callMethodString(int aHandle, string aMethodName, string aValue)
        {
            NetworkServer.instance.callMethodString(aHandle, aMethodName, aValue);
        }
        [RPC]
        public void callMethodVector(int aHandle, string aMethodName, Vector3 aValue)
        {
            NetworkServer.instance.callMethodVector(aHandle, aMethodName, aValue);
        }


        public Prefab[] prefabs
        {
            get { return m_Prefabs; }
        }
        
	}
}