using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraUtil : MonoBehaviour
{
    public GameObject UIBattle, UICamp;
    public Camera BattleCameraObj, CampCameraObj, MainCameraObj;
    [SerializeField] EventSystem EventSystem_Current;

    public void SetCameraActive(Camera CameraToSet, bool isActive)
    {
        //to avoid re-rendering camera that covers the main camera
        MainCameraObj.gameObject.SetActive(false);
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
            //else if (CameraToSet.gameObject.name == "Character Camera")
            //{
            //    CameraToSet.gameObject.SetActive(isActive);
            //}
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
            //else if (CameraToSet.gameObject.name == "Character Camera")
            //{
            //    CameraToSet.gameObject.SetActive(isActive);
            //}
        }
        //re-rendering main after render the camera
        MainCameraObj.gameObject.SetActive(true);
    }

    //override Function that takes Camera
    public void SetUIActive(Camera CameraRelatedToUI, bool isActive)
    {
        if (isActive)
        {
            if (CameraRelatedToUI.gameObject.name == "UI Camp Camera")
            {
                UICamp.SetActive(true);
            }
            else if (CameraRelatedToUI.gameObject.name == "UI Battle Camera")
            {
                UIBattle.SetActive(true);
            }
        }
        else
        {
            if (CameraRelatedToUI.gameObject.name == "UI Camp Camera")
            {
                UICamp.SetActive(false);
            }
            else if (CameraRelatedToUI.gameObject.name == "UI Battle Camera")
            {
                UIBattle.SetActive(false);
            }
        }
    }

    public void SetUIActive(GameObject UI, bool isActive)
    {
        if (isActive)
        {
            if (UI.name == "UI Camp")
            {
                UICamp.SetActive(true);
            }
            else if (UI.name == "UI Battle")
            {
                UIBattle.SetActive(true);
            }
        }
        else
        {
            if (UI.name == "UI Camp")
            {
                UICamp.SetActive(false);
            }
            else if (UI.name == "UI Battle")
            {
                UIBattle.SetActive(false);
            }
        }
    }

}
