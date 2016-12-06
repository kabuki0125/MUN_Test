using UnityEngine;
using System.Collections;

public class BtnReturnMenu : MonoBehaviour
{
	/// <summary>
	/// サンプルメニューに戻る
	/// </summary>
	public void SceneLoad()
	{
		Application.LoadLevel("SampleMenu");
	}
}
