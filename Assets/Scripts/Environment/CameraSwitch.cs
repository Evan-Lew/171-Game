using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{

    [SerializeField] Camera environmentCam;
    public int camPosIndex;

    //public List<GameObject> cameraSpawningPositions = new List<GameObject>();
    public List<Transform> cameraSpawningPositions = new List<Transform>();


    // Update is called once per frame
    void Update()
    {

        //for camera testing

        //press M, then environment cam will be set the postion of cam at index in your list
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (camPosIndex < cameraSpawningPositions.Count - 1) {
                camPosIndex += 1;
            } else {
                camPosIndex = 0;
            }

        }
        environmentCam.transform.position = cameraSpawningPositions[camPosIndex].position;
        environmentCam.transform.rotation = cameraSpawningPositions[camPosIndex].rotation;

    }

}
