using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{
    [ExecuteInEditMode()]
	public class WorldGUI : MonoBehaviour {
        
        [SerializeField()]
        private Rect m_MainGUI = new Rect(Screen.width * 0.5f - 50.0f, Screen.height * 0.5f - 50.0f, 100.0f, 100.0f);



		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

        void OnGUI()
        {
            if (GUI.Button(m_MainGUI,"Disconnect"))
            {
                Network.Disconnect();
                if (Network.isServer)
                {
                    MasterServer.UnregisterHost();
                }

                Application.LoadLevel(LevelManager.LEVEL_LOBBY);
            }
        }
	}

}