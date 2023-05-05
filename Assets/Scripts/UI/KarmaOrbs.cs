using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarmaOrbs : MonoBehaviour
{

    public Transform scale;
    private Vector3 scalePos;

    public float smooth = 0.5f;
    public float speed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        scalePos = scale.position;
    }

    // Update is called once per frame
    void Update()
    {
        scalePos = scale.position;
        smoothRotation(scalePos);
        transform.Translate(transform.forward * speed);
    }

    void smoothRotation(Vector3 target){
        Vector3 direction = target - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smooth);
    }

}
