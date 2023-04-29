using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarmaScale : MonoBehaviour
{

    public SpriteRenderer ScaleLeft;
    public SpriteRenderer ScaleRight;
    public SpriteRenderer ScaleBar;
    public Transform leftPivot;
    public Transform rightPivot;

    private float zRotation = 0.0f;

    private float ScaleBarXSize;
    private float ScaleYSize;
    private Vector3 ScaleBarPosition;
    private float testRotates = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        ScaleBarXSize = ScaleBar.size.x;
        ScaleYSize = ScaleLeft.size.y;
        ScaleBarPosition = ScaleBar.transform.position;

        // ScaleLeft.transform.localPosition = new Vector3(-ScaleBarXSize/2, -ScaleYSize/2,0);
        // ScaleRight.transform.localPosition = new Vector3(ScaleBarXSize/2, -ScaleYSize/2,0);

    }

    // Takes the karma difference and rotates the scales based on it
    public void MoveScales(float karmaDiff){
        float newRotation = 10*karmaDiff;
        Matrix4x4 barToWorldMatrix = ScaleBar.localToWorldMatrix;

        
        // Min and Max rotations are 60 and -60 degrees
        // newRotation = newRotation > 60 ? 60 : newRotation;
        // newRotation = newRotation < -60 ? -60 : newRotation;

        zRotation = newRotation;

        float newX = ScaleBarXSize * Mathf.Sin(newRotation);
        float newY = ScaleBarXSize * Mathf.Cos(newRotation);

        // ScaleLeft.transform.localPosition = new Vector3(newX/2, -newY, 0);
        // ScaleRight.transform.localPosition = new Vector3(-newX/2, newY, 0);

        Quaternion rotation = Quaternion.Euler(0, 0, zRotation);
        Quaternion counterRotation = Quaternion.Euler(0, 0, -zRotation);
        ScaleBar.transform.localRotation = rotation;
        // ScaleLeft.transform.rotation = Quaternion.Euler(0, 0, 0);
        // ScaleRight.transform.rotation = Quaternion.Euler(0, 0, 0);
        // ScaleLeft.transform.position = leftPivot.position - new Vector3(0, 0.001f, 0);
        // ScaleRight.transform.position = rightPivot.position - new Vector3(0, 0.001f, 0);


    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.W)){
            testRotates += 1.0f;
            MoveScales(testRotates);
        }
    }

}
