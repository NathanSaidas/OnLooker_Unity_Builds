using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class GameServer
    {
        #region SINGLETON
        private static GameServer s_Instance = null;
        public static GameServer instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new GameServer();
                }
                return s_Instance;
            }
        }
        #endregion




    }

}