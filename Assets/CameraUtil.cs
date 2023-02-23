using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtil : MonoBehaviour
{
    public GameObject UIBattle, UICamp;
    public Camera BattleCameraObj, CampCameraObj;

    public void SetCameraActive(Camera CameraToSet, bool isActive)
    {
        if (isActive)
        {
            if(CameraToSet.gameObject.name == "UI Camp Camera")
            {
                CameraToSet.gameObject.SetActive(isActive);
                UICamp.SetActive(true);
            }else if(CameraToSet.gameObject.name == "UI Battle Camera")
            {
                CameraToSet.gameObject.SetActive(isActive);
                UIBattle.SetActive(true);
            }
        }
        else
        {
            if (CameraToSet.gameObject.name == "UI Camp Camera")
            {
                CameraToSet.gameObject.SetActive(isActive);
                UICamp.SetActive(false);
            }
            else if (CameraToSet.gameObject.name == "UI Battle Camera")
            {
                CameraToSet.gameObject.SetActive(isActive);
                UIBattle.SetActive(false);
            }
        }
    
    }


}
