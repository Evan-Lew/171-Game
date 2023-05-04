using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BT_ResetHealth : MonoBehaviour
{
     [SerializeField] Character_Basedata current_player;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(BattleController.battleNum == 100)
        {
            current_player.Health_Total = 35;
            current_player.Health_Current = 35;
        }
    }
}
