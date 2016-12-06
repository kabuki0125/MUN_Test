using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using MonobitEngine;

public class NetworkControl : MonobitEngine.MonoBehaviour
{
    
    /// <summary>
    /// MonobitEngine.MonoBehaviour定義 : サーバ接続失敗コールバック.
    /// </summary>
    public void OnConnectToServerFailed(object parameters)
    {
        Debug.Log("OnConnectToServerFailed : StatusCode = " + parameters);
        m_bConnectFailed = true;
        m_bDisplayWindow = true;
    }
    
    /// <summary>
    /// MonobitEngine.MonoBehaviour定義 : 途中切断コールバック.
    /// </summary>
    public void OnDisconnectedFromServer()
    {
        Debug.Log("OnDisconnectedFromServer");
        if( m_bDisconnect == false ){
            m_bDisconnect = true;
            m_bDisplayWindow = true;
        }else{
            m_bDisconnect = false;
        }
    }
    
    /// <summary>
    /// 部屋作成時のコールバック.
    /// </summary>
    public void OnCreateRoom()
    {
        Debug.Log("Create Room Success!!");
    }
    /// <summary>
    /// 部屋作成失敗時のコールバック.
    /// </summary>
    public void OnCreateRoomFailed()
    {
        Debug.Log("Create Room Failed....");
    }

    // 初期化
    void Start()
    {
        // デフォルトロビーへの自動入室を許可する
        MonobitNetwork.autoJoinLobby = true;

        // MUNサーバに接続する
        MonobitNetwork.ConnectServer("Bearpocalypse_v1.0");
    }
    
    // 更新
    void Update()
    {
        // MUNサーバに接続しており、かつルームに入室している場合
        if (MonobitNetwork.isConnect && MonobitNetwork.inRoom){
            // プレイヤーキャラクタが未登場の場合に登場させる
            if (m_playerObject == null){
                m_playerObject = MonobitNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
            }
        }
    }
    
    // GUI制御
    void OnGUI()
    {
        // サーバ接続状況に応じて、ウィンドウを表示する
        if(m_bDisplayWindow){
            GUILayout.Window(0, new Rect(Screen.width / 2 - 100, Screen.height / 2 - 40, 200, 80), WindowControl, "Caution");
        }
        
        // デフォルトのボタンと被らないように、段下げを行なう。
        GUILayout.Space(24);

        // MUNサーバに接続している場合
        if( MonobitNetwork.isConnect ){
            
            // ボタン入力でサーバから切断＆シーンリセット
            if( GUILayout.Button("Disconnect", GUILayout.Width(150))){
                m_bDisconnect = true; // 正常動作のため、bDisconnect を true にして、GUIウィンドウ表示をキャンセルする
                MonobitNetwork.DisconnectServer();                                                  // サーバから切断
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single); // シーンをリロード
            }
            
            // ルームに入室している場合
            if( MonobitNetwork.inRoom ){
                // ボタン入力でルームから退室
                if (GUILayout.Button("Leave Room", GUILayout.Width(150))){
                    MonobitNetwork.LeaveRoom();
                }   
            }
            // ルームに入室していない場合
            else {
                GUILayout.BeginHorizontal();
                // ルーム名の入力
                GUILayout.Label("RoomName : ");
                m_roomName = GUILayout.TextField(m_roomName, GUILayout.Width(200));
                // ボタン入力でルーム作成
                if ( GUILayout.Button("Create Room", GUILayout.Width(150))){
                    MonobitNetwork.CreateRoom(m_roomName);
                }
                GUILayout.EndHorizontal();
                
                // 現在存在するルームからランダムに入室する
                if( GUILayout.Button("Join Random Room", GUILayout.Width(200)) ){
                    MonobitNetwork.JoinRandomRoom();
                }                
                // ルーム一覧から選択式で入室する
                foreach (RoomData room in MonobitNetwork.GetRoomData()){
                    if (GUILayout.Button("Enter Room : " + room.name + "(" + room.playerCount + "/" + ((room.maxPlayers == 0) ? "-" : room.maxPlayers.ToString()) + ")")){
                        MonobitNetwork.JoinRoom(room.name);
                    }
                } 
            }
            
        }
    }
    
    // ウィンドウ表示用メソッド
    private void WindowControl(int windowId)
    {
        // GUIスタイル設定
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        GUIStyleState stylestate = new GUIStyleState();
        stylestate.textColor = Color.white;
        style.normal = stylestate;

        // 途中切断時の表示
        if ( m_bDisconnect )
        {
            GUILayout.Label("途中切断しました。\n再接続しますか？", style);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("はい", GUILayout.Width(50))){
                // もう一度接続処理を実行する
                MonobitNetwork.ConnectServer("Bearpocalypse_v1.0");
                m_bDisconnect = false;
                m_bDisplayWindow = false;
            }
            if (GUILayout.Button("いいえ", GUILayout.Width(50))){
                // シーンをリロードし、初期化する
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Additive);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return;
        }
        
        // 接続失敗時の表示
        if ( m_bConnectFailed )
        {
            GUILayout.Label("接続に失敗しました。\n再接続しますか？", style);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("はい", GUILayout.Width(50))){
                // もう一度接続処理を実行する
                MonobitNetwork.ConnectServer("Bearpocalypse_v1.0");
                m_bConnectFailed = false;
                m_bDisplayWindow = false;
            }
            if (GUILayout.Button("いいえ", GUILayout.Width(50))){
                // オフラインモードで起動する
                MonobitNetwork.offline = true;
                m_bConnectFailed = false;
                m_bDisplayWindow = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return;
        }
    }
    
    private string m_roomName = "room_name";    // ルーム名
    private GameObject m_playerObject = null;   // プレイヤーキャラクタ
    private bool m_bDisplayWindow = false;      // ウィンドウ表示フラグ
    private bool m_bConnectFailed = false;      // サーバ接続失敗フラグ
    private static bool m_bDisconnect = false;    // サーバ途中切断フラグ
}
