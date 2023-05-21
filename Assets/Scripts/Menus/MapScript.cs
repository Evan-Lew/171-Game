using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScript : MonoBehaviour
{
    public GameObject scroll;
    public GameObject button;
    public GameObject deckbutton;
    
    public void ShowBox()
    {
        scroll.SetActive(true);
        button.SetActive(false);
    }
    public void Start()
    {
        deckbutton.SetActive(false);
    }
    public void Update()
    {
        if (BattleController.battleNum > 0)
                {
                    button.SetActive(false);
                    deckbutton.SetActive(true);
                }
    }

} 