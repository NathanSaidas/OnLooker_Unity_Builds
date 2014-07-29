using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{
    //This class holds all the data related to the game object.
    //Character to Controller
    //Buffs 
    //Health / Mana etc..
    [RequireComponent(typeof(NetworkView))]
	public class NetServerInput : MonoBehaviour {

        public float speed = 10.0f;
        private CharacterController controller;

        private float horizontalMotion = 0.0f;
        private float verticalMotion = 0.0f;

        

		// Use this for initialization
		void Start ()
        {
            if (Network.isServer)
            {
                controller = GetComponent<CharacterController>();
            }
		}
		
		// Update is called once per frame
		void Update ()
        {
            //Server only
            if (Network.isClient)
            {
                return;
            }
            if (controller != null)
            {
                controller.Move(new Vector3(horizontalMotion * speed * Time.deltaTime, 0.0f, verticalMotion * speed * Time.deltaTime));
            }
		}

        [RPC]
        public void updateClientMotion(float horizontal, float vertical)
        {
            horizontalMotion = horizontal;
            verticalMotion = vertical;
        }


        
	}

}