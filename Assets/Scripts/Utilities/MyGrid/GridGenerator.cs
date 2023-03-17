using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] Transform generateLocation;
    [SerializeField] int  dimensionRow, dimensionColumn;
    [SerializeField] float cellDistance;
    [SerializeField] bool enableDebugBox;

    MyGrid myGrid;
    // Start is called before the first frame update
    void Start()
    {
        //turn off gizmos for gameview performance for tesing
        enableDebugBox = false;
        myGrid = new MyGrid(generateLocation.position, dimensionColumn, dimensionRow, cellDistance);
        
    }


    private void OnDrawGizmos()
    {
        if (enableDebugBox)
        {
            for (int x = 0; x < dimensionColumn; x++)
            {
                for (int y = 0; y < dimensionRow; y++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(new Vector3 (x,y) * cellDistance + generateLocation.position, 5f);
                }
            }
        }
    }

}
