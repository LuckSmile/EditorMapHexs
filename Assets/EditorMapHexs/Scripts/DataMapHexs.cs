using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuckSmile.MapHexs
{
    public class DataMapHexs : ScriptableObject
    {
        public TypesHex Type = TypesHex.SharpTop;
        [HideInInspector] public List<DataEarthHex> hexs = null;
        
        public ParametersEarthHex parametersEarthHex = null;

        public enum TypesHex
        {
            SharpTop,
            BluntTop
        }
    }
}
