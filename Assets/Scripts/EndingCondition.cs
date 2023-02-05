using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCondition : MonoBehaviour
{
    [SerializeField] Character player, enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if (player.Health_Current <= 0 || enemy.Health_Current <= 0)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}
