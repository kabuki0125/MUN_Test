using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// NoneProgrammingに遷移する
/// </summary>
public class BtnNoneProgramming : MonoBehaviour
{
	/// <summary>
	/// NoneProgrammingに遷移する
	/// </summary>
	public void SceneLoad()
	{
        SceneManager.LoadScene("NoneProgramming", LoadSceneMode.Additive);
	}
}
