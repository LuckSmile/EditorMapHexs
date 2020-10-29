using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using JetBrains.Annotations;

namespace LuckSmile.Map
{
    public class DataMapHexs
    {
        public EarthHex[] Hexs => hexs.Values.ToArray();
        private readonly Dictionary<Vector2, EarthHex> hexs;
        private readonly Vector2 sizeWindow;
        private readonly Vector2 sizeCell;
        private readonly TypesHex type;
        public DataMapHexs(TypesHex type, Vector2 sizeWindow, Vector2 sizeCell)
        {
            this.type = type;
            this.sizeWindow = sizeWindow;
            this.sizeCell = sizeCell;
            this.hexs = new Dictionary<Vector2, EarthHex>();

            EarthHex hex = CreateHex(Vector2.zero, EarthHex.Types.Full);
            hex.ThisData.RectPosition.position = (sizeWindow - sizeCell) / 2f;

            //ReproductionHex(hex);
        }
        private EarthHex CreateHex(Vector2 index, EarthHex.Types type)
        {
            if (hexs.Keys.Contains(index) == false)
            {
                EarthHex hex = new EarthHex(new EarthHex.Data(index, type));
                hex.ThisData.RectPosition = new Rect(Vector2.zero, sizeCell);
                hex.ThisData.OnEvent = new EarthHex.OnEvents(null, () => RemoveHex(index), () => ReproductionHex(hex), RemoveNoneHexs);
                hexs.Add(hex.ThisData.Index, hex);
                return hex;
            }
            return null;
        }
        private EarthHex CreateHexSharpTop(Vector2 index, Vector2 direction, EarthHex parent)
        {
            EarthHex hex = CreateHex(index, EarthHex.Types.None);
            
            if (hex == null)
                return null;

            Vector2 position = parent == null ? Vector2.zero : parent.ThisData.RectPosition.position;
            position += direction * sizeCell;
            hex.ThisData.RectPosition = new Rect(position, sizeCell);
            hex.ThisData.parent = parent;
            return hex;
        }
        public void RemoveHex(Vector2 index)
        {
            RemoveNoneHexs();
            void Remove(EarthHex hex)
            {
                hexs.Remove(hex.ThisData.Index);
                EarthHex[] childs = hex.ThisData.childs.ToArray();
                for(int indexChild = 0; indexChild < childs.Length; indexChild++)
                {
                    Remove(childs[indexChild]);
                }
            }
            Remove(hexs[index]);
        }
        public void RemoveNoneHexs()
        {
            EarthHex[] hexs = this.hexs.Values.ToArray();
            for (int index = 0; index < hexs.Length; index++)
            {
                EarthHex h = hexs[index];
                if (h.ThisData.Type == EarthHex.Types.None)
                {
                    this.hexs.Remove(h.ThisData.Index);
                }
            }
        }
        public void ReproductionHex(EarthHex hex)
        {
            RemoveNoneHexs();

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
                        this.CreateHexSharpTop(hex.ThisData.Index - index, directionPoint[new Vector2(direction[x], direction[y])], hex);
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