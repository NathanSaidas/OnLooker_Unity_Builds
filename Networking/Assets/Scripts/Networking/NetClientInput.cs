using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

    [RequireComponent(typeof(NetworkView))]
	public class NetClientInput : MonoBehaviour {

        private NetworkPlayer m_Owner;
        private float lastMotionH = 0.0f;
        private float lastMotionV = 0.0f;


        
		

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


                    
                }
            }
        }

        
	}

}