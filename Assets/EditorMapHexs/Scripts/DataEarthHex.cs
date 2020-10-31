using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataEarthHex
{
    public Vector2Int Index { get; private set; }
    public List<DataEarthHex> Childs { get; private set; }
    public DataEarthHex(Vector2Int index)
    {
        this.Index = index;
        this.Childs = new List<DataEarthHex>();
    }
}
