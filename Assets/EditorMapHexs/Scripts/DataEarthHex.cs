using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] public class DataEarthHex
{
    public Vector2Int Index => index;
    [SerializeField] private Vector2Int index = Vector2Int.zero;
    public List<DataEarthHex> childs = null;
    public DataEarthHex(Vector2Int index)
    {
        this.index = index;
        this.childs = new List<DataEarthHex>();
    }
}