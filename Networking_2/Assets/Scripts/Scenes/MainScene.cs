using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{
    [ExecuteInEditMode()]
	public class MainScene : MonoBehaviour {

        private enum EMenuState
        {
            MAIN_MENU,
            OPTIONS,
            OFFLINE,
            LOGIN,
            CREATE_ACCOUNT
        }
        [SerializeField()]
        private EMenuState m_MainState = EMenuState.MAIN_MENU;
        [SerializeField()]
        private float m_MenuWidth = 200.0f;
        [SerializeField()]
        private float m_MenuHeight = 30.0f;

        private string m_UserID = string.Empty;
        private string m_Password = string.Empty;
        private string m_Email = string.Empty;

        private bool m_LoggedIn = false;

		// Use this for initialization
		void Start () 
        {
            
		}
		
		// Update is called once per frame
		void Update () 
        {
            if (Network.isClient)
            {
                if (m_MainState == EMenuState.LOGIN)
                {
                    //Check for Login Status every frame
                    int status = AuthenticationServer.instance.getStatus(AuthenticationServerUnity.STATUS_TYPE_LOGIN);
                    
                    if(status == AuthenticationServer.ERROR_NONE)
                    {
                        if (m_LoggedIn == false)
                        {
                            m_LoggedIn = true;
                            Debug.Log("Successfully Logged In");
                        }
                        
                    }
                    else if( status > AuthenticationServer.ERROR_NONE)
                    {
                        //Error Check
                        Debug.Log("Error Logging In");
                    }
                }
                else if (m_MainState == EMenuState.CREATE_ACCOUNT)
                {
                    int status = AuthenticationServer.instance.getStatus(AuthenticationServerUnity.STATUS_TYPE_REGISTER_SERVER);
                    if(status ==  AuthenticationServer.ERROR_NONE)
                    {
                        m_MainState = EMenuState.LOGIN;
                        Debug.Log("Created an account");
                    }
                    else if(status > AuthenticationServer.ERROR_NONE)
                    {
                        Debug.Log("Error creating account");
                    }
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    AuthenticationServer.instance.requestCall();
                    //AuthenticationServer.instance.requestCall(new string[] {"A Key Pressed"});
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    //AuthenticationServer.instance.requestCall(new string[] { "B Key Pressed" });
                }
            }
		}

        void OnGUI()
        {

            if (m_MainState != EMenuState.MAIN_MENU)
            {
                if (GUI.Button(new Rect(0.0f, 0.0f, 50.0f, 32.0f), "Back"))
                {
                    if (m_MainState == EMenuState.CREATE_ACCOUNT)
                    {
                        m_MainState = EMenuState.LOGIN;
                    }
                    else
                    {
                        m_MainState = EMenuState.MAIN_MENU;
                    }
                }
            }
            switch (m_MainState)
            {
                case EMenuState.MAIN_MENU:
                    mainMenuGUI();
                    break;
                case EMenuState.OPTIONS:
                    optionsGUI();
                    break;
                case EMenuState.OFFLINE:
                    offlineGUI();
                    break;
                case EMenuState.LOGIN:
                    loginGUI();
                    break;
                case EMenuState.CREATE_ACCOUNT:
                    createAccountGUI();
                    break;
            }
        }

        void mainMenuGUI()
        {
            int elements = 4;
            float height = m_MenuHeight * elements;
            Rect rect = new Rect(Screen.width * 0.5f - m_MenuWidth * 0.5f, Screen.height * 0.5f - height * 0.5f, m_MenuWidth, height);
            
            GUILayout.BeginArea(rect);
            {
                GUILayout.Label("Main Menu");
                if (Network.isClient == false)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Login"))
                {
                    m_MainState = EMenuState.LOGIN;
                }

                GUI.enabled = true;
                if (GUILayout.Button("Offline Mode"))
                {
                    //m_MainState = EMenuState.OFFLINE;
                }
                if(GUILayout.Button("Options"))
                {
                    //m_MainState = EMenuState.OPTIONS;
                }
            }
            GUILayout.EndArea();
        }

        void optionsGUI()
        {

        }
        void offlineGUI()
        {

        }
        void loginGUI()
        {
            if (Network.isClient == false)
            {
                m_MainState = EMenuState.MAIN_MENU;
                return;
            }
            int elements = 4;
            float height = m_MenuHeight * elements;
            Rect rect = new Rect(Screen.width * 0.5f - m_MenuWidth * 0.5f, Screen.height * 0.5f - height * 0.5f, m_MenuWidth, height);
            GUILayout.BeginArea(rect);
            {
                GUILayout.Label("Enter Login");
                //Email or Username
                m_UserID = Utils.editorTextfield("User ID", m_UserID);
                m_Password = Utils.editorTextfield("Password", m_Password);

                if (GUILayout.Button("Login"))
                {
                    int loginSuccess = -1;
                    //Try Login
                    if (m_UserID.Contains(".com") || m_UserID.Contains(".ca"))
                    {
                        loginSuccess = AuthenticationServer.instance.loginEmail(m_UserID, m_Password);
                    }
                    else
                    {
                        if (m_UserID.Contains("@") == false)
                        {
                            loginSuccess = AuthenticationServer.instance.login(m_UserID, m_Password);
                        }
                    }
                    if (loginSuccess == AuthenticationServer.ERROR_NONE)
                    {

                    }
                    else if (loginSuccess == AuthenticationServer.STATUS_PENDING)
                    {
                        Debug.Log("Status Pending");

                    }
                    else
                    {
                        //HANDLE ERROR
                        Debug.Log("Error Loging in");
                    }
                }
                if (GUILayout.Button("Register Account"))
                {
                    m_MainState = EMenuState.CREATE_ACCOUNT;
                    m_UserID = string.Empty;
                    m_Password = string.Empty;
                    m_Email = string.Empty;
                }
            }
            GUILayout.EndArea();
        }
        void createAccountGUI()
        {
            if (Network.isClient == false)
            {
                m_MainState = EMenuState.MAIN_MENU;
                return;
            }
            int elements = 5;
            float height = m_MenuHeight * elements;
            Rect rect = new Rect(Screen.width * 0.5f - m_MenuWidth * 0.5f, Screen.height * 0.5f - height * 0.5f, m_MenuWidth, height);
            

            GUILayout.BeginArea(rect);
            {
                GUILayout.Label("Enter Account Info");
                //Email or Username
                m_UserID = Utils.editorTextfield("Username", m_UserID);
                m_Password = Utils.editorTextfield("Password", m_Password);
                m_Email = Utils.editorTextfield("Email", m_Email);


                if (GUILayout.Button("Create"))
                {
                    int loginSuccess = -1;
                    //Try Login
                    loginSuccess = AuthenticationServer.instance.newUser(m_UserID, m_Password, m_Email);

                    if (loginSuccess == AuthenticationServer.ERROR_NONE)
                    {
                        m_MainState = EMenuState.LOGIN;
                        m_Password = string.Empty;
                    }
                    else if (loginSuccess == AuthenticationServer.STATUS_PENDING)
                    {
                        Debug.Log("Status Pending");
                    }
                    else
                    {
                        //HANDLE ERROR
                        Debug.Log("Error Creating Account");
                    }
                }
            }
            GUILayout.EndArea();
        }
	}

}