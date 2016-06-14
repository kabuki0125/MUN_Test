using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReturnMenu : MonoBehaviour {

	void OnGUI () {
		GUILayout.Space(20);
		
		// メニューに戻る
		if (GUILayout.Button("Return Menu", GUILayout.Width(100)))
		{
            SceneManager.LoadScene("SampleMenu", LoadSceneMode.Additive);
		}
	}
}
