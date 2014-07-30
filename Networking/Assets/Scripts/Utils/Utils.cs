using UnityEngine;
using System.Collections.Generic;

namespace OnLooker
{

	public class Utils : MonoBehaviour {

        public static string editorTextfield(string aLabel, string aContent)
        {
            string text = string.Empty;
            GUILayout.BeginHorizontal();
            GUILayout.Label(aLabel);
            text = GUILayout.TextField(aContent);
            GUILayout.EndHorizontal();
            return text;
        }
        public static string editorTextfield(string aLabel, string aContent, float aFixedLabelWidth)
        {
            string text = string.Empty;
            GUILayout.BeginHorizontal();
            GUILayout.Label(aLabel,GUILayout.MaxWidth(aFixedLabelWidth));
            text = GUILayout.TextField(aContent);
            GUILayout.EndHorizontal();
            return text;
        }

        public static bool editorButton(string aLabel, string aContent)
        {
            bool click = false;
            GUILayout.BeginHorizontal();
            GUILayout.Label(aLabel);
            click = GUILayout.Button(aContent);
            GUILayout.EndHorizontal();
            return click;
        }
	}

}