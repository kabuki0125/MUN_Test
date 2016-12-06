using UnityEngine;
using System.Collections;

/// <summary>
/// RandomMatchingReconnectに遷移する
/// </summary>
public class BtnRandomMatchingReconnect : MonoBehaviour
{
	/// <summary>
	/// RandomMatchingReconectに遷移する
	/// </summary>
	public void SceneLoad()
	{
		Application.LoadLevel("OfflineSceneReconnect");
	}
}
