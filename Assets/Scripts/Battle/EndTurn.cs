using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EndTurn : MonoBehaviour
{
    public void playerEndTurnButton()
    {
        BattleController.instance.playerEndTurn();
    }
}
