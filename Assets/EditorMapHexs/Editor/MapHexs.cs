using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using LuckSmile.MapHexs;

namespace LuckSmile.EditorMapHexs
{
    public class MapHexs
    {
        public EarthHex[] Hexs => hexs.Values.ToArray();
        public DataMapHexs Data { get; private set; }
        private readonly Dictionary<Vector2, EarthHex> hexs = null;
        private readonly Vector2 sizeCell = Vector2.zero;
        public MapHexs(DataMapHexs data, Vector2 sizeWindow, Vector2 sizeCell)
        {
            this.Data = data;

            this.sizeCell = sizeCell;
            this.hexs = new Dictionary<Vector2, EarthHex>();

            Unloading(sizeWindow);
        }
        private EarthHex CreateHex(Vector2 index, EarthHex.Types type, DataEarthHex data = null)
        {
            if (hexs.Keys.Contains(index) == true)
                return hexs[index];

            GUIStyle style = new GUIStyle();
            style.normal.background = type == EarthHex.Types.Pointer ? Data.parametersEarthHex.TextureHexPointer : Data.parametersEarthHex.TextureHexBase;
            style.border = new RectOffset(4, 4, 4, 4);

            if(data == null)
                data = new DataEarthHex(new Vector2Int((int)index.x, (int)index.y));

            EarthHex hex = new EarthHex(data, type, new EarthHex.OnEvents(null, () => RemoveHex(index), ClearPointers), this, style)
            {
                RectPosition = new Rect(Vector2.zero, sizeCell)
            };
            hexs.Add(index, hex);
            return hex;
        }
        private EarthHex CreatePointer(Vector2 index, Vector2 direction, EarthHex parent)
        {
            if (hexs.Keys.Contains(index) == true)
                return hexs[index];
            
            EarthHex hex = CreateHex(index, EarthHex.Types.Pointer);

            Vector2 position = parent == null ? Vector2.zero : parent.RectPosition.position;
            position += direction * sizeCell;
            Debug.Log(direction);
            hex.RectPosition = new Rect(position, sizeCell);
            hex.parent = parent;
            return hex;
        }
        private void Unloading(Vector2 sizeWindow)
        {
            EarthHex SpawnChilds(EarthHex hex)
            {
                Dictionary<Vector2, Vector2> directionPoint = new Dictionary<Vector2, Vector2>
                {
                    { new Vector2(0, 1), new Vector2(0, 1.4f) },
                    { new Vector2(0, -1), new Vector2(0, -1.4f) },
                    { new Vector2(1, -1), new Vector2(1.1f, -0.5f * 1.4f ) },
                    { new Vector2(-1, 1), new Vector2(-1.1f, 0.5f * 1.4f) },
                    { new Vector2(1, 1), new Vector2(1.1f, 0.5f * 1.4f) },
                    { new Vector2(-1, -1), new Vector2(-1.1f, -0.5f * 1.4f) }
                };

                DataEarthHex[] dataChilds = hex.Data.childs.ToArray();
                hex.Data.childs.Clear();
                for (int index = 0; index < dataChilds.Length; index++)
                {
                    DataEarthHex hexDataChild = dataChilds[index];

                    EarthHex hexChild = CreateHex(hexDataChild.Index, EarthHex.Types.Base, hexDataChild);
                    Vector2 position = hex.RectPosition.position;
                    
                    Vector2 key = hex.Data.Index - hexChild.Data.Index;
                    key.y /= Mathf.Abs(key.y) == 2 ? 2 : 1;
                    Debug.Log(key);
                    Vector2 direction = directionPoint[key];
                    position += direction * sizeCell;
                    hexChild.RectPosition = new Rect(position, sizeCell);
                    hexChild.parent = hex;
                    
                    hex.AddChild(hexChild);
                    SpawnChilds(hexChild);
                }
                return hex;
            }

            DataEarthHex data;
            if (Data.hexs.Count == 0)
                data = new DataEarthHex(new Vector2Int(0, 0));
            else
                data = Data.hexs[0];
            
            EarthHex main = CreateHex(Vector2.zero, EarthHex.Types.Base, data);
            main.RectPosition.position = (sizeWindow - sizeCell) / 2f;
            SpawnChilds(main);
        }
        public void Saving()
        {
            Data.hexs.Clear();
            EarthHex[] hexs = this.hexs.Values.ToArray();
            Debug.Log(hexs.Length);
            for (int index = 0; index < hexs.Length; index++)
            {
                EarthHex hex = hexs[index];
                Data.hexs.Add(hex.Data);
            }
            Debug.Log(Data.hexs.Count);
            EditorUtility.SetDirty(Data);
            AssetDatabase.SaveAssets();

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
            MapHexsEditor.selectedHex = null;
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
                    if(Data.Type == DataMapHexs.TypesHex.SharpTop)
                    {
                        Vector2 index = new Vector2(direction[x], direction[y]);
                        index.y *= index.x == 0 ? 2 : 1;
                        EarthHex pointer = this.CreatePointer(hex.Data.Index - index, directionPoint[new Vector2(direction[x], direction[y])], hex);
                    }
                }
            }
        }
    }
}