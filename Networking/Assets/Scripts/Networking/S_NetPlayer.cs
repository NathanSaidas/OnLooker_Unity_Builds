using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class S_NetPlayer : MonoBehaviour {

        
		void Start () 
        {
            if (Network.isServer)
            {

            }
		}
		void Update ()
        {
            if (Network.isClient)
            {
                return;
            }
            //Game Logic Updates go here
            {

            }
        }

        //RPC function calls here to set states
        //Input States
        
	}

}