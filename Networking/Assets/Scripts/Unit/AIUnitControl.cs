using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class AIUnitControl : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

        void OnTriggerEnter(Collider aCollider)
        {
            if (aCollider.tag == "Finish")
            {
                //Destroy this
            }
        }
	}

}