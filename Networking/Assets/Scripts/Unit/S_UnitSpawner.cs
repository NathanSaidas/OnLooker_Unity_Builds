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
                    m_ServerGameObjects.Add(unitGO);
                }
                for (int i = m_ServerGameObjects.Count - 1; i >= 0; i--)
                {
                    if (m_ServerGameObjects[i] == null)
                    {
                        m_ServerGameObjects.RemoveAt(i);
                    }
                }
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