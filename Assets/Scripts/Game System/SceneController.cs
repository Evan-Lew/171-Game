using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public int sceneCount;
    int inc, inc2;
    public AsyncOperation Operation1;
    public AsyncOperation Operation2;
    public List<AsyncOperation> Operation = new List<AsyncOperation>();

    private void Awake()
    {

    }


    void Start()
    {
        sceneCount = SceneManager.sceneCountInBuildSettings;
        inc = 1;
        inc2 = 1;
        //avoid index 0, set to null
        AsyncOperation temp = new AsyncOperation();
        Operation.Add(temp);


        //load all scenes
        for (int i = 1; i < sceneCount; i++)
        {
            temp = new AsyncOperation();
            temp = SceneManager.LoadSceneAsync(i, LoadSceneMode.Additive);
            Operation.Add(temp);
            Operation[i].allowSceneActivation = false;
           
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex(inc2), UnloadSceneOptions.None);
           
            Operation[inc2].allowSceneActivation = true;
            inc2++;
        }

        //enable one scene at a time
        if (Input.GetKeyDown(KeyCode.D))
        {
            
            Operation[inc].allowSceneActivation = true;
            inc++;
        }
    }
}
