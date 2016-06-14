using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using MonobitEngine.Definitions;

namespace MonobitEngine.Editor
{
    /**
     * ServerSettings のインスペクタ表示用クラス.
     */
    [CustomEditor(typeof(ServerSettings))]
    public class ServerSettingsInspector : UnityEditor.Editor
    {
        /** ServerSettins 本体. */
        ServerSettings m_Settings = null;

        /** 初期化したかどうかのフラグ. */
        bool m_Initialize = false;

        /**
         * Inspector上のGUI表示.
         */
        public override void OnInspectorGUI()
        {
#if UNITY_IPHONE
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
            m_Settings = target as ServerSettings;
            if (m_Settings == null) return;

            // 初期設定
            Init();

            // アプリケーション設定
            ApplicationSettings();

            // サーバ設定
            ServerSettings();

            // データの更新
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(m_Settings);
            }
        }

        /**
         * 初期設定.
         */
        private void Init()
        {
            if (!m_Initialize)
            {
                m_Initialize = true;
                AuthenticationCode m_SaveData = AssetDatabase.LoadAssetAtPath(MonobitNetworkSettings.GetAuthCodeSettingsPath(), (typeof(AuthenticationCode))) as AuthenticationCode;
                if (m_SaveData != null)
                {
                    MonobitNetworkSettings.MonobitServerSettings.AuthID = MonobitNetworkSettings.Decrypt(m_SaveData.saveAuthID);
                }
                AssetDatabase.Refresh();
            }
        }

        /**
         * アプリケーション設定.
         */
        private void ApplicationSettings()
        {
            // 標題の表示
            EditorGUILayout.LabelField("Application Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel = 2;
            GUILayout.Space(5);

            // 認証コードの表示
            if (m_Settings.AuthID == "") SaveAuthID();
            EditorGUILayout.LabelField("Authentication Code", m_Settings.AuthID);
            
            // ボタン表示）
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate Code"))
            {
                SaveAuthID();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = 0;
            GUILayout.Space(5);
        }

        private void SaveAuthID()
        {
            m_Settings.UpdateAuthID();
            AssetDatabase.DeleteAsset(MonobitNetworkSettings.GetAuthCodeSettingsPath());
            AuthenticationCode m_SaveData = ScriptableObject.CreateInstance<AuthenticationCode>();
            m_SaveData.saveAuthID = MonobitNetworkSettings.Encrypt(m_Settings.AuthID);
            EditorUtility.SetDirty(m_SaveData);
            AssetDatabase.CreateAsset(m_SaveData, MonobitNetworkSettings.GetAuthCodeSettingsPath());
            AssetDatabase.SaveAssets();
        }

        /**
         * サーバ設定.
         */
        private void ServerSettings()
        {
            // 標題の表示
            EditorGUILayout.LabelField("Server Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel = 2;
            GUILayout.Space(5);

            GUI.enabled = !EditorApplication.isPlaying;

            // ホストタイプの設定
            m_Settings.HostType = (ServerSettings.MunHostingOption)EditorGUILayout.EnumPopup("Host Type", m_Settings.HostType);

            // ポート番号＆アドレスの設定
            switch (m_Settings.HostType)
            {
                case MonobitEngine.ServerSettings.MunHostingOption.None:
                    {
                    }
                    break;
                case MonobitEngine.ServerSettings.MunHostingOption.MunTestServer:
                    {
                    }
                    break;
                case MonobitEngine.ServerSettings.MunHostingOption.SelfServer:
                    {
                        // アドレス・ポート番号ともに自分で設定する
                        m_Settings.SelfServerAddress = EditorGUILayout.TextField("IP Address", m_Settings.SelfServerAddress).Trim();
                        string port = EditorGUILayout.TextField("Port", m_Settings.SelfServerPort.ToString());

                        // 入力されたポート番号が正常化をチェック
                        ushort usPort = 0;
                        if (ushort.TryParse(port, out usPort) == true)
                        {
                            m_Settings.SelfServerPort = usPort;
                        }
                    }
                    break;
                case MonobitEngine.ServerSettings.MunHostingOption.OfflineMode:
                    {
                    }
                    break;
            }

            GUI.enabled = true;

            EditorGUI.indentLevel = 0;
            GUILayout.Space(5);
        }
    }
}
