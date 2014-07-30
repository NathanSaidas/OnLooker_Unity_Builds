using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{
    [RequireComponent(typeof(NavMeshAgent))]
	public class UnitControl : MonoBehaviour 
    {
        [SerializeField()]
        private bool m_Selected = false;

        public bool selected
        {
            get { return m_Selected; }
            set { m_Selected = value; }
        }

        [SerializeField()]
        private Vector3 m_Target = Vector3.zero;
        [SerializeField()]
        private Camera m_PlayerCamera = null;
        private NavMeshAgent m_Agent = null;
		// Use this for initialization
		void Start () {
            m_Agent = GetComponent<NavMeshAgent>();
		}
		
		// Update is called once per frame
		void Update ()
        {

            if(m_PlayerCamera == null || m_Agent == null)
            {
                Debug.Log("error");
                return;
            }
            if (Input.GetMouseButtonDown(1) && m_Selected == true)
            {
                RaycastHit hit;
                Ray ray = m_PlayerCamera.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray,out hit))
                {
                    m_Target = hit.point;
                    m_Agent.SetDestination(m_Target);
                }

                
            }
		}

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawWireSphere(m_Agent.destination, 1.0f);
        }
	}

}