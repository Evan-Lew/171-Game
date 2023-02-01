using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeReferenced : MonoBehaviour
{
    [TextArea]
    public string Notes = "The Object is used as reference of ";
    public List<GameObject> AsReferenceOf;

}
