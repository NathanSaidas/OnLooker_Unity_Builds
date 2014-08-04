using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace OnLooker
{
    [Serializable]
    public class NetworkUserInfo
    {
        private int m_NetworkAccess = NetworkAccess.ADMIN;
        private string m_Username = string.Empty;
        private string m_Email = string.Empty;
        private bool m_IsOnline = false;

        public int networkAccess
        {
            get { return m_NetworkAccess; }
            set { m_NetworkAccess = value; }
        }
        public string username
        {
            get { return m_Username; }
            set { m_Username = value; }
        }
        public string email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }
        public bool isOnline
        {
            get { return m_IsOnline; }
            set { m_IsOnline = value; }
        }

        public byte[] toBytes()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            return stream.ToArray();
        }
        public void fromBytes(byte[] aBytes)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(aBytes);
            NetworkUserInfo user = (NetworkUserInfo)formatter.Deserialize(stream);
            username = user.username;
            email = user.email;
            isOnline = user.isOnline;
            networkAccess = user.networkAccess;
        }

        public static byte[] toBytesFromList(List<NetworkUserInfo> aUsers)
        {
            if(aUsers == null)
            {
                return null;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, aUsers);
            return stream.ToArray();

        }
        public static List<NetworkUserInfo> toListFromBytes(byte[] aData)
        {
            if (aData == null)
            {
                return null;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(aData);
            List<NetworkUserInfo> data = (List<NetworkUserInfo>)formatter.Deserialize(stream);
            return data;
        }

    }

    [Serializable]
	public class NetworkUser
    {
        [SerializeField]
        private NetworkHandle m_Handle = new NetworkHandle();
        [SerializeField]
        private int m_NetworkAccess = NetworkAccess.ADMIN;
        [SerializeField]
        private string m_Password = string.Empty;
        [SerializeField]
        private string m_Email = string.Empty;
        [SerializeField]
        private bool m_IsOnline = false;
        [SerializeField]
        private int m_ID = 0;


        //Info about IP Address and such
        public NetworkPlayer networkPlayer
        {
            get { return m_Handle.player; }
            set { m_Handle.player = value; }
        }
        //Players username
        public string username
        {
            get { return m_Handle.username; }
            set { m_Handle.username = value; }
        }
        //Players id inside the game, eg Player_1 Player_2, Red/Blue, Ally / Horde
        public int gameID
        {
            get { return m_Handle.id; }
            set { m_Handle.id = value; }
        }
        //The level of network access this player has
        public int networkAccess
        {
            get { return m_NetworkAccess; }
            set { m_NetworkAccess = value; }
        }
        //The players password
        public string password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }
        //The players email
        public string email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }
        //Whether or not the player is online
        public bool isOnline
        {
            get { return m_IsOnline; }
            set { m_IsOnline = value; }
        }
        //The players ID in the Authentication Server
        public int id
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public NetworkUserInfo info
        {
            get
            {
                NetworkUserInfo userInfo = new NetworkUserInfo();
                userInfo.username = username;
                userInfo.email = email;
                userInfo.isOnline = isOnline;
                userInfo.networkAccess = networkAccess;
                return userInfo;
            }
        }

        public static List<NetworkUserInfo> toInfoList(List<NetworkUser> aUsers)
        {
            if (aUsers == null)
            {
                return null;
            }
            List<NetworkUserInfo> userInfos = new List<NetworkUserInfo>();

            for (int i = 0; i < aUsers.Count; i++)
            {
                NetworkUserInfo lInfo = new NetworkUserInfo();
                lInfo.username = aUsers[i].username;
                lInfo.email = aUsers[i].email;
                lInfo.isOnline = aUsers[i].isOnline;
                lInfo.networkAccess = aUsers[i].networkAccess;
                userInfos.Add(lInfo);
            }
            return userInfos;
        }

        public byte[] toBytes
        {
            get
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }
        public static NetworkUser fromBytes(byte[] aBytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(aBytes);
            NetworkUser user = (NetworkUser)bf.Deserialize(ms);
            return user;
        }

		
	}

}