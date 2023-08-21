using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Rowcol {
    public int row;
    public int column;
    public Rowcol(int r = 0, int c = 0) {
        row = r;
        column = c;
    }
}

public class CustomGrid<T> where T : class {
    public int Width { get; set; }
    public int Height { get; set; }
    public Vector3 OriginPosition { get; set; }
    private T[,] _gridArray;
    private float _offsetX;
    private float _offsetY;

    public CustomGrid(int width, int height, Vector3 origin, float offsetX, float offsetY) {
        Width = width;
        Height = height;
        OriginPosition = origin;

        _offsetX = offsetX;
        _offsetY = offsetY;

        _gridArray = new T[height, width];
    }

    public T GetElement(int row, int column) {
        return _gridArray[row, column];
    }

    public T GetElement(Rowcol rowcol) {
        return GetElement(rowcol.row, rowcol.column);
    }

    public void SetElement(int row, int column, T value) {
        _gridArray[row, column] = value;
    }

    public Vector3 RowcolToPoint(int row, int column) {
        float x = column * _offsetX;
        float y = row * _offsetY;
        return OriginPosition + new Vector3(x, y);
    }

    public List<int> GetAdjustNode(int row, int column, bool ascending) {
        List<int> colList = new List<int>();
        for (int x = column - 1; x <= column + 1; ++x) {
            if (x < 0 || x >= Width) continue;

            int nextRow = (ascending ? row + 1 : row - 1);
            if (GetElement(nextRow, x) != null) {
                colList.Add(x);
            }
        }
        return colList;
    }
}
