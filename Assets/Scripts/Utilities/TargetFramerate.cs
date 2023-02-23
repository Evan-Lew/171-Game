
using UnityEngine;

public class TargetFramerate : MonoBehaviour
{
    public int targetFrameRate;

    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
