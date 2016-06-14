﻿using UnityEngine;
using System;
using System.Collections;
using MonobitEngine;
using MonobitEngine.Definitions;

public class SD_Unitychan_PC : MonobitEngine.MonoBehaviour
{
    private Animator animator;                  // アニメータコントローラ
    private int animId = 0;                     // 再生中のアニメーションID
	private bool isMainCameraDisabled = false;	// メインカメラ復旧用フラグ

    // Use this for initialization
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animId = Animator.StringToHash("animId");

		if (!monobitView.isMine)
        {
            gameObject.transform.Find("Camera").GetComponent<Camera>().enabled = false;
            gameObject.transform.Find("Camera").GetComponent<AudioListener>().enabled = false;
        }
        else
        {
            GameObject.Find("Main Camera").GetComponent<Camera>().enabled = false;
			isMainCameraDisabled = true;
		}
    }

	void OnDestroy()
	{
		if( isMainCameraDisabled )
		{
			GameObject.Find("Main Camera").GetComponent<Camera>().enabled = true;
		}
	}

    // Update is called once per frame
    public void Update()
    {
		if (monobitView.isMine)
        {
            // キャラクタの移動＆アニメーション切り替え
            if (Input.GetKey("up"))
            {
                gameObject.transform.position += gameObject.transform.forward * 0.1f;
                animator.SetInteger(animId, 1);
            }
            else
            {
                animator.SetInteger(animId, 0);
            }
            if (Input.GetKey("right"))
            {
                gameObject.transform.Rotate(0, 2.0f, 0);
            }
            if (Input.GetKey("left"))
            {
                gameObject.transform.Rotate(0, -2.0f, 0);
            }
            if (Input.GetKeyDown("z"))
            {
                MonobitNetwork.Instantiate("Cube", transform.position, transform.rotation, 0);
            }
        }
    }
}