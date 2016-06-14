using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BtnReturnMenu : MonoBehaviour
{
	/// <summary>
	/// サンプルメニューに戻る
	/// </summary>
	public void SceneLoad()
	{
        SceneManager.LoadScene("SampleMenu", LoadSceneMode.Additive);
	}
}
