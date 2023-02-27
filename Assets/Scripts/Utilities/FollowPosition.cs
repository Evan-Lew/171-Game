using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [Tooltip("special only works for object that has different origin in sprite renderer")]
    [SerializeField] private enum spritePos {center, bottom, ObjPosition}
    [SerializeField] private spritePos targetPoint;
    // Update is called once per frame
    void Update()
    {

        if (targetPoint == spritePos.center)
        {
            this.gameObject.transform.position = targetObject.GetComponent<GetSpriteObjCenter>().center;
        }
        else if (targetPoint == spritePos.bottom)
        {
            this.gameObject.transform.position = targetObject.GetComponent<GetSpriteObjCenter>().bottom;
        }
        else
        {
            this.gameObject.transform.position = targetObject.transform.position;
        }
     
    }
}
