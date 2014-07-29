using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    [RequireComponent(typeof(NetworkView))]
	public class NetClientInput : MonoBehaviour {

        public float speed = 10.0f;
        private CharacterController controller;

        private NetworkPlayer m_Owner;
        private float lastMotionH = 0.0f;
        private float lastMotionV = 0.0f;


        private float positionErrorThreshhold = 0.2f;
        public Vector3 serverPos;
        public Quaternion serverRot;
		

        [RPC]
        public void setOwner(NetworkPlayer aPlayer)
        {
            m_Owner = aPlayer;
            if (aPlayer == Network.player)
            {
                enabled = true;
            }
            else
            {
                //Disable other things

            }
        }
        [RPC]
        public NetworkPlayer getOwner()
        {
            return m_Owner;
        }

        private void Awake()
        {
            if (Network.isClient)
            {
                enabled = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Client Side
            if (Network.isServer)
            {
                return;
            }
            if ((m_Owner != null) && Network.player == m_Owner)
            {
                float mH = Input.GetAxis("Horizontal");
                float mV = Input.GetAxis("Vertical");
                if (mH != lastMotionH || mV != lastMotionV)
                {
                    networkView.RPC("updateClientMotion", RPCMode.Server, mH, mV);
                    lastMotionH = mH;
                    lastMotionV = mV;

                    controller.Move(new Vector3(lastMotionH * speed * Time.deltaTime, 0.0f, lastMotionV * speed * Time.deltaTime));


                    
                }
            }
        }


        public void lerpToTarget()
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