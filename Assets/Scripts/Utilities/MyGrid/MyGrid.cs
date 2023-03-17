using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid
{
    Vector3 initPos;
    int width, height;
    float cellSize;
    public int[,] gridArray;

    public MyGrid(Vector3 startingPoint, int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.initPos = startingPoint;
        this.gridArray = new int[width, height];
    }


    public Vector3 GetPosFromRowColumn(int row, int column)
    {
        return new Vector3(column, row) * cellSize + initPos;
    }


    public Vector3 GetWorldPosition(int x, int y, Vector3 startingPoint)
    {
        return new Vector3(x, y) * cellSize + startingPoint;
    }


    private void GetRowColumnFromWorldPosition(Vector3 worldPos, out int row, out int column)
    {
        row = Mathf.FloorToInt(worldPos.y / cellSize);
        column = Mathf.FloorToInt(worldPos.x / cellSize);
    }
}
