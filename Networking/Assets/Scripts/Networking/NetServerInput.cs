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

        private float positionErrorThreshhold = 0.2f;
        private Vector3 serverPos;
        private Quaternion serverRot;

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


        void lerpToTarget()
        {
            float distance = Vector3.Distance(transform.position, serverPos);

            if (distance >= positionErrorThreshhold)
            {
                float lerp = ((1.0f / distance) * speed) / 100.0f;

                transform.position = Vector3.Lerp(transform.position, serverPos, lerp);
                transform.rotation = Quaternion.Slerp(transform.rotation, serverRot, lerp);
            }

        }
	}

}