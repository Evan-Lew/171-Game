using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = targetObject.transform.position;
    }
}
