using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class NetworkClient
    {
        private static NetworkClient s_Instance;
        public static NetworkClient instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new NetworkClient();
                }
                return s_Instance;
            }
        }


        private NetworkManager m_Manager = null;
        private string m_Username = string.Empty;

        private HostData[] m_HostList;
        

        private void onConnectedToServer()
        {
            //Load Server Scene
            Application.LoadLevel(LevelManager.LEVEL_SERVER_START);
        }
        
        private void onSceneLoaded(int aLevel)
        {
            if (m_Manager != null && Network.isClient && aLevel == LevelManager.LEVEL_SERVER_START)
            {
                m_Manager.registerPlayer(m_Username);
            }
        }

        public void refreshServerList()
        {
            if (m_Manager == null)
            {
                Debug.Log("Missing NetworkManager in the Scene");
                return;
            }
            m_Manager.pollServerList();
        }

        public void connectToServer(HostData aData)
        {
            if (aData == null)
            {
                return;
            }
            if (m_Manager == null)
            {
                Debug.Log("Missing NetworkManager in the Scene");
                return;
            }
            Network.Connect(aData);
        }


        public HostData[] hostList
        {
            get { return m_HostList; }
        }



        public NetworkManager manager
        {

            get { return m_Manager; }
            set
            {
                if (m_Manager == null && value != null)
                {
                    m_Manager = value;
                    m_Manager.registerClientCallbacks(onConnectedToServer,onSceneLoaded);

                }
                else if (m_Manager != null && value == null)
                {
                    m_Manager = value;
                }

            }
        }
        public string username
        {
            get { return m_Username; }
            set { m_Username = value; }
        }
	}

}