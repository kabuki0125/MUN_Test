using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MonobitEngine;

/// <summary>
/// サンプル用：チャットスクリプト
/// </summary>
public class ChatScriptForGame : MonobitEngine.MonoBehaviour
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
        // 自動接続スクリプトのボタンと被らないようにする
        GUILayout.Space(24);
        
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
               // ※自動接続スクリプトがやるので入室処理はカット
            }
        }
        // MUNサーバに接続していない場合
        else{
            // プレイヤー名の入力
            GUILayout.BeginHorizontal();
            GUILayout.Label("PlayerName : ");
            MonobitNetwork.playerName = GUILayout.TextField((MonobitNetwork.playerName == null) ? "": MonobitNetwork.playerName, GUILayout.Width(200));
            GUILayout.EndHorizontal();

            // ※ロビーへの入室管理も自動接続スクリプトがやってくれるのでカット
        }
    }

    private string m_chatWord = "";   // チャット発言文
    private List<string> m_chatLog = new List<string>();  // チャット発言ログ
}
