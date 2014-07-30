using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class S_UnitSpawner : MonoBehaviour {

        [SerializeField()]
        private GameObject m_Unit = null;
        [SerializeField()]
        private float m_SpawnTime = 0.0f;
        [SerializeField()]
        private float m_SpawnRate = 1.0f;

        [SerializeField()]
        private List<GameObject> m_ServerGameObjects = new List<GameObject>();

        private UniqueNumberGenerator m_UNG = new UniqueNumberGenerator();
        private UniqueNumberGenerator ung
        {
            get { return m_UNG; }
            set { m_UNG = value; }
        }

		// Use this for initialization
		void Start () 
        {
		
		}
		
		// Update is called once per frame
		void Update () 
        {
            if (Network.isServer == false)
            {
                return;
            }

            if (m_Unit == null)
            {
                return;
            }
            m_SpawnTime += Time.deltaTime;
            if (m_SpawnTime > m_SpawnRate)
            {
                m_SpawnTime = 0.0f;
                GameObject unitGO = (GameObject)Network.Instantiate(m_Unit, transform.position, Quaternion.identity, NetworkGroup.SERVER);
                if (unitGO != null)
                {
                    S_UnitAI unitAI = unitGO.GetComponent<S_UnitAI>();
                    if (unitAI != null)
                    {
                        unitAI.handle = ung.getUniqueNumber();
                    }
                    m_ServerGameObjects.Add(unitGO);
                }
                for (int i = m_ServerGameObjects.Count - 1; i >= 0; i--)
                {
                    if (m_ServerGameObjects[i] == null)
                    {
                        Debug.Log("Trash in the list removing");
                        m_ServerGameObjects.RemoveAt(i);
                    }
                }
            }

		}
        //[RPC]
        //void instantiate(NetworkPlayer aSender)
        //{
        //    GameObject unitGO = (GameObject)Network.Instantiate(m_Unit, transform.position, Quaternion.identity, NetworkGroup.SERVER);
        //    if (unitGO != null)
        //    {
        //        m_ServerGameObjects.Add(unitGO);
        //    }
        //}
        //
        [RPC]
        public void destroy(NetworkPlayer aSender, int aHandle, NetworkViewID aViewID)
        {
            if (Network.isServer && aSender == Network.player)
            {
                Network.Destroy(aViewID);
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

        //[RPC]
        //void unregisterGameObject(NetworkPlayer aPlayer)
        //{
        //    if (Network.isServer && aPlayer == Network.player)
        //    {
        //        m_ServerGameObjects.Remove(
        //    }
        //}
	}

}