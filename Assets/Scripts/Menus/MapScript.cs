using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScript : MonoBehaviour
{
    public GameObject scroll;
    public GameObject button;

    public void ShowBox()
    {
        scroll.SetActive(true);
        button.SetActive(false);
        
        
    }


} 