using UnityEngine;
using System.Collections;


/// <summary>
/// Karma for the player. If he falls under the floor move him high up in the air
/// </summary>
public class Karma : MonobitEngine.MonoBehaviour 
{		
	void Update () 
	{
        // オブジェクト所有権を所持しなければ実行しない
        if( !monobitView.isMine )
        {
            return;
        }
        
		if(transform.position.y < -5)
		{
			transform.position = new Vector3(0,25,0);
		}	
	}
}
