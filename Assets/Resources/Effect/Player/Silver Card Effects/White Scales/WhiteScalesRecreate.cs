using System.Collections.Generic;
using UnityEngine;

public class WhiteScalesRecreate : MonoBehaviour
{
    [SerializeField] GameObject HolyShield;
    [SerializeField] GameObject DestroyedShield;
    GameObject ShieldObj;


    private void OnDisable()
    {
        Destroy(ShieldObj);
    }


    private void OnEnable()
    {
        if (HolyShield.transform.Find("Effect_09_Shield_2"))
        {
            Destroy(HolyShield.transform.Find("Effect_09_Shield_2").gameObject);
        }
        ShieldObj = Instantiate(DestroyedShield, HolyShield.transform);
    }

}
