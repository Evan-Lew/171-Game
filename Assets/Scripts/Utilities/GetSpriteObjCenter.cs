using UnityEngine;

public class GetSpriteObjCenter : MonoBehaviour
{

    public Vector3 center;
    public Vector3 bottom;

    // Update is called once per frame
    void Update()
    {
        center = GetComponent<SpriteRenderer>().bounds.center;
        bottom = GetComponent<SpriteRenderer>().bounds.min;
        bottom.x = center.x;
        bottom.z = center.z;
    }
}
