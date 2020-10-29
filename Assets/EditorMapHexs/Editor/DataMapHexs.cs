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
        private readonly Dictionary<Vector2, EarthHex> hexs;
        private readonly Vector2 sizeCell;
        private readonly TypesHex type;
        public DataMapHexs(TypesHex type, Vector2 sizeWindow, Vector2 sizeCell)
        {
            this.type = type;
            this.sizeCell = sizeCell;
            this.hexs = new Dictionary<Vector2, EarthHex>();

            EarthHex hex = CreateHex(Vector2.zero, EarthHex.Types.Base);
            hex.ThisData.RectPosition.position = (sizeWindow - sizeCell) / 2f;
            hexs.Add(hex.ThisData.Index, hex);
        }
        private EarthHex CreateHex(Vector2 index, EarthHex.Types type)
        {
            EarthHex hex = new EarthHex(new EarthHex.Data(index, type));
            hex.ThisData.RectPosition = new Rect(Vector2.zero, sizeCell);
            hex.ThisData.OnEvent = new EarthHex.OnEvents(null, () => RemoveHex(index), () => ReproductionPointers(hex), ClearPointers);
            return hex;
        }
        private EarthHex CreatePointer(Vector2 index, Vector2 direction, EarthHex parent)
        {
            if (hexs.Keys.Contains(index) == true)
                return null;
            
            EarthHex hex = CreateHex(index, EarthHex.Types.Pointer);

            Vector2 position = parent == null ? Vector2.zero : parent.ThisData.RectPosition.position;
            position += direction * sizeCell;
            hex.ThisData.RectPosition = new Rect(position, sizeCell);
            hex.ThisData.parent = parent;
            return hex;
        }
        public void RemoveHex(Vector2 index)
        {
            ClearPointers();
            void Remove(EarthHex hex)
            {
                hexs.Remove(hex.ThisData.Index);
                EarthHex[] childs = hex.ThisData.childs.ToArray();
                for(int indexChild = 0; indexChild < childs.Length; indexChild++)
                {
                    Remove(childs[indexChild]);
                }
            }
            hexs[index].ThisData.parent.ThisData.childs.Remove(hexs[index]);
            Remove(hexs[index]);
        }
        public void ClearPointers()
        {
            EarthHex[] hexs = this.hexs.Values.ToArray();
            for (int index = 0; index < hexs.Length; index++)
            {
                EarthHex h = hexs[index];
                if (h.ThisData.Type == EarthHex.Types.Pointer)
                {
                    this.hexs.Remove(h.ThisData.Index);
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
                        EarthHex pointer = this.CreatePointer(hex.ThisData.Index - index, directionPoint[new Vector2(direction[x], direction[y])], hex);
                        if(pointer != null)
                        {
                            hexs.Add(pointer.ThisData.Index, pointer);
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