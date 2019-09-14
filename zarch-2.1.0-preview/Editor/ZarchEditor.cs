using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Z
{
    public class ZarchEditor : Editor
    {
        //[MenuItem("Tools/Zarch/Init")]
        public static void CreateZarchConnectorObject()
        {
            if (FindObjectOfType<ZarchUnity3DConnector>())
                return;
            var h = new GameObject("ZarchUnity3DConnector").AddComponent<ZarchUnity3DConnector>();
            h.gameObject.AddComponent<ThreadBridge>();
            h.gameObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
            h.gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        [MenuItem("Tools/Zarch/Zarch Utility Panel")]
        public static void ZarchPanelWindow() { EditorWindow.GetWindow<ZarchPanel>(); }


        //[MenuItem("Tools/Zarch/GitHub")]
        public static void JumpToGithub() { Application.OpenURL("https://github.com/dastudio/zarch"); }
    }

    public class ZarchPanel : EditorWindow
    {
        ZarchUnity3DConnector connector;

        public ZarchPanel() { titleContent = new GUIContent("Zarch"); }


        void OnGUI()
        {
            GUILayout.Space(10);

            GUILayout.Label("Console Output:");

            GUILayout.BeginVertical("box");


            GUILayout.BeginHorizontal();

            if (connector == null)
                connector = FindObjectOfType<ZarchUnity3DConnector>();

            if (connector == null)
            {
                GUI.color = Color.red;
                if (GUILayout.Button("init")) { ZarchEditor.CreateZarchConnectorObject(); }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                return;
            }

            GUI.color = Color.white;

            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();

            //GUILayout.Label(new GUIContent("Console Text Component:"));

            connector.console = (UnityEngine.UI.Text)EditorGUILayout.ObjectField("Console",connector.console, typeof(UnityEngine.UI.Text), true, null);

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.Label(new GUIContent("Prefabs:"));

            GUILayout.BeginVertical("box");

            for (int i = 0; i < connector.prefabs.Count; i++)
            {
                GameObject obj = (GameObject)EditorGUILayout.ObjectField(connector.prefabs[i],typeof(GameObject),false,null);
                if (obj == connector.prefabs[i])
                    continue;
                connector.prefabs[i] = obj;

            }
            
            if (GUILayout.Button("+")) { connector.prefabs.Add(connector.gameObject); }

            GUILayout.EndVertical();

            GUILayout.Space(10);


            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();

            GUILayout.Label("Frame Rate:");

            GUILayout.Label(Application.targetFrameRate.ToString());

            GUILayout.EndHorizontal();


            Application.targetFrameRate = System.Convert.ToInt32(GUILayout.HorizontalSlider(Application.targetFrameRate, -1, 120));

            if (Application.targetFrameRate != -1)
                QualitySettings.vSyncCount = 0;

            if (GUILayout.Button("GitHub")) { ZarchEditor.JumpToGithub(); }

            GUILayout.EndVertical();
        }


    }

}