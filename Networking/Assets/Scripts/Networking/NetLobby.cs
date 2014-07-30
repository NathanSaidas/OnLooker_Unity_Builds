using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OnLooker
{


    public enum MenuMode
    {
        START,
        CLIENT,
        SERVER
    }

    [ExecuteInEditMode()]
	public class NetLobby : MonoBehaviour 
    {
        [SerializeField()]
        private MenuMode m_MenuMode = MenuMode.START;
        [SerializeField()]
        private Rect m_MainGUI = new Rect(Screen.width * 0.5f - 50.0f, Screen.height * 0.5f - 50.0f, 100.0f, 100.0f);

        private HostData[] m_HostList;
		// Use this for initialization
		void Start () 
        {
		    
		}

        #region NetCallbacks
        //Client
        //When connected to server
        //Client and Server
        void OnDisconnectedFromServer(NetworkDisconnection aInfo)
        {
            if(Network.isServer)
            {
                Debug.Log("Local server connection disconnected");
            }
            else
            {
                if(aInfo == NetworkDisconnection.LostConnection)
                {
                    Debug.Log("Lost Connection");
                }
                else
                {
                    Debug.Log("Successfully disconnected from the server");
                }
            }

        }
        //Client
        //When connection failed for some reason
        void OnFailedToConnect(NetworkConnectionError aError)
        {
            Debug.Log(aError);
        }
        //Client and Server
        //After a Network.Instantiate call
        void OnNetworkInstantiate(NetworkMessageInfo aInfo)
        {

        }
        void OnPlayerConnected(NetworkPlayer aPlayer)
        {

        }
        void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {

        }
        void OnServerInitialized()
        {
            Application.LoadLevel("World");
            Debug.Log("Server Initialized");
        }
        void OnMasterServerEvent(MasterServerEvent aEvent)
        {
            if (aEvent == MasterServerEvent.RegistrationSucceeded)
            {
                Debug.Log("Registered");
            }
            else
            {
                Debug.Log(aEvent);
            }
        }
        #endregion

        void OnGUI()
        {
            m_MainGUI.x = Screen.width * 0.5f - m_MainGUI.width * 0.5f;
            m_MainGUI.y = Screen.height * 0.5f - m_MainGUI.height * 0.5f;

            switch (m_MenuMode)
            {
                case MenuMode.START:
                    menuStart();
                    break;
                case MenuMode.CLIENT:
                    menuClient();
                    break;
                case MenuMode.SERVER:
                    menuServer();
                    break;
            }
            
        }

        void menuStart()
        {
            GUILayout.BeginArea(m_MainGUI);
            C_NetServerManager.username = Utils.editorTextfield("Username:", C_NetServerManager.username,80.0f);
            if(GUILayout.Button("Start Game"))
            {
                m_MenuMode = MenuMode.SERVER;
            }
            if (C_NetServerManager.username == string.Empty)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }
            
            if (GUILayout.Button("Join Game"))
            {
                m_MenuMode = MenuMode.CLIENT;
            }

            GUI.enabled = true;
            GUILayout.EndArea();
        }
        void menuClient()
        {
            if (GUI.Button(new Rect(0.0f, 0.0f, 100.0f, 45.0f), "Back"))
            {
                m_MenuMode = MenuMode.START;
                return;
            }
            GUILayout.BeginArea(m_MainGUI);
            GUILayout.Label("Client");
            if (GUILayout.Button("Refresh List"))
            {
                //TODO: Refresh Host List
                StartCoroutine(refreshHostList());
            }
            if (m_HostList != null)
            {
                for(int i = 0; i < m_HostList.Length; i++)
                {
                    if (Utils.editorButton(m_HostList[i].gameName, "Connect"))
                    {
                        Debug.Log("Connecting to: " + m_HostList[i].gameName);
                        Network.Connect(m_HostList[i]);
                        break;
                    }
                }
            }
            GUILayout.EndArea();
        }
        void menuServer()
        {
            if (GUI.Button(new Rect(0.0f, 0.0f, 100.0f, 45.0f), "Back"))
            {
                m_MenuMode = MenuMode.START;
                return;
            }
            GUILayout.BeginArea(m_MainGUI);
            GUILayout.Label("Server");
            if (GUILayout.Button("Start"))
            {
                startServer();
            }
            GUILayout.EndArea();
        }

        public void startServer()
        {
            //Network.InitializeServer(16, NetServer.DEFAULT_PORT,false);
            Network.InitializeServer(16, 25002, false);
            MasterServer.RegisterHost(NetServer.GAME_TYPE, "OnLooker Server", "Test Server");
        }

        public IEnumerator refreshHostList()
        {
            Debug.Log("Refreshing...");
            MasterServer.RequestHostList(NetServer.GAME_TYPE);
        
            float timeEnd = Time.time + 3.0f;
            while (Time.time < timeEnd)
            {
                m_HostList = MasterServer.PollHostList();
                yield return new WaitForEndOfFrame();
            }
        
            if (m_HostList == null || m_HostList.Length == 0)
            {
                Debug.Log("No Server");
            }
            else
            {
                Debug.Log("Server Found");
            }
        }
	}

}