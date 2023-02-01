using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTest : MonoBehaviour
{
    [SerializeField] private Character player;
    [SerializeField] private Character enemy;

    public void Trigger()
    {
        player.Health_Current -= 5;
        enemy.Priority_Current += 2;

    }
}
