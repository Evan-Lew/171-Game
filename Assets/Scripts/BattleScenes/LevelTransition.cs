using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public bool optionSelected = false;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (optionSelected)
        {
            SceneManager.LoadScene("MountainChallenge");
            optionSelected = false;
        }
    }
}
