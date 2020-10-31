using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LuckSmile.EditorMapHexs
{
    public class DataMapHexs
    {
        public EarthHex[] Hexs => hexs.Values.ToArray();
        public readonly ParametersEarthHex parametersEarthHex = null;
        private readonly Dictionary<Vector2, EarthHex> hexs = null;
        private readonly Vector2 sizeCell = Vector2.zero;
        private readonly TypesHex type = TypesHex.SharpTop;
        public DataMapHexs(TypesHex type, ParametersEarthHex parametersEarthHex, Vector2 sizeWindow, Vector2 sizeCell)
        {
            this.parametersEarthHex = parametersEarthHex;

            this.type = type;
            this.sizeCell = sizeCell;
            this.hexs = new Dictionary<Vector2, EarthHex>();

            EarthHex hex = CreateHex(Vector2.zero, EarthHex.Types.Base);
            hex.RectPosition.position = (sizeWindow - sizeCell) / 2f;
            hexs.Add(hex.Data.Index, hex);
        }
        private EarthHex CreateHex(Vector2 index, EarthHex.Types type)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = type == EarthHex.Types.Pointer ? parametersEarthHex.TextureHexPointer : parametersEarthHex.TextureHexBase;
            style.border = new RectOffset(4, 4, 4, 4);

            DataEarthHex dataEarthHex = new DataEarthHex(new Vector2Int((int)index.x, (int)index.y));

            EarthHex hex = new EarthHex(dataEarthHex, type, new EarthHex.OnEvents(null, () => RemoveHex(index), ClearPointers), this, style)
            {
                RectPosition = new Rect(Vector2.zero, sizeCell)
            };// new EarthHex.Data(index, type), style);
            
            return hex;
        }
        private EarthHex CreatePointer(Vector2 index, Vector2 direction, EarthHex parent)
        {
            if (hexs.Keys.Contains(index) == true)
                return null;
            
            EarthHex hex = CreateHex(index, EarthHex.Types.Pointer);

            Vector2 position = parent == null ? Vector2.zero : parent.RectPosition.position;
            position += direction * sizeCell;
            hex.RectPosition = new Rect(position, sizeCell);
            hex.parent = parent;
            return hex;
        }
        public void RemoveHex(Vector2 index)
        {
            ClearPointers();
            void Remove(EarthHex hex)
            {
                hexs.Remove(hex.Data.Index);
                EarthHex[] childs = hex.Childs;
                for(int indexChild = 0; indexChild < childs.Length; indexChild++)
                {
                    Remove(childs[indexChild]);
                }
            }
            hexs[index].parent.RemoveChild(hexs[index]);
            Remove(hexs[index]);
        }
        public void ClearPointers()
        {
            EarthHex[] hexs = this.hexs.Values.ToArray();
            for (int index = 0; index < hexs.Length; index++)
            {
                EarthHex hex = hexs[index];
                if (hex.Type == EarthHex.Types.Pointer)
                {
                    this.hexs.Remove(hex.Data.Index);
                }
            }
        }
        public void ReproductionPointers(EarthHex hex)
        {
            ClearPointers();

            int[] direction = new int[] { 1, 0, -1 };
            Dictionary<Vector2, Vector2> directionPoint = new Dictionary<Vector2, Vector2>
            {
                { new Vector2(0, 1), new Vector2(0, 1.4f) },
                { new Vector2(0, -1), new Vector2(0, -1.4f) },
                { new Vector2(1, -1), new Vector2(1.1f, -0.5f * 1.4f ) },
                { new Vector2(-1, 1), new Vector2(-1.1f, 0.5f * 1.4f) },
                { new Vector2(1, 1), new Vector2(1.1f, 0.5f * 1.4f) },
                { new Vector2(-1, -1), new Vector2(-1.1f, -0.5f * 1.4f) }
            };
            for (int x = 0; x < direction.Length; x++)
            {
                for (int y = 0; y < direction.Length; y++)
                {
                    if (direction[x] != 0 && direction[y] == 0)
                        continue;
                    if (direction[x] == 0 && direction[y] == 0)
                        continue;
                    if(type == TypesHex.SharpTop)
                    {
                        Vector2 index = new Vector2(direction[x], direction[y]);
                        index.y *= index.x == 0 ? 2 : 1;
                        EarthHex pointer = this.CreatePointer(hex.Data.Index - index, directionPoint[new Vector2(direction[x], direction[y])], hex);
                        if(pointer != null)
                        {
                            hexs.Add(pointer.Data.Index, pointer);
                        }
                    }
                }
            }
        }
        public enum TypesHex
        {
            SharpTop,
            BluntTop
        }
    }
}