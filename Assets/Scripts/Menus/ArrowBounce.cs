using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBounce : MonoBehaviour
{
    public float min=2f;
    public float max=3f;
    public GameObject WP1;
    public GameObject WP2;
    public GameObject WP3;
    public GameObject WP4;

    void Start () {
        if (BattleController.battleNum == 1)   //waterfall
        { 
            transform.position = WP1.transform.position;  
        }
        if (BattleController.battleNum == 2)   //temple
        {
            transform.position = WP2.transform.position;
        }
        if (BattleController.battleNum == 3) //grove
        {
            transform.position = WP3.transform.position;
        }
        if (BattleController.battleNum == 4) //shrine
        {
            transform.position = WP4.transform.position;
        }
        min=transform.position.x;
        max=transform.position.x+4;
    }
    void Update () {
        transform.position =new Vector3(Mathf.PingPong(Time.time*8,max-min)+min, transform.position.y, transform.position.z);
    }
}