using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarmaScale : MonoBehaviour
{

    public SpriteRenderer ScaleLeft;
    public SpriteRenderer ScaleRight;
    public SpriteRenderer ScaleBar;

    private float ScaleBarXSize;
    private float ScaleYSize;
    private Vector3 ScaleBarPosition;

    // Start is called before the first frame update
    void Start()
    {
        ScaleBarXSize = ScaleBar.size.x;
        // ScaleBarXSize = 24;
        ScaleYSize = ScaleLeft.size.y;
        ScaleBarPosition = ScaleBar.transform.position;

    }

    // Takes the karma difference and rotates the scales based on it
    public void MoveScales(float karmaDiff){
        float newRotation = 10*karmaDiff;

        newRotation = newRotation > 60 ? 60 : newRotation;
        newRotation = newRotation < -60 ? -60 : newRotation;

        float newY = ScaleBarXSize * Mathf.Sin(newRotation*Mathf.PI/180);
        float newX = ScaleBarXSize * Mathf.Cos(newRotation*Mathf.PI/180);

        ScaleLeft.transform.localPosition = new Vector3(-newX, -newY-ScaleYSize, 0);
        ScaleRight.transform.localPosition = new Vector3(newX, newY-ScaleYSize, 0);

        Quaternion rotation = Quaternion.Euler(0, 0, newRotation);
        ScaleBar.transform.localRotation = rotation;
    }

    private void Update(){

    }

}
