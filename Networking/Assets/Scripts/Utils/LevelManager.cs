using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class LevelManager : MonoBehaviour
    {
        private static string[] s_LevelNames;

        public static int LEVEL_LOBBY = 0;
        public static int LEVEL_WORLD = 1;

        public static string getLevelString(int aIndex)
        {
            if (s_LevelNames != null)
            {
                if (aIndex >= 0 && aIndex < s_LevelNames.Length)
                {
                    return s_LevelNames[aIndex];
                }
            }
            return string.Empty;   
        }

        [SerializeField()]
        private string[] m_LevelNames = new string[] { "Lobby", "World" };

        public void Start()
        {
            s_LevelNames = m_LevelNames;
        }
        public void Update()
        {
            //Visual Update
            m_LevelNames = s_LevelNames;
        }
            
		
	}

}