using UnityEditor;
using UnityEngine;

public class Randomiser : EditorWindow
{
    bool randomX, randomY, randomZ;
    bool randomScale;
    float minScale, maxScale;

    [MenuItem("My Tools/Randomiser")]

    static void Init()
    {
        Randomiser window = (Randomiser)GetWindow(typeof(Randomiser));
        window.Show();
    }

    private void OnGUI()
    {

        GUILayout.Label("Randomise selected Objects", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Rotation");
        randomX = EditorGUILayout.Toggle("Randomise X", randomX);
        randomY = EditorGUILayout.Toggle("Randomise Y", randomY);
        randomZ = EditorGUILayout.Toggle("Randomise Z", randomZ);

        if (GUILayout.Button("Randomise Rotation"))
        {
            foreach(GameObject obj in Selection.gameObjects)
            {
                obj.transform.rotation = Quaternion.Euler(GetRandomRotation(obj.transform.rotation.eulerAngles));
            }
        }


        GUILayout.Space(10);
        GUILayout.Label("Scale");
        minScale = EditorGUILayout.FloatField("Min Scale", minScale);
        maxScale = EditorGUILayout.FloatField("Max Scale", maxScale);

        if (GUILayout.Button("Randomise Scale"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                float setScale = Random.Range(minScale, maxScale);
                obj.transform.localScale = new Vector3(setScale, setScale, setScale);
            }
        }
    }

    private Vector3 GetRandomRotation(Vector3 currentRotation)
    {
        float x = randomX ? Random.Range(0f, 360f) : currentRotation.x;
        float y = randomY ? Random.Range(0f, 360f) : currentRotation.y;
        float z = randomZ ? Random.Range(0f, 360f) : currentRotation.z;
        return new Vector3(x, y, z);
    }
}
