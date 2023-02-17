using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;


public class Links : EditorWindow
{

    private string input_1 = "", input_2 = "", input_3 = "";
    [Serializable]
    public class superLinkGroup
    {
        public string label;
        public string theLink;
    }

    public List<superLinkGroup> myLinkGroup = new List<superLinkGroup>();

    [MenuItem("My Tools/Links")]
    static void Init()
    {
        Links window = (Links)EditorWindow.GetWindow(typeof(Links));
        window.Show();
    }

    string filePath = "/Editor/linkdata.json";


    void OnGUI()
    {
        GUILayout.Space(20);

        GUILayout.Label("Useful Links", EditorStyles.boldLabel);
        GUILayout.Space(10);


        foreach (superLinkGroup link in myLinkGroup)
        {
            if (GUILayout.Button(link.label,  GUILayout.Width(120), GUILayout.Height(30)))
            {
                Application.OpenURL(link.theLink);
            }
        }

        GUILayout.Space(10);
        GUILayout.Label("Add Links");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Label", GUILayout.Width(120));
        input_1 = GUILayout.TextField(input_1);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Link", GUILayout.Width(120));
        input_2 = GUILayout.TextField(input_2);
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Add"))
        {
            superLinkGroup temp = new superLinkGroup();
            temp.label = input_1;
            temp.theLink = input_2;
            myLinkGroup.Add(temp);

            input_1 = "";
            input_2 = "";
        }


        GUILayout.Space(10);
        GUILayout.Label("Remove Link");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Label To Remove", GUILayout.Width(120));
        input_3 = GUILayout.TextField(input_3);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Remove"))
        {
            int index = myLinkGroup.FindIndex(x => x.label == input_3);
            if (index != -1)
            {
                myLinkGroup.RemoveAt(index);
                input_3 = "";
            }
        }




        GUILayout.Space(20);
        if (GUILayout.Button(("Save"), GUILayout.Width(100)))
        {
            Save();
        }

    }



    private void Save()
    {
        string jsonData = JsonUtility.ToJson(new SerializableList<superLinkGroup>(myLinkGroup));
        File.WriteAllText(Application.dataPath + filePath, jsonData);
    }

    private void Load()
    {
        if (File.Exists(Application.dataPath + "/" + filePath))
        {
            string jsonData = File.ReadAllText(Application.dataPath + "/" + filePath);
            SerializableList<superLinkGroup> serializableList = JsonUtility.FromJson<SerializableList<superLinkGroup>>(jsonData);
            myLinkGroup = serializableList.ToList();
        }
    }

    private void OnEnable()
    {
        Load();
    }

    private void OnDestroy()
    {
        Save();
    }


    [Serializable]
    public class SerializableList<T>
    {
        public List<T> list;

        public SerializableList(List<T> list)
        {
            this.list = list;
        }

        public List<T> ToList()
        {
            return list;
        }
    }
}
