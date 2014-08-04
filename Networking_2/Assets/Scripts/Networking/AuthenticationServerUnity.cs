using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OnLooker
{
    [RequireComponent(typeof(NetworkView))]
	public class AuthenticationServerUnity : MonoBehaviour 
    {
        public const int STATUS_TYPE_LOGIN = 1;
        public const int STATUS_TYPE_REGISTER_SERVER = 3;
        public const int STATUS_TYPE_NEW_PASSWORD = 4;

        [SerializeField]
        private float m_SaveTime = 180.0f;

        [SerializeField]
        private List<NetworkUser> m_Users;

        [SerializeField]
        private bool m_IsServerBuild = false;


        //
        int m_ConnectionCount = 16;
        int m_PortNumber = 26548;
        string m_GameTypeName = "OnLooker_Game";
        string m_GameName = "Authentication";
        //
        bool m_IsConnected = false;
        bool m_TryConnect = true;


        private int m_NewUserStatus = -1;
        private int m_LoginStatus = -1;
        private int m_NewPasswordStatus = -1;

		// Use this for initialization
		void Awake () 
        {
            AuthenticationServer.instance.saveTime = Mathf.Abs(m_SaveTime);
            FieldInfo field = AuthenticationServer.instance.GetType().GetField("m_Component", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(AuthenticationServer.instance, this);
            }
        }
        void Start()
        {
            DontDestroyOnLoad(gameObject);

            if (m_IsServerBuild)
            {
                //Create a server
                Network.InitializeServer(m_ConnectionCount, m_PortNumber, false);
                MasterServer.RegisterHost(m_GameTypeName, m_GameName, "Authentication Server");
            }
            else
            {
                refreshHostList();
            }
        }
        private void OnConnectedToServer()
        {
            if (m_IsConnected == false)
            {
                m_IsConnected = true;
                Debug.Log("Connected to Authentication Server");
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
        private void OnPlayerDisconnected(NetworkPlayer aPlayer)
        {
            Debug.Log("Player Disconnected");
            AuthenticationServer.instance.userLoggedOff(aPlayer);
        }
        void OnDestroy()
        {
            AuthenticationServer.instance.save();
        }
		
		// Update is called once per frame
		void Update () 
        {


            AuthenticationServer.instance.update();
            m_Users = AuthenticationServer.instance.nUsers;

            if (m_IsServerBuild == false && m_IsConnected == false)
            {
                if (m_TryConnect == true)
                {
                    HostData[] hostList = AuthenticationServer.instance.hostList;
                    if (hostList != null)
                    {
                        for (int i = 0; i < hostList.Length; i++)
                        {
                            if (hostList[i].gameName == m_GameName)
                            {
                                Network.Connect(hostList[i]);
                                m_TryConnect = false;
                                break;
                            }
                        }
                    }
                }
                else
                {

                }
            }
		}
        public void refreshHostList()
        {
            StartCoroutine(pollHostList());
        }

        private IEnumerator pollHostList()
        {
            MasterServer.RequestHostList(m_GameTypeName);

            Type authenticationServer = AuthenticationServer.instance.GetType();
            FieldInfo serverField = authenticationServer.GetField("m_HostList", BindingFlags.NonPublic | BindingFlags.Instance);
            if (serverField == null)
            {
                yield return 0;
            }

            float timeEnd = Time.time + 3.0f;
            while (Time.time < timeEnd)
            {
                HostData[] hostData = MasterServer.PollHostList();
                serverField.SetValue(AuthenticationServer.instance, hostData);
                yield return new WaitForEndOfFrame();
            }
        }



        //Called to server
        [RPC]
        private void loginServer(string aUsername, string aPassword, NetworkMessageInfo aInfo)
        {
            Debug.Log(aInfo.sender.guid + ": User made request for login");
            if (Network.isServer)
            {
                int status = AuthenticationServer.instance.login(aUsername, aPassword);
                if (status == AuthenticationServer.ERROR_NONE)
                {
                    AuthenticationServer.instance.userLoggedIn(aInfo.sender, aUsername);
                }
                Debug.Log("Login Status: " + status);
                networkView.RPC("statusReciever", aInfo.sender,STATUS_TYPE_LOGIN, status);
            }
        }
        [RPC]
        private void loginServerEmail(string aEmail, string aPassword, NetworkMessageInfo aInfo)
        {
            Debug.Log(aInfo.sender.guid + ": User made request for login");
            if (Network.isServer)
            {
                int status = AuthenticationServer.instance.loginEmail(aEmail, aPassword);
                if (status == AuthenticationServer.ERROR_NONE)
                {
                    AuthenticationServer.instance.userLoggedIn(aInfo.sender, aEmail);
                }
                networkView.RPC("statusReciever", aInfo.sender,STATUS_TYPE_LOGIN, status);
            }
        }
        [RPC]
        private void registerServer(string aUsername, string aPassword, string aEmail, int aNetworkAccess, NetworkMessageInfo aInfo)
        {
            Debug.Log(aInfo.sender.guid + ": User made request for register account");
            if (Network.isServer)
            {
                int status = AuthenticationServer.instance.newUser(aUsername, aPassword, aEmail,aNetworkAccess);
                networkView.RPC("statusReciever", aInfo.sender,STATUS_TYPE_REGISTER_SERVER, status);
            }
        }
        [RPC]
        private void newPasswordServer(string aUsername, string aOldPassword, string aNewPassword, NetworkMessageInfo aInfo)
        {
            Debug.Log(aInfo.sender.guid + ": User made request for new password");
            if (Network.isServer)
            {
                int status = AuthenticationServer.instance.changePassword(aUsername, aOldPassword, aNewPassword);
                networkView.RPC("statusReciever", aInfo.sender, STATUS_TYPE_NEW_PASSWORD, status);
            }
        }
        [RPC]
        private void newPasswordServerEmail(string aEmail, string aOldPassword, string aNewPassword, NetworkMessageInfo aInfo)
        {
            Debug.Log(aInfo.sender.guid + ": User made request for new password");
            if (Network.isServer)
            {
                int status = AuthenticationServer.instance.changePasswordEmail(aEmail, aOldPassword, aNewPassword);
                networkView.RPC("statusReciever", aInfo.sender, STATUS_TYPE_NEW_PASSWORD, status);
            }
        }
        //Called the client
        [RPC]
        private void statusReciever(int aSenderType, int aStatus)
        {
            
            switch (aSenderType)
            {
                case STATUS_TYPE_LOGIN:
                    Debug.Log("Recieving Status for Login: " + aStatus);
                    m_LoginStatus = aStatus;
                    break;
                case STATUS_TYPE_REGISTER_SERVER:
                    m_NewUserStatus = aStatus;
                    break;
                case STATUS_TYPE_NEW_PASSWORD:
                    m_NewPasswordStatus = aStatus;
                    break;
            }
        }

        public int getStatus(int aStatusType)
        {
            int status = 0;
            switch (aStatusType)
            {
                case STATUS_TYPE_LOGIN:
                    status = m_LoginStatus;
                    m_LoginStatus = -1;
                    break;
                case STATUS_TYPE_REGISTER_SERVER:
                    status = m_NewUserStatus;
                    m_NewUserStatus = -1;
                    break;
                case STATUS_TYPE_NEW_PASSWORD:
                    status = m_NewPasswordStatus;
                    m_NewPasswordStatus = -1;
                    break;
            }
            return status;
        }



        //Sent to the server
        [RPC]
        private void requestCall(int aRequest, NetworkMessageInfo aInfo)
        {
            if (Network.isClient)
            {
                return;
            }
            Debug.Log("Server Making Request");
            switch (aRequest)
            {
                case NetworkRequest.USER_LIST:
                    {
                        byte[] users = AuthenticationServer.instance.getOnlineUsers();
                        if (users != null)
                        {
                            Debug.Log("Sending " + users.Length + " bytes of data");
                            networkView.RPC("receiveCall", aInfo.sender, aRequest, users);
                        }
                    }
                    break;
            }
            
        }
        //Sent to client
        [RPC]
        private void receiveCall(int aRequest, byte[] aData)
        {
            if (aData != null)
            {
                Debug.Log("Recieved " + aData.Length + " bytes");
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(aData);
                List<NetworkPlayer> players = (List<NetworkPlayer>)formatter.Deserialize(stream);
                
            }
            else
            {
                Debug.Log("null data");
            }
        }


	}

}