using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarmaOrbs : MonoBehaviour
{

    // WIP

    public Transform scale;
    private Vector3 scalePos;
    private Vector3 velocity = Vector3.zero;

    public float smooth = 0.5f;
    public float speed = 1.0f;

    private Vector3 bob = new Vector3(0.0f, 0.5f, 0.0f);
    private Vector3 negBob = new Vector3(0.0f, -0.5f, 0.0f);
    private int bobTimer = 0;
    private bool bobbingUp = true;

    void Start()
    {
        scalePos = scale.position;
    }

    void Update(){
        scalePos = scale.position;
        if(bobbingUp)
            transform.position = Vector3.SmoothDamp(transform.position, scalePos + bob, ref velocity, smooth);
        else
            transform.position = Vector3.SmoothDamp(transform.position, scalePos + negBob, ref velocity, smooth);


        bobTimer += 1;

        if (bobTimer > 20){
            bobbingUp = !bobbingUp;
        }

    }

    // // Update is called once per frame
    // void Update()
    // {
    //     scalePos = scale.position;
    //     smoothRotation(scalePos);
    //     transform.Translate(transform.forward * speed);
    // }

    // void smoothRotation(Vector3 target){
    //     Vector3 direction = target - transform.position;
    //     Quaternion targetRotation = Quaternion.LookRotation(direction);
    //     transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smooth);
    // }

}
