using UnityEngine;

// This script is not used (Laihong made it)
// It makes the sprite look towards the camera
public class LookAt : MonoBehaviour
{
    public Transform LookingAtTarget;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + LookingAtTarget.forward);
    }
}
