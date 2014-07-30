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


        //Memory / System Information
        private int m_Handle = 0;
        private bool m_IsDirty = false;
        public int handle
        {
            get { return m_Handle; }
            set { m_Handle = value; }
        }
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
                //Network.Destroy(gameObject);
                networkView.RPC("destroy", RPCMode.AllBuffered, Network.player, handle, networkView.viewID);
            }
        }

        void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            Vector3 position = transform.position;
            Vector3 rotation = transform.rotation.eulerAngles;

            if (stream.isWriting && Network.isServer)
            {
                stream.Serialize(ref position);
                stream.Serialize(ref rotation);
            }
            else
            {
                stream.Serialize(ref position);
                stream.Serialize(ref rotation);

                transform.position = position;
                transform.rotation = Quaternion.Euler(rotation);
            }
        }

        [RPC]
        public void destroy(NetworkPlayer aSender, int aHandle, NetworkViewID aViewID)
        {
            if (Network.isServer && aSender == Network.player)
            {
                Network.Destroy(gameObject);
                Network.RemoveRPCs(aViewID);
                //Go through the list and find the game object with the matching handle
                //for (int i = 0; i < m_ServerGameObjects.Count; i++)
                //{
                //    if (m_ServerGameObjects[i] != null)
                //    {
                //        S_UnitAI unitAI = m_ServerGameObjects[i].GetComponent<S_UnitAI>();
                //        if (unitAI != null)
                //        {
                //            //If found then destroy the game object and remove it from the list
                //            if(unitAI.handle == aHandle)
                //            {
                //                Network.Destroy(unitAI.gameObject);
                //                m_ServerGameObjects.RemoveAt(i);
                //                return;
                //            }
                //        }
                //    }
                //}
            }
        }

	}

}