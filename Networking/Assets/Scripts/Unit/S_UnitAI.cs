using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{
    public interface INetworkListener
    {
        void callMethod(string aName, object[] aArgs);
    }

    [RequireComponent(typeof(NavMeshAgent))]
	public class S_UnitAI : MonoBehaviour , INetworkListener
    {
        public const byte STATE_IDLE = 0;
        public const byte STATE_MOVING = 1;

        private NavMeshAgent m_Agent = null;


        [SerializeField()]
        private VectorValue m_Destination = new VectorValue("m_Destination", Vector3.zero);
        private ByteValue m_State = new ByteValue("m_State", STATE_IDLE);
        private NetworkComponent m_Network = null;

        //Client
        private bool m_IsSelected = false;


		// Use this for initialization
		void Start () 
        {
            m_Network = GetComponent<NetworkComponent>();
            if (m_Network == null)
            {
                Debug.LogError("MISSING NETWORK COMPONENT");
            }
            if (Network.isClient)
            {
                m_Destination.setReadOnly();
                m_State.setReadOnly();
            }
            else if (Network.isServer)
            {
                m_Destination.setWriteOnly();
                m_State.setWriteOnly();
            }
            m_Network.registerListener(this);

            m_Network.addValue(m_Destination);
            m_Network.addValue(m_State);
		}
		
		// Update is called once per frame
		void Update () 
        {
            

            switch (m_State.data)
            {
                case STATE_IDLE:
                    Debug.Log("Idle");
                    break;

                case STATE_MOVING:
                    Debug.Log("Moving");
                    break;
            }

            
		}

        void OnTriggerEnter(Collider aCollider)
        {
            
        }

        public void callMethod(string aName, object[] aArgs)
        {
            if(aName == "setPosition")
            {
                if (aArgs[0].GetType() == typeof(Vector3))
                {
                    transform.position = (Vector3)aArgs[0];
                }
            }
        }


        

	}

}