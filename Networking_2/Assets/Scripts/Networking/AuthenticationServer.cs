using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OnLooker
{
    

    //Purpose of this class is to provide a safe way of storing data and retrieving it
	public class AuthenticationServer
    {
        #region SINGLETON
        private static AuthenticationServer s_Instance = null;
        public static AuthenticationServer instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new AuthenticationServer();
                }
                return s_Instance;
            }
        }
        #endregion

        public const int STATUS_PENDING = -2;
        public const int ERROR_UNKNOWN = -1;
        public const int ERROR_NONE = 0;
        //User does not exist
        public const int ERROR_USERNAME_NOT_EXISTS = 1;
        //Email does not exist
        public const int ERROR_EMAIL_NOT_EXISTS = 2;
        public const int ERROR_INVALID_USERNAME = 4;
        public const int ERROR_INVALID_PASSWORD = 8;
        public const int ERROR_INVALID_EMAIL = 16;

        public AuthenticationServer()
        {
            m_Data = new FileData("Users");
            m_Data.load();
            m_Data.save();

            //Get all the users from the save file
            UserInfoSaveData[] users = m_Data.get<UserInfoSaveData>();
            if (users != null)
            {
                //Clear our current user list
                m_Users.Clear();
                //add each user from the harddisk into a proper network user on the server ram
                for (int i = 0; i < users.Length; i++)
                {
                    m_Users.Add(users[i].networkUser);
                    m_Users[i].id = m_UserIDGen.getUniqueNumber();
                    m_Users[i].isOnline = false;
                }
            }

        }


        private FileData m_Data = null;
        private List<NetworkUser> m_Users = new List<NetworkUser>();
        private List<NetworkUser> m_OnlineUser = new List<NetworkUser>();
        private UniqueNumberGenerator m_UserIDGen = new UniqueNumberGenerator();
        private float m_SaveTime = 180.0f;  //3 Min
        private float m_CurrentTime = 0.0f;

        private AuthenticationServerUnity m_Component = null;
        private HostData[] m_HostList = null;

        public List<NetworkUser> nUsers
        {
            get { return m_Users; }
        }

        public int newUser(string aUsername, string aPassword, string aEmail)
        {
            return newUser(aUsername, aPassword, aEmail, NetworkAccess.DEFAULT);
        }

        public int newUser(string aUsername, string aPassword, string aEmail, int aNetworkAccess)
        {
            

            //Search for users with the same username or email
            int returnFlag = ERROR_NONE;

            if (aUsername == string.Empty)
            {
                returnFlag |= ERROR_INVALID_USERNAME;
            }
            if (aPassword == string.Empty)
            {
                returnFlag |= ERROR_INVALID_PASSWORD;
            }
            if (aEmail == string.Empty)
            {
                returnFlag |= ERROR_INVALID_EMAIL;
            }
            if (returnFlag != ERROR_NONE)
            {
                return returnFlag;
            }

            if (Network.isClient)
            {
                if (m_Component != null)
                {
                    m_Component.networkView.RPC("registerServer", RPCMode.Server, aUsername, aPassword, aEmail, aNetworkAccess);
                }
                else
                {
                    Debug.Log("Missing AuthenticationServerUnity Component");
                }
                return STATUS_PENDING;
            }

            for (int i = 0; i < m_Users.Count; i++)
            {
                if (m_Users[i].username == aUsername)
                {
                    returnFlag |= ERROR_USERNAME_NOT_EXISTS;
                }
                if (m_Users[i].email == aEmail)
                {
                    returnFlag |= ERROR_EMAIL_NOT_EXISTS;
                }
            }
            if (returnFlag != ERROR_NONE)
            {
                return returnFlag;
            }

            NetworkUser user = new NetworkUser();
            user.username = aUsername;
            user.password = aPassword;
            user.email = aEmail;
            user.id = m_UserIDGen.getUniqueNumber();
            user.isOnline = false;
            user.networkAccess = aNetworkAccess;

            m_Users.Add(user);
            
            return returnFlag;
        }
        public int login(string aUsername, string aPassword)
        {
            int returnFlag = ERROR_NONE;
            if (aUsername == string.Empty)
            {
                returnFlag |= ERROR_INVALID_USERNAME;
            }
            if (aPassword == string.Empty)
            {
                returnFlag |= ERROR_INVALID_PASSWORD;
            }
            if (returnFlag != ERROR_NONE)
            {
                return returnFlag;
            }

            if (Network.isClient)
            {
                if (m_Component != null)
                {
                    Debug.Log("Requesting Status from Login Server");
                    m_Component.networkView.RPC("loginServer", RPCMode.Server, aUsername, aPassword);
                }
                else
                {
                    Debug.Log("Missing AuthenticationServerUnity Component");
                }
                return STATUS_PENDING;
            }

            for (int i = 0; i < m_Users.Count; i++)
            {
                if (m_Users[i].username == aUsername)
                {
                    if (m_Users[i].password == aPassword)
                    {
                        m_Users[i].isOnline = true;
                        return ERROR_NONE;
                    }
                    else
                    {
                        return ERROR_INVALID_PASSWORD;
                    }
                }
            }

            return ERROR_USERNAME_NOT_EXISTS;
        }
        public int loginEmail(string aEmail, string aPassword)
        {
            int returnFlag = ERROR_NONE;
            if (aEmail == string.Empty)
            {
                returnFlag |= ERROR_INVALID_USERNAME;
            }
            if (aPassword == string.Empty)
            {
                returnFlag |= ERROR_INVALID_PASSWORD;
            }
            if (returnFlag != ERROR_NONE)
            {
                return returnFlag;
            }

            if (Network.isClient)
            {
                if (m_Component != null)
                {
                    m_Component.networkView.RPC("loginServerEmail", RPCMode.Server, aEmail, aPassword);
                }
                else
                {
                    Debug.Log("Missing AuthenticationServerUnity Component");
                }
                return STATUS_PENDING;
            }

            for (int i = 0; i < m_Users.Count; i++)
            {
                if (m_Users[i].email == aEmail)
                {
                    if (m_Users[i].password == aPassword)
                    {
                        m_Users[i].isOnline = true;
                        return ERROR_NONE;
                    }
                    else
                    {
                        return ERROR_INVALID_PASSWORD;
                    }
                }
            }
            return ERROR_EMAIL_NOT_EXISTS;
        }

        public int changePassword(string aUsername, string aOldPassword, string aNewPassword)
        {
            int returnFlag = ERROR_NONE;
            if (aUsername == string.Empty)
            {
                returnFlag |= ERROR_INVALID_USERNAME;
            }
            if (aOldPassword == string.Empty || aNewPassword == string.Empty)
            {
                returnFlag |= ERROR_INVALID_PASSWORD;
            }
            if (returnFlag != ERROR_NONE)
            {
                return returnFlag;
            }
            if (Network.isClient)
            {
                if (m_Component != null)
                {
                    m_Component.networkView.RPC("newPasswordServer", RPCMode.Server, aUsername, aOldPassword, aNewPassword);
                }
                else
                {
                    Debug.Log("Missing AuthenticationServerUnity Component");
                }
                return STATUS_PENDING;
            }
            for (int i = 0; i < m_Users.Count; i++)
            {
                if (m_Users[i].username == aUsername)
                {
                    if (m_Users[i].password == aOldPassword)
                    {
                        m_Users[i].password = aNewPassword;
                        return ERROR_NONE;
                    }
                    else
                    {
                        return ERROR_INVALID_PASSWORD;
                    }
                }
            }
            return ERROR_USERNAME_NOT_EXISTS;
        }

        public int changePasswordEmail(string aEmail, string aOldPassword, string aNewPassword)
        {
            int returnFlag = ERROR_NONE;
            if (aEmail == string.Empty)
            {
                returnFlag |= ERROR_INVALID_USERNAME;
            }
            if (aOldPassword == string.Empty || aNewPassword == string.Empty)
            {
                returnFlag |= ERROR_INVALID_PASSWORD;
            }
            if (returnFlag != ERROR_NONE)
            {
                return returnFlag;
            }
            if (Network.isClient)
            {
                if (m_Component != null)
                {
                    m_Component.networkView.RPC("newPasswordServer", RPCMode.Server, aEmail, aOldPassword, aNewPassword);
                }
                else
                {
                    Debug.Log("Missing AuthenticationServerUnity Component");
                }
                return STATUS_PENDING;
            }
            for (int i = 0; i < m_Users.Count; i++)
            {
                if (m_Users[i].email == aEmail)
                {
                    if (m_Users[i].password == aOldPassword)
                    {
                        m_Users[i].password = aNewPassword;
                        return ERROR_NONE;
                    }
                    else
                    {
                        return ERROR_INVALID_PASSWORD;
                    }
                }
            }
            return ERROR_EMAIL_NOT_EXISTS;
        }


        public void save()
        {
            m_Data.clear();
            for (int i = 0; i < m_Users.Count; i++)
            {
                UserInfoSaveData data = new UserInfoSaveData(m_Users[i].username);
                data.username = m_Users[i].username;
                data.password = m_Users[i].password;
                data.email = m_Users[i].email;
                data.authenticationID = m_Users[i].id;
                data.serverAccess = m_Users[i].networkAccess;
                m_Data.add(data);
            }
            m_Data.save();
        }

        public void update()
        {
            //Every X seconds, Save the server
            m_CurrentTime += Time.deltaTime;
            if (m_CurrentTime > m_SaveTime)
            {
                save();
                m_CurrentTime = 0.0f;
            }
            
        }

        #region HELPERS
        //Helpers
        public NetworkUser getUser(string aUsername)
        {
            for (int i = 0; i < m_Users.Count; i++)
            {
                if (m_Users[i].username == aUsername)
                {
                    return m_Users[i];
                }
            }
            return null;
        }
        public NetworkUser getUser(int aID)
        {
            for (int i = 0; i < m_Users.Count; i++)
            {
                if (m_Users[i].id == aID)
                {
                    return m_Users[i];
                }
            }
            return null;
        }
        public byte[] getOnlineUsers()
        {
            if (Network.isClient)
            {
                //request
                return null;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            return stream.ToArray();
        }
        public NetworkUser getUserEmail(string aEmail)
        {
            for (int i = 0; i < m_Users.Count; i++)
            {
                if (m_Users[i].email == aEmail)
                {
                    return m_Users[i];
                }
            }
            return null;
        }
        public float saveTime
        {
            get { return m_SaveTime; }
            set { m_SaveTime = value; }

        }
        public HostData[] hostList
        {
            get { return m_HostList; }
        }

        public int getStatus(int aStatusType)
        {
            if (m_Component == null)
            {
                Debug.Log("Missing AuthenticationServerUnity Component");
                return -1;
            }
            return m_Component.getStatus(aStatusType);
        }

        public void userLoggedIn(NetworkPlayer aPlayer, string aUsername)
        {
            if (Network.isClient)
            {
                Debug.Log("User is client");
                return;
            }

            NetworkUser user = getUser(aUsername);
            if (user != null)
            {
                user.isOnline = true;
                user.networkPlayer = aPlayer;
                m_OnlineUser.Add(user);

                Debug.Log("Player " + aUsername + "logged in at " + aPlayer.guid);
            }
            else
            {
                Debug.Log("User does not exist");
            }
        }

        public void userLoggedOff( NetworkPlayer aPlayer)
        {
            if (Network.isClient)
            {
                return;
            }
            for (int i = 0; i < m_OnlineUser.Count; i++)
            {
                if (m_OnlineUser[i].networkPlayer == aPlayer)
                {
                    Debug.Log("Player " + m_OnlineUser[i].username + "logged off at " + aPlayer.guid);
                    m_OnlineUser[i].isOnline = false;
                    m_OnlineUser.RemoveAt(i);
                    break;
                }
            }
        }

        public void requestCall()
        {
            if (m_Component == null)
            {
                return;
            }

            if (Network.isClient)
            {
                m_Component.networkView.RPC("requestCall", RPCMode.Server);
            }
        }

        #endregion

    }

}