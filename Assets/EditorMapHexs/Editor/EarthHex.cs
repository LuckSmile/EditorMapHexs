using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace LuckSmile.EditorMapHexs
{
    public class EarthHex
    {
        public Data ThisData { get; private set; }
        public EarthHex(Data data, GUIStyle style)
        {
            this.ThisData = data;
            ThisData.Set(data.Type, style);
        }
        public void Draw()
        {
            GUI.Box(ThisData.RectPosition, $"x: {ThisData.Index.x} / y: {ThisData.Index.y}" + $"\nx:{ThisData.RectPosition.x}/y:{ThisData.RectPosition.y}", ThisData.Style);
        }
        public void Drag(Vector2 delta)
        {
            this.ThisData.RectPosition.position += delta;
        }
        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Удалить"), false, () => ThisData.OnEvent.OnRemove());
            genericMenu.ShowAsContext();
        }
        public bool ProcessEvents(Event e)
        {
            switch(e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        ThisData.OnEvent.OnChangeState?.Invoke(this);
                        GUI.changed = true;
                    }
                    if(this.ThisData.Type == Types.Base)
                    {
                        if (e.button == 1 && this.ThisData.parent != null)
                        {
                            this.ProcessContextMenu();
                            GUI.changed = true;
                        }
                    }
                    break;
            }
            return false;
        }
        public class Data
        {
            public List<EarthHex> childs;
            public EarthHex parent = null;
            public Vector2 Index { get; private set; }
            public Types Type { get; private set; }
            public Rect RectPosition;
            public OnEvents OnEvent { get; set; }
            public GUIStyle Style { get; private set; }
            public Data(Vector2 index, Types type)
            {
                childs = new List<EarthHex>();
                this.Index = index;
                this.RectPosition = new Rect();
                this.Type = type;
            }
            public void Set(Types Type, GUIStyle Style)
            {
                this.Type = Type;
                this.Style = Style;
            }
        }
        public struct OnEvents
        {
            public Action<EarthHex> OnEdit { get; private set; }
            public Action OnRemove { get; private set; }
            public Action OnRemoveNoneHexs { get; private set; }
            public Action<EarthHex> OnChangeState { get; private set; }
            public OnEvents(Action<EarthHex> OnEdit, Action OnRemove, Action<EarthHex> OnChangeState, Action OnRemoveNoneHexs)
            {
                this.OnEdit = OnEdit;
                this.OnRemove = OnRemove;
                this.OnChangeState = OnChangeState;
                this.OnRemoveNoneHexs = OnRemoveNoneHexs;
            }
        }
        public enum Types
        {
            Pointer,
            Base
        }
    }
}
