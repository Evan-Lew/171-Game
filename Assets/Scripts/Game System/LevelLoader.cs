using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Slider loadingSlider;
    public List<string> MultipleScenesToLoad;
    public static LevelLoader instance;

    private void Awake()
    {
        instance = this;
        MultipleScenesToLoad.Add("GameController");
        MultipleScenesToLoad.Add("Environment");
        MultipleScenesToLoad.Add("MainMenu");
        loadMultipleLevels(MultipleScenesToLoad);
    }

    void EnableLoader()
    {
        loadingScreen.SetActive(true);
        loadingSlider.value = 0;
    }

    void DisableLoader()
    {
        MultipleScenesToLoad.Clear();
        loadingScreen.SetActive(false);
    }
    
    public void loadALevel(string sceneName)
    {
        StartCoroutine(LoadALevelAsyncChronously(sceneName));
    }

    IEnumerator LoadALevelAsyncChronously(string sceneName)
    {
        EnableLoader();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingSlider.value = progress;
            yield return null;
        }
        DisableLoader();
    }

    public void loadMultipleLevels(List<string> sceneList)
    {
        StartCoroutine(LoadMultipleLevelsAsynchronously(sceneList));
    }

    IEnumerator LoadMultipleLevelsAsynchronously(List<string> sceneList)
    {
        EnableLoader();
        AsyncOperation operation;
        float currentProgress;
        float loadCount = 0;
        foreach (string sceneToLoad in sceneList)
        {
            operation = SceneManager.LoadSceneAsync(sceneToLoad);
            while (!operation.isDone)
            {
                currentProgress = Mathf.Clamp01(operation.progress / 0.9f);
                loadingSlider.value = (loadCount + currentProgress) / sceneList.Count;
                yield return null;
            }
            loadCount++;
        }
        DisableLoader();
    }
}
