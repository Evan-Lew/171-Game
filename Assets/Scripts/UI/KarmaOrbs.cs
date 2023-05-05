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

    void Start()
    {
        scalePos = scale.position;
    }

    void Update(){
        scalePos = scale.position;
        transform.position = Vector3.SmoothDamp(transform.position, scalePos, ref velocity, smooth);
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
