using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    public struct NetworkConnectionInfo
    {
        private NetworkDisconnection m_Flag;
        private float m_Time;
        private bool m_Connected;

        public NetworkDisconnection flag
        {
            get { return m_Flag; }
            set { m_Flag = value; }
        }
        public float time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }
        public bool connected
        {
            get { return m_Connected; }
            set { m_Connected = value; }
        }
    }

}