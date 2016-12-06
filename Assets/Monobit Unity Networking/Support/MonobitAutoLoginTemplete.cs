using System;
using UnityEngine;
using MonobitEngine;

namespace Monobit.Support
{
    [Serializable]
    [RequireComponent(typeof(MonobitView))]
    [AddComponentMenu("Monobit Networking Support/Monobit Auto Login Templete &v")]
    public class MonobitAutoLoginTemplete : MonobitEngine.MonoBehaviour
    {
        [SerializeField]
        public GameObject InstantiatePrefab = null;

        [SerializeField]
        public Vector3 camPosition = new Vector3(1, 1, -3);

        [SerializeField]
        public Quaternion camRotation = Quaternion.identity;

		private GameObject go = null;

		private bool bStart = false;
		private bool bSelectMenu = false;

        void OnGUI()
        {
            if (bSelectMenu == false)
            {
                if (!MonobitNetwork.isConnect)
                {
                    if (GUILayout.Button("Connect", GUILayout.Width(150)))
                    {
                        bSelectMenu = true;
                        MonobitNetwork.autoJoinLobby = true;
                        MonobitNetwork.ConnectServer("MonobitAutoLoginTemplete_v0.1");
                    }
                }
                else if (MonobitNetwork.inRoom)
				{
					if (!bStart)
					{
						if (GUILayout.Button("GameStart", GUILayout.Width(150)))
						{
							bSelectMenu = true;
							monobitView.RPC("GameStart", MonobitTargets.All, null);
						}
					}
					else
					{
						if (GUILayout.Button("Disconnect", GUILayout.Width(150)))
						{
							MonobitNetwork.DisconnectServer();
						}
					}
                }
            }
        }

        void OnConnectToServerFailed(DisconnectCause cause)
        {
            bSelectMenu = false;
            Monobit.Utl.LogE("OnConnectToServerFailed cause={0}", cause);
        }
        void OnDisconnectedFromServer()
        {
            bSelectMenu = false;

            // 全てのオブジェクトを消すため、シーンを再ロードする。
            Application.LoadLevel(Application.loadedLevel);
        }
        void OnJoinedLobby()
        {
			bSelectMenu = false;
			Monobit.Utl.LogD("OnJoinedLobby");
			MonobitNetwork.JoinRandomRoom();
		}
		void OnJoinRoomFailed()
        {
            Monobit.Utl.LogD("OnJoinRoomFailed");
            MonobitNetwork.CreateRoom("AutoLoginRoom");
        }
        void OnMonobitRandomJoinFailed()
        {
            Monobit.Utl.LogD("OnMonobitRandomJoinFailed");
            MonobitNetwork.CreateRoom("AutoLoginRoom");
        }
        void OnJoinedRoom()
        {
			Monobit.Utl.LogD("OnJoinedRoom");
		}

		[MunRPC]
		void GameStart()
		{
			bStart = true;
			bSelectMenu = false;
			if (InstantiatePrefab == null || go != null)
            {
                return;
            }
			go = MonobitNetwork.Instantiate(InstantiatePrefab.name, Vector3.zero, Quaternion.identity, 0) as GameObject;
			if (go != null)
            {
                Camera mainCamera = GameObject.FindObjectOfType<Camera>();
                mainCamera.GetComponent<Camera>().enabled = false;

				Camera camera = go.GetComponentInChildren<Camera>();
                if (camera == null)
                {
                    GameObject camObj = new GameObject();
                    camObj.name = "Camera";
                    camera = camObj.AddComponent<Camera>();
					camera.transform.parent = go.transform;
                }
                camera.transform.position = camPosition;
                camera.transform.rotation = camRotation;
            }
        }
    }

#if UNITY_EDITOR
#endif // UNITY_EDITOR
}

