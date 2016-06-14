using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MonobitEngine;

/// <summary>
/// サンプル用：チャットスクリプト
/// </summary>
public class ChatScript : MonobitEngine.MonoBehaviour
{

    /// <summary>
    /// RPC 受信関数.
    /// </summary>
    [MunRPC]
    void RecvChat(string senderName, string senderWord)
    {
        m_chatLog.Add(senderName + " : " + senderWord);
        if( m_chatLog.Count > 10 ){
            m_chatLog.RemoveAt(0);
        }
    }
    
    // GUI制御
    void OnGUI()
    {
        // MUNサーバに接続している場合
        if( MonobitNetwork.isConnect ){
            // ルームに入室している場合
            if ( MonobitNetwork.inRoom ){
                // ルーム内のプレイヤー一覧の表示
                GUILayout.BeginHorizontal();
                GUILayout.Label("PlayerList : ");
                foreach(MonobitPlayer player in MonobitNetwork.playerList){
                    GUILayout.Label(player.name + " ");
                }
                GUILayout.EndHorizontal();
                
                // ルームからの退室
                if (GUILayout.Button("Leave Room", GUILayout.Width(150))){
                    MonobitNetwork.LeaveRoom();
                    m_chatLog.Clear();
                }
                
                // チャット発言文の入力
                GUILayout.BeginHorizontal();
                GUILayout.Label("Message : ");
                m_chatWord = GUILayout.TextField(m_chatWord, GUILayout.Width(400));
                GUILayout.EndHorizontal();
                
                // チャット発言文を送信する
                if(GUILayout.Button("Send", GUILayout.Width(100))){
                    monobitView.RPC("RecvChat", MonobitTargets.All, MonobitNetwork.playerName, m_chatWord);
                    m_chatWord = "";
                }   
                
                // チャットログを表示する
                string msg = "";
                for(int i = 0; i < 10; ++i ){
                    msg += ((i < m_chatLog.Count) ? m_chatLog[i] : "") + "\r\n";
                }
                GUILayout.TextArea(msg);
            }
            // ルームに入室していない場合
            else{
                // ルーム名の入力
                GUILayout.BeginHorizontal();
                GUILayout.Label("RoomName : ");
                m_roomName = GUILayout.TextField(m_roomName, GUILayout.Width(200));
                GUILayout.EndHorizontal();
                
                // ルームを作成して入室する
                if (GUILayout.Button("Create Room", GUILayout.Width(150))){
                    MonobitNetwork.CreateRoom(m_roomName);
                }
                
                // ルーム一覧を検索
                foreach( RoomData room in MonobitNetwork.GetRoomData()){
                    // ルームを選択して入室する
                    if (GUILayout.Button("Enter Room : " + room.name + "(" + room.playerCount + "/" + ((room.maxPlayers == 0) ? "-" : room.maxPlayers.ToString()) + ")")){
                        MonobitNetwork.JoinRoom(room.name);
                    }
                }
            }
        }
        // MUNサーバに接続していない場合
        else{
            // プレイヤー名の入力
            GUILayout.BeginHorizontal();
            GUILayout.Label("PlayerName : ");
            MonobitNetwork.playerName = GUILayout.TextField((MonobitNetwork.playerName == null) ? "": MonobitNetwork.playerName, GUILayout.Width(200));
            GUILayout.EndHorizontal();
            
            // デフォルトロビーへの自動入室を許可する
            MonobitNetwork.autoJoinLobby = true;
            
            // MUNサーバに接続する
            if( GUILayout.Button("Connect Server", GUILayout.Width(150))){
                MonobitNetwork.ConnectServer("SimpleChat_v1.0");
            }
        }
    }
    
    private string m_roomName = "";   // ルーム名
    private string m_chatWord = "";   // チャット発言文
    private List<string> m_chatLog = new List<string>();  // チャット発言ログ
}
