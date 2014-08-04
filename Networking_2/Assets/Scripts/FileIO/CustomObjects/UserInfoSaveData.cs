using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace OnLooker
{
    [Serializable]
    public class UserInfoSaveData : CustomSaveData
    {
        private string m_Username;
        private string m_Password;
        private string m_Email;
        private int m_ServerAccess;
        private int m_AuthenticationID;

        public UserInfoSaveData()
        {
            name = "UserInfo";
        }
        public UserInfoSaveData(string aName)
        {
            name = aName;
        }
        public UserInfoSaveData(SerializationInfo aInfo, StreamingContext aContext)
        {
            name = (string)aInfo.GetValue("Name", typeof(string));
            m_Username = (string)aInfo.GetValue("Username", typeof(string));
            m_Password = (string)aInfo.GetValue("Password", typeof(string));
            m_Email = (string)aInfo.GetValue("Email", typeof(string));
            m_ServerAccess = (int)aInfo.GetValue("ServerAccess", typeof(int));
            m_AuthenticationID = (int)aInfo.GetValue("AuthenticationID", typeof(int));

        }
        public override void save(Stream aStream, BinaryFormatter aFormatter)
        {
            if (aStream != null && aFormatter != null)
            {
                aFormatter.Serialize(aStream, this);
            }
        }
        //Override this method for additional data make sure to call the base
        public override void GetObjectData(SerializationInfo aInfo, StreamingContext aContext)
        {
            aInfo.AddValue("Name", name);
            aInfo.AddValue("Username", m_Username);
            aInfo.AddValue("Password", m_Password);
            aInfo.AddValue("Email", m_Email);
            aInfo.AddValue("ServerAccess", m_ServerAccess);
            aInfo.AddValue("AuthenticationID", m_AuthenticationID);
        }

        public string username
        {
            get { return m_Username; }
            set { m_Username = value; }
        }
        public string password
        {
            get { return m_Password; }
            set { m_Password = value; }
        }
        public string email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }
        public int serverAccess
        {
            get { return m_ServerAccess; }
            set { m_ServerAccess = value; }
        }
        public int authenticationID
        {
            get { return m_AuthenticationID; }
            set { m_AuthenticationID = value; }
        }

        public NetworkUser networkUser
        {
            get
            {
                NetworkUser user = new NetworkUser();
                user.username = username;
                user.password = password;
                user.email = email;
                user.networkAccess = serverAccess;
                user.id = authenticationID;
                return user;
            }
        }
        
    }
}
