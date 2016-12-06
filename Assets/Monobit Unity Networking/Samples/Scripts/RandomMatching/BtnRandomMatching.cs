using UnityEngine;
using System.Collections;

/// <summary>
/// RandomMatchingに遷移する
/// </summary>
public class BtnRandomMatching : MonoBehaviour
{
	/// <summary>
	/// RandomMachingに遷移する
	/// </summary>
	public void SceneLoad()
	{
		Application.LoadLevel("OfflineScene");
	}
}
