using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{
    public class NetworkPriority
    {
        public const int SERVER = 0;
        public const int ADMIN = 1;
        public const int MODERATOR = 2;
        public const int MEMBER = 3;
        public const int DEFAULT = 4;

    }
	public class NetworkUser
    {
        private NetworkHandle m_Handle = new NetworkHandle();
        private int m_Priority = NetworkPriority.ADMIN;
        private bool m_IsOnline = false;

        public NetworkHandle handle
        {
            get { return m_Handle; }
            set { m_Handle = value; }
        }
        public int priority
        {
            get { return m_Priority; }
            set { m_Priority = value; }
        }
        public bool isOnline
        {
            get { return m_IsOnline; }
            set { m_IsOnline = value; }
        }
	}

}