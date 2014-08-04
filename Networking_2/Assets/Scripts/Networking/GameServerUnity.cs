using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class GameServerUnity : MonoBehaviour 
    {

        int m_ConnectionCount = 16;
        int m_PortNumber = 26549;
        string m_GameTypeName = "OnLooker_Game";
        string m_GameName = "Game_Server";
        [SerializeField]
        private bool m_IsServerBuild = false;

        [SerializeField]
        private string[] m_Connections;

        private void Start()
        {
            if (m_IsServerBuild == true)
            {
                Network.InitializeServer(16, 26549, false);
                MasterServer.RegisterHost(m_GameTypeName, m_GameName, "Game Server");
            }
        }

        private void OnServerInitialized()
        {
            Debug.Log("Server Initialized");
            
        }
        private void OnMasterServerEvent(MasterServerEvent aEvent)
        {
            
        }
        private void OnPlayerConnected(NetworkPlayer aPlayer)
        {

        }

        private void Update()
        {
            m_Connections = new string[Network.connections.Length];
            for (int i = 0; i < Network.connections.Length; i++)
            {
                m_Connections[i] = Network.connections[i].guid;
            }
        }

        private void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {
            Debug.Log("Player Disconnected");
            AuthenticationServer.instance.userLoggedOff(aPlayer);
        }
	}

}