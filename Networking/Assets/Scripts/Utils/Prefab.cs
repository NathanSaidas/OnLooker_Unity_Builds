using UnityEngine;
using System;
using System.Collections.Generic;

namespace OnLooker
{
    //Class designed to encapsulate prefabs and give them a name
    [System.Serializable]
    public class Prefab
    {
        [SerializeField()]
        private string m_Name = null;
        [SerializeField()]
        private GameObject m_GameObject = null;

        private int m_ID = 0;

        public string name
        {
            get { return m_Name; }
        }
        public GameObject gameObject
        {
            get { return m_GameObject; }
        }
        public int id
        {
            get { return m_ID; }
        }
    }

}