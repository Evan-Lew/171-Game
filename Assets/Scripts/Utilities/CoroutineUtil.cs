using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineUtil:MonoBehaviour
{
    public static CoroutineUtil instance;

    private void Awake()
    {
        instance = this;

    }

    public IEnumerator WaitNumSeconds(System.Action func, float secondWaited)
    {

        yield return new WaitForSeconds(secondWaited);
        func();
    }

}
