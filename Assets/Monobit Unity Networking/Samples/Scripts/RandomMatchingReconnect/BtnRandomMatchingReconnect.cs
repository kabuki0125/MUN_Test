using UnityEngine;
using UnityEngine.SceneManagement;
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
        SceneManager.LoadScene("OfflineSceneReconnect", LoadSceneMode.Additive);
	}
}
