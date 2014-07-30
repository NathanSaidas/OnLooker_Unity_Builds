using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{
    [RequireComponent(typeof(NavMeshAgent))]
	public class S_UnitAI : MonoBehaviour
    {
        private NavMeshAgent m_Agent = null;
        [SerializeField()]
        private Vector3 m_Destination = Vector3.zero;

        private bool m_IsDirty = false;
        public bool isDirty
        {
            get { return m_IsDirty; }
            set { m_IsDirty = value; }
        }
		// Use this for initialization
		void Start () 
        {
            if (m_Agent == null)
            {
                m_Agent = GetComponent<NavMeshAgent>();
            }
            GameObject go = GameObject.FindGameObjectWithTag("Finish");
            if (go != null)
            {
                m_Destination = go.transform.position;
            }
		}
		
		// Update is called once per frame
		void Update () 
        {
            if (m_Agent != null)
            {
                m_Agent.SetDestination(m_Destination);
            }
		}

        void OnTriggerEnter(Collider aCollider)
        {
            //networkView.RPC("unregisterGameObject", RPCMode.Server, Network.player);
            if (Network.isServer == true && aCollider.tag == "Finish")
            {
                Network.Destroy(gameObject);
            }
        }


	}

}