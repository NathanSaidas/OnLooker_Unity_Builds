using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace OnLooker
{

    public struct NetworkServerInfo
    {

        private int m_MaxConnections;
        private int m_PortNumber;
        private string m_ServerName;
        private string m_ServerComment;


        public int maxConnections
        {
            get { return m_MaxConnections; }
            set { m_MaxConnections = value; }
        }
        public int portNumber
        {
            get { return m_PortNumber; }
            set { m_PortNumber = value; }
        }
        public string serverName
        {
            get { return m_ServerName; }
            set { m_ServerName = value; }
        }
        public string serverComment
        {
            get { return m_ServerComment; }
            set { m_ServerComment = value; }
        }
    }

    public delegate void NetworkCallback();
    public delegate void NetworkSceneLoaded(int aIndex);
    public delegate void NetworkDisconnectCallback(NetworkDisconnection aInfo);
    public delegate void NetworkUserConnectedCallback(NetworkPlayer aPlayer);
    public delegate void NetworkConnectionCallback(NetworkPlayer aPlayer, string aUsername);
    public delegate void NetworkStateChangeCallback(int aHandle, string aSender);
    public delegate void NetworkInstantiationCallback(NetworkPlayer aOwner, string aUsername, int aHandle);

    public class NetworkServerProcess
    {
        public const int PROCESS_LOGIN_QUEUE = 1;
        public const int PROCESS_DISCONNECTED_PLAYERS = 2;
    }

    //The the network server is a singleton used to store server data
    [System.Serializable()]
	public class NetworkServer
    {
        public const string GAME_TYPE = "OnLookerGameType";
        public const int DEFAULT_UNITY_PORT = 25002;


        static private NetworkServer s_Instance;
        static public NetworkServer instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new NetworkServer();
                } 
                return s_Instance;
            }
        }

        public NetworkServer()
        {
            
        }

        //This is the component for which initially instantiates the server instance
        //This component will also be used to make rpc calls and such.
        private NetworkManager m_ManagerComponent = null;
        private NetworkServerInfo m_ServerInfo;
        //The list of data
        private List<NetworkComponent> m_Data = new List<NetworkComponent>();
        private List<NetworkUser> m_Players = new List<NetworkUser>();
        private UniqueNumberGenerator m_PlayerNumberGen = new UniqueNumberGenerator();
        private List<NetworkPlayer> m_LoginQueue = new List<NetworkPlayer>();

        private UniqueNumberGenerator m_GameObjectGen = new UniqueNumberGenerator(1);
        private List<NetworkComponent> m_GameObjects = new List<NetworkComponent>();
        private int m_NetworkState = 0;
        //List of Hosts for Connectoins
        private HostData[] m_HostList;
        

        //Start a server
        public void startServer(string aServerName,int aMaxConnections, int aPortNumber )
        {
            if (m_ManagerComponent == null)
            {
                Debug.Log("Missing NetworkManager in the Scene");
                return;
            }
            //Initialize Server Info
            m_ServerInfo = new NetworkServerInfo();
            m_ServerInfo.maxConnections = aMaxConnections;
            m_ServerInfo.portNumber = aPortNumber;
            m_ServerInfo.serverName = aServerName;
            m_ServerInfo.serverComment = string.Empty;
            //Attempt to Connect to the server
            Network.InitializeServer(aMaxConnections, aPortNumber, false);
            MasterServer.RegisterHost(GAME_TYPE, aServerName);
        }
        public void startServer(string aServerName, string aServerComment, int aMaxConnections, int aPortNumber)
        {
            if (m_ManagerComponent == null)
            {
                Debug.Log("Missing NetworkManager in the Scene");
                return;
            }
            //Initialize Server Info
            m_ServerInfo = new NetworkServerInfo();
            m_ServerInfo.maxConnections = aMaxConnections;
            m_ServerInfo.portNumber = aPortNumber;
            m_ServerInfo.serverName = aServerName;
            m_ServerInfo.serverComment = aServerComment;
            //Attempt to Connect to the server
            Network.InitializeServer(aMaxConnections, aPortNumber, false);
            MasterServer.RegisterHost(GAME_TYPE, aServerName,aServerComment);
        }
        public void startServer(string aServerName, int aMaxConnections, int aPortNumber, string aServerPassword)
        {
            if (m_ManagerComponent == null)
            {
                Debug.Log("Missing NetworkManager in the Scene");
                return;
            }
            if (aServerPassword == string.Empty)
            {
                startServer(aServerName, aMaxConnections, aPortNumber);
                Debug.LogWarning("Attempted to start server with an empty password field");
                return;
            }
            //Initialize Server Info
            m_ServerInfo = new NetworkServerInfo();
            m_ServerInfo.maxConnections = aMaxConnections;
            m_ServerInfo.portNumber = aPortNumber;
            m_ServerInfo.serverName = aServerName;
            m_ServerInfo.serverComment = string.Empty;
            //Attempt to Connect to the server
            Network.incomingPassword = aServerPassword;
            Network.InitializeServer(aMaxConnections, aPortNumber, false);
            MasterServer.RegisterHost(GAME_TYPE, aServerName);
        }
        public void startServer(string aServerName, string aServerComment, int aMaxConnections, int aPortNumber, string aServerPassword)
        {
            if (m_ManagerComponent == null)
            {
                Debug.Log("Missing NetworkManager in the Scene");
                return;
            }
            if (aServerPassword == string.Empty)
            {
                startServer(aServerName, aServerComment, aMaxConnections, aPortNumber);
                Debug.LogWarning("Attempted to start server with an empty password field");
                return;
            }
            //Initialize Server Info
            //Initialize Server Info
            m_ServerInfo = new NetworkServerInfo();
            m_ServerInfo.maxConnections = aMaxConnections;
            m_ServerInfo.portNumber = aPortNumber;
            m_ServerInfo.serverName = aServerName;
            m_ServerInfo.serverComment = aServerComment;
            //Attempt to Connect to the server
            Network.incomingPassword = aServerPassword;
            Network.InitializeServer(aMaxConnections, aPortNumber, false);
            MasterServer.RegisterHost(GAME_TYPE, aServerName);
        }

        public void pollServerList()
        {

            if (m_ManagerComponent != null)
            {
                m_ManagerComponent.pollServerList();
            }
            else
            {
                Debug.Log("Missing NetworkManager in the Scene");
            }
        }

        //Moved to NetworkClient
        //public void connectToServer(HostData aData)
        //{
        //    if (m_ManagerComponent == null)
        //    {
        //        Debug.Log("Missing Manager Component");
        //        return;
        //    }
        //    Network.Connect(aData);
        //}
        //Server Only Event Handlers
        private void onServerStart()
        {
            if (Network.isClient)
            {
                return;
            }

            //Create a player ranked as a server priority
            NetworkUser serverUser = new NetworkUser();
            serverUser.priority = NetworkPriority.SERVER;
            NetworkHandle handle = serverUser.handle;
            handle.username = "[Admin]Server";
            handle.id = 0; //Player 0
            handle.networkPlayer = Network.player;
            serverUser.handle = handle;
            //Add the user to the list of current players
            registerPlayer(serverUser);

            
        }

        private void onPlayerConnected(NetworkPlayer aPlayer)
        {
            if (Network.isClient)
            {
                return;
            }
            m_LoginQueue.Add(aPlayer);
            m_NetworkState = m_NetworkState | NetworkServerProcess.PROCESS_LOGIN_QUEUE;
        }
        private void onPlayerDisconnected(NetworkPlayer aPlayer)
        {
            if (Network.isClient)
            {
                return;
            }
            NetworkUser user = getUser(aPlayer);
            //This is just in case someone disconnects and didnt mean to
            if (user != null)
            {
                user.isOnline = false; //This user is no longer online
                m_NetworkState = m_NetworkState | NetworkServerProcess.PROCESS_DISCONNECTED_PLAYERS;
            }
            //If they were not found online then its possible they were kicked off the server or disconnectedp properly
        }

        private void update()
        {
            if ((m_NetworkState & NetworkServerProcess.PROCESS_DISCONNECTED_PLAYERS) == NetworkServerProcess.PROCESS_DISCONNECTED_PLAYERS)
            {
                processDisconnectedPlayers();
            }
        }

        private void processDisconnectedPlayers()
        {
            for (int i = 0; i < m_Players.Count; i++)
            {
                if (m_Players[i].isOnline == false)
                {
                    unregisterPlayer(m_Players[i]);
                }
            }
            
        }

        public NetworkManager manager
        {
            get{return m_ManagerComponent;}
            set
            {
                if (m_ManagerComponent == null && value != null)
                {
                    m_ManagerComponent = value;
                    m_ManagerComponent.registerServerCallbacks(onPlayerConnected, onPlayerDisconnected);
                }
                else if(m_ManagerComponent != null && value == null)
                {
                    m_ManagerComponent = null;
                }
            }
        }



        #region helpers

        public bool isServer
        {
            get
            {
                return Network.isServer;
            }
        }
        public bool isClient
        {
            get
            {
                return Network.isClient;
            }
        }

        public Prefab[] networkPrefabs
        {
            get
            {
                if (m_ManagerComponent != null)
                {
                    return m_ManagerComponent.prefabs;
                }
                return null;
            }
        }
        public HostData[] hostList
        {
            get { return m_HostList; }
        }

        //Returns a single network component with the matching handle
        public NetworkComponent getData(int aHandle)
        {
            for (int i = 0; i < m_Data.Count; i++)
            {
                if (m_Data[i] != null && m_Data[i].handle != null && m_Data[i].handle.id == aHandle)
                {
                    return m_Data[i];
                }
            }
            return null;
        }
        //Returns an array of data belonging to a single user
        public NetworkComponent[] getData(string aUsername)
        {
            List<NetworkComponent> components = new List<NetworkComponent>();
            for (int i = 0; i < m_Data.Count; i++)
            {
                if (m_Data[i] != null && m_Data[i].handle != null && m_Data[i].handle.username == aUsername)
                {
                    components.Add(m_Data[i]);
                }
            }
            return components.ToArray();
        }
        public string getUsername(int aPlayerID)
        {
            for (int i = 0; i < m_Players.Count; i++)
            {
                if (m_Players[i].handle.id == aPlayerID)
                {
                    return m_Players[i].handle.username;
                }
            }
            return string.Empty;
        }

        public int getUserRank(int aPlayerID)
        {
            for (int i = 0; i < m_Players.Count; i++)
            {
                if (m_Players[i].handle.id == aPlayerID)
                {
                    return m_Players[i].priority;
                }
            }
            return -1;
        }
        public NetworkUser getUser(string aUsername)
        {
            for (int i = 0; i < m_Players.Count; i++)
            {
                if (m_Players[i] != null && m_Players[i].handle.username == aUsername)
                {
                    return m_Players[i];
                }
            }
            return null;
        }
        public NetworkUser getUser(NetworkPlayer aPlayer)
        {
            for (int i = 0; i < m_Players.Count; i++)
            {
                if (m_Players[i] != null && m_Players[i].handle.networkPlayer == aPlayer)
                {
                    return m_Players[i];
                }
            }
            return null;
        }

        //This will get called via
        public bool registerPlayer(NetworkUser aPlayer)
        {
            //Server side
            if (isClient || (m_NetworkState & NetworkServerProcess.PROCESS_LOGIN_QUEUE) != NetworkServerProcess.PROCESS_LOGIN_QUEUE )
            {
                return false;
            }
            if (m_PlayerNumberGen == null)
            {
                Debug.LogError("Player Unique Number Generator was found null");
                return false;
            }
            if (aPlayer == null)
            {
                Debug.LogError("Network User was found null");
                return false;
            }
            bool playerInLogin = false;
            //Is this player on in the login quue
            for (int i = 0; i < m_LoginQueue.Count; i++)
            {
                if(m_LoginQueue[i] == aPlayer.handle.networkPlayer)
                {
                    playerInLogin = true;
                    break;
                }
            }
            if (playerInLogin == false)
            {
                Debug.LogWarning("Network User was not in the login queue. Possible bypass?");
                return false;
            }

            NetworkUser matchingPlayer = null;
            //Search for the player by username
            //If this player exists already and their NetworkPlayer is not the same as before
            //It means their on a different computer.
            //Could do password check here as well
            for (int i = 0; i < m_Players.Count; i++)
            {
                if (m_Players[i] != null && m_Players[i].handle.username == aPlayer.handle.username)
                {
                    matchingPlayer = m_Players[i];
                    break;
                }
            }
            //Found a match
            if (matchingPlayer != null)
            {
                //Same Computer
                if (matchingPlayer.handle.networkPlayer == aPlayer.handle.networkPlayer)
                {
                    //Do nothing
                    matchingPlayer.isOnline = true;
                }
                //Different Computer
                else
                {
                    //If this player is online then were going to need to disconnect them and connect the other
                    if (matchingPlayer.isOnline)
                    {
                        Network.CloseConnection(matchingPlayer.handle.networkPlayer, true);
                        matchingPlayer.handle.networkPlayer = aPlayer.handle.networkPlayer;
                    }
                    else
                    {
                        matchingPlayer.handle.networkPlayer = aPlayer.handle.networkPlayer;
                        matchingPlayer.isOnline = true;
                    }
                }
                m_LoginQueue.Remove(aPlayer.handle.networkPlayer);
                for (int i = 0; i < m_GameObjects.Count; i++)
                {
                    if (m_GameObjects[i] != null)
                    {
                        m_GameObjects[i].networkView.RPC("playerConnected", RPCMode.All, matchingPlayer.handle.networkPlayer, matchingPlayer.handle.username);
                    }
                }
                return true;
            }
            //Otherwise if we werent able to find a player on the network with the same username
            //Assign an ID to the player
            aPlayer.handle.id = m_PlayerNumberGen.getUniqueNumber();
            aPlayer.isOnline = true;
            //TODO: After we make accounts possible in the framework I can make a function to get
            //this users NetworkPriority from the saved data and set it
            //For now everyone will be a admin
            aPlayer.priority = NetworkPriority.ADMIN;
            m_Players.Add(aPlayer);
            //Remove the player from the login queue since they are logged in
            m_LoginQueue.Remove(aPlayer.handle.networkPlayer);
            for (int i = 0; i < m_GameObjects.Count; i++)
            {
                if (m_GameObjects[i] != null)
                {
                    m_GameObjects[i].networkView.RPC("playerConnected", RPCMode.All, matchingPlayer.handle.networkPlayer, matchingPlayer.handle.username);
                }
            }
            return true;
        }

        //When someone is about to disconnect they'll unregister their player from the server then disconnect
        public bool unregisterPlayer(NetworkUser aPlayer)
        {
            NetworkUser user = getUser(aPlayer.handle.username);
            //If the user doesnt exist return false
            if (user == null)
            {
                return false;
            }
            //Found a matching user
            //Remove them from the list
            else
            {
                if (m_PlayerNumberGen == null)
                {
                    Debug.LogWarning("Player Unique Number Generator was found null");
                    return false;
                }
                else
                {
                    m_PlayerNumberGen.freeNumber(user.handle.id);
                }
                for (int i = 0; i < m_GameObjects.Count; i++)
                {
                    if (m_GameObjects[i] != null)
                    {
                        m_GameObjects[i].networkView.RPC("playerDisconnected", RPCMode.All,user.handle.networkPlayer, user.handle.username);
                    }
                }
                m_Players.Remove(user);
            }
            return true;
        }

        public NetworkComponent getNetworkComponent(int aHandle)
        {
            if (aHandle != 0)
            {
                for (int i = 0; i < m_GameObjects.Count; i++)
                {
                    if (m_GameObjects[i].handle.id == aHandle)
                    {
                        return m_GameObjects[i];
                    }
                }
                
            }
            return null;
        }
        
       
        #endregion

        public void objectDestroyed(int aHandle)
        {
            if (m_ManagerComponent != null)
            {
                m_ManagerComponent.networkView.RPC("onObjectDestroy", RPCMode.Server, aHandle);
            }
        }
        public void freeObject(int aHandle)
        {
            for (int i = 0; i < m_GameObjects.Count; i++)
            {
                if (m_GameObjects[i] != null)
                {
                    if (m_GameObjects[i].handle.id == aHandle)
                    {
                        m_GameObjects.Remove(m_GameObjects[i]);
                        m_GameObjectGen.freeNumber(aHandle);
                        break;
                    }
                }
            }
        }

        public void requestSpawn(NetworkPlayer aSender, int aPrefabIndex)
        {
            if (m_ManagerComponent != null)
            {
                m_ManagerComponent.requestSpawn(aSender, aPrefabIndex);
            }
        }

        public void requestDespawn(int aHandle)
        {
            if (m_ManagerComponent != null)
            {
                m_ManagerComponent.requestDespawn(aHandle);
            }
        }
        
        public void networkSpawn(NetworkPlayer aSender, int aPrefabIndex)
        {
            if(Network.isClient || m_GameObjectGen == null)
            {
                return;
            }
            Prefab[] prefabs = networkPrefabs;
            NetworkUser user = NetworkServer.instance.getUser(aSender);
            if (user == null)
            {
                Debug.Log("User not found");
                return;
            }
            if(prefabs == null)
            {
                Debug.Log("No Prefabs");
                return;
            }
            if (aPrefabIndex >= 0 && aPrefabIndex < prefabs.Length)
            {
                GameObject spawnedObj = (GameObject)Network.Instantiate(prefabs[aPrefabIndex].gameObject, Vector3.zero, Quaternion.identity, NetworkGroup.SERVER);
                NetworkComponent networkComponent = spawnedObj.GetComponent<NetworkComponent>();

                if (networkComponent != null)
                {
                    networkComponent.setOwner(user.handle.networkPlayer, user.handle.username, m_GameObjectGen.getUniqueNumber());
                    m_GameObjects.Add(networkComponent);
                    networkComponent.networkView.RPC("serverSpawned", RPCMode.AllBuffered);

                }
                else
                {
                    Debug.LogError("Game Object spawned on network but missing Network Component");
                }
            }
        }


        public void callMethod(int aHandle, string aMethodName, int aValue)
        {
            if (m_ManagerComponent != null)
            {
                m_ManagerComponent.networkView.RPC("callMethodInt", RPCMode.Server, aHandle, aMethodName, aValue);
            }
        }
        public void callMethod(int aHandle, string aMethodName, string aValue)
        {
            if (m_ManagerComponent != null)
            {
                m_ManagerComponent.networkView.RPC("callMethodString", RPCMode.Server, aHandle, aMethodName, aValue);
            }
        }
        public void callMethod(int aHandle, string aMethodName, Vector3 aValue)
        {
            if (m_ManagerComponent != null)
            {
                m_ManagerComponent.networkView.RPC("callMethodVector", RPCMode.Server, aHandle, aMethodName, aValue);
            }
        }


        public void callMethodInt(int aHandle, string aMethodName, int aValue)
        {
            if (Network.isServer)
            {
                for (int i = 0; i < m_GameObjects.Count; i++)
                {
                    if (m_GameObjects[i].handle.id == aHandle)
                    {
                        m_GameObjects[i].networkView.RPC("callMethodInt", RPCMode.All, aMethodName, aValue);
                        break;
                    }
                }
            }
        }
        public void callMethodString(int aHandle, string aMethodName, string aValue)
        {
            if (Network.isServer)
            {
                for (int i = 0; i < m_GameObjects.Count; i++)
                {
                    if (m_GameObjects[i].handle.id == aHandle)
                    {
                        m_GameObjects[i].networkView.RPC("callMethodString", RPCMode.All, aMethodName, aValue);
                        break;
                    }
                }
            }
        }
        public void callMethodVector(int aHandle, string aMethodName, Vector3 aValue)
        {
            if (Network.isServer)
            {
                for (int i = 0; i < m_GameObjects.Count; i++)
                {
                    if (m_GameObjects[i].handle.id == aHandle)
                    {
                        m_GameObjects[i].networkView.RPC("callMethodVector", RPCMode.All, aMethodName, aValue);
                        break;
                    }
                }
            }
        }


    }

}