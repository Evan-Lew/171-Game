using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform LookingAtTarget;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + LookingAtTarget.forward);
    }
}
