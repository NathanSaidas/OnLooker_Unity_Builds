using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class UnitSelectionManager : MonoBehaviour {

        [SerializeField()]
        private UnitControl m_CurrentControlled;
        [SerializeField()]
        private int m_UnitLayer = 8;
        [SerializeField()]
        private Camera m_CurrentCam = null;
		// Use this for initialization
		void Start () 
        {
            //m_CurrentCam = Camera.current;
		}
		
		// Update is called once per frame
		void FixedUpdate () 
        {
            if (Input.GetMouseButtonDown(0) && m_CurrentCam != null)
            {
                Debug.Log("Searching for unit");
                int layer = 1 << m_UnitLayer;

                RaycastHit aHit;
                Ray ray = m_CurrentCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out aHit, Mathf.Infinity, layer))
                {
                    UnitControl unit = aHit.collider.GetComponent<UnitControl>();
                    if (unit != null)
                    {
                        if (m_CurrentControlled != null)
                        {
                            m_CurrentControlled.selected = false;
                        }
                        unit.selected = true;
                        m_CurrentControlled = unit;
                    }
                }
                else
                {
                    if (m_CurrentControlled != null)
                    {
                        m_CurrentControlled.selected = false;
                        m_CurrentControlled = null;
                    }
                }
            }
		}
	}

}