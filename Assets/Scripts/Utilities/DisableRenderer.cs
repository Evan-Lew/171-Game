using UnityEngine;

public class DisableRenderer : MonoBehaviour
{

    void Start()
    {
        DisableObjectRenderer();
    }

  
    void DisableObjectRenderer()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }
}
