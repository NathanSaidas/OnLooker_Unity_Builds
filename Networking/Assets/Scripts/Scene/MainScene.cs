using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    [ExecuteInEditMode()]
	public class MainScene : MonoBehaviour 
    {

        private enum MenuState
        {
            START,
            CREATE_GAME,
            JOIN_GAME
        }
        
        [SerializeField()]
        private MenuState m_MenuState;
        [SerializeField()]
        private bool m_LoggedIn = false;
        [SerializeField()]
        private string m_Username;
        [SerializeField()]
        private string m_ServerName;
        [SerializeField()]
        private int m_ConnectCount;

        [SerializeField()]
        private Rect m_LayoutArea;
        [SerializeField()]
        private Rect m_ServerListLabel;
        [SerializeField()]
        private Rect m_RefreshButton;

        [SerializeField()]
        private Vector2 m_ServerInfoDisplayPos;
        [SerializeField()]
        private float m_RowHeight;
        [SerializeField()]
        private float m_Column_0_Width;
        [SerializeField()]
        private float m_Column_1_Width;
		// Use this for initialization
		void Start () 
        {
		
		}
		
		// Update is called once per frame
		void Update () 
        {
		
		}

        private void OnLevelWasLoaded(int aLevel)
        {
            if (aLevel != LevelManager.LEVEL_APPLICATION_MAIN)
            {
                Destroy(this);
            }
            Debug.Log(aLevel);
        }

        void OnGUI()
        {
            switch (m_MenuState)
            {
                case MenuState.START:
                    startMenu();
                    break;

                case MenuState.JOIN_GAME:
                    joinGameMenu();
                    break;

                case MenuState.CREATE_GAME:
                    createGameMenu();
                    break;
            }
        }

        private void startMenu()
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2.0f - 50.0f, Screen.height / 2.0f - 100.0f, 100.0f, 100.0f));
            if (GUILayout.Button("Start Server"))
            {
                m_MenuState = MenuState.CREATE_GAME;
            }
            if (GUILayout.Button("Join Server"))
            {
                m_MenuState = MenuState.JOIN_GAME;
            }
            GUILayout.EndArea();
        }
        private void joinGameMenu()
        {
            if (GUI.Button(new Rect(0.0f, 0.0f, 100.0f, 30.0f), "Back"))
            {
                m_MenuState = MenuState.START;
                return;
            }

            if (m_LoggedIn == false)
            {
                drawLogin();
                return;
            }
            if (m_Username == string.Empty && Application.isPlaying)
            {
                m_LoggedIn = false;
                return;
            }

            Rect rect = m_LayoutArea;
            
            //Server Label
            rect.x += m_ServerListLabel.x;
            rect.y += m_ServerListLabel.y;
            rect.width = m_ServerListLabel.width;
            rect.height = m_ServerListLabel.height;
            GUI.Label(rect, "Server List");
            //Server Refresh Button
            rect = m_LayoutArea;
            rect.x += m_RefreshButton.x;
            rect.y += m_RefreshButton.y;
            rect.width = m_RefreshButton.width;
            rect.height = m_RefreshButton.height;
            if (GUI.Button(rect, "Refresh"))
            {
                NetworkClient.instance.refreshServerList();
            }
            //Draw Server
            Vector2 cursor = m_ServerInfoDisplayPos;
            cursor.x += m_LayoutArea.x;
            cursor.y += m_LayoutArea.y;

            HostData[] hostData = NetworkClient.instance.hostList;
            if (hostData == null || hostData.Length == 0)
            {
                return;
            }
            HostData connectHost = null;
            for (int i = 0; i < hostData.Length; i++)
            {
                rect.x = cursor.x;
                rect.y = cursor.y + m_RowHeight * i;
                rect.width = m_Column_0_Width;
                rect.height = m_RowHeight;
                GUI.Label(rect, hostData[i].gameName);
                rect.x += m_Column_0_Width;
                rect.width = m_Column_1_Width;
                if (GUI.Button(rect, "Connect"))
                {
                    connectHost = hostData[i];
                }
            }

            if (connectHost != null)
            {
                NetworkClient.instance.connectToServer(connectHost);
            }

        }

        private void drawLogin()
        {
            float width = 350.0f;
            float height = 150.0f;
            int elements = 1;

            GUILayout.BeginArea(new Rect(Screen.width / 2.0f - width * 0.5f, Screen.height / 2.0f - height * 0.5f, width, height));
            GUILayout.Label("Enter Login:");
            m_Username = Utils.editorTextfield("Name:", m_Username);
            if (m_Username == string.Empty)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button("Login"))
            {
                m_LoggedIn = true;
            }

            GUI.enabled = true;
            GUILayout.EndArea();
        }

        private void createGameMenu()
        {
            if (GUI.Button(new Rect(0.0f, 0.0f, 100.0f, 30.0f), "Back"))
            {
                m_MenuState = MenuState.START;
                return;
            }

            float width = 200.0f;
            float height = 100.0f;
            int elements = 4;

            GUILayout.BeginArea(new Rect(Screen.width / 2.0f - width * 0.5f, Screen.height / 2.0f - height * 0.5f * elements, width, height));
            GUILayout.Label("Enter Server Info:");
            m_ServerName = Utils.editorTextfield("Name:", m_ServerName);
            m_ConnectCount = Utils.editorIntField("Number of Players:", m_ConnectCount);
            if (m_ServerName == string.Empty || m_ConnectCount <= 0 || m_ConnectCount > 128)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button("Start Server"))
            {
                NetworkServer.instance.startServer(m_ServerName, m_ConnectCount, NetworkServer.DEFAULT_UNITY_PORT);
            }

            GUI.enabled = true;
            GUILayout.EndArea();
        }
	}

}