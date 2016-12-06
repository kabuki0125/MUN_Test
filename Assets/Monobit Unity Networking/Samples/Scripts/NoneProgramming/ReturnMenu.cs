using UnityEngine;
using System.Collections;

public class ReturnMenu : MonoBehaviour {

	void OnGUI () {
        // GUI用の解像度を調整する
        Vector2 guiScreenSize = new Vector2(800, 480);
        if (Screen.width > Screen.height)
        {
            // landscape
            GUIUtility.ScaleAroundPivot(new Vector2(Screen.width / guiScreenSize.x, Screen.height / guiScreenSize.y), Vector2.zero);
        }
        else
        {
            // portrait
            GUIUtility.ScaleAroundPivot(new Vector2(Screen.width / guiScreenSize.y, Screen.height / guiScreenSize.x), Vector2.zero);
        }

        GUILayout.Space(20);
		
		// メニューに戻る
		if (GUILayout.Button("Return Menu", GUILayout.Width(100)))
		{
			Application.LoadLevel("SampleMenu");
		}
	}
}
