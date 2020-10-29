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
        public EarthHex(Data data)
        {
            this.ThisData = data;
            GUIStyle style = new GUIStyle();
            style.normal.background = (ThisData.Type == Types.Pointer ? EditorGUIUtility.Load("Assets/None.png") : EditorGUIUtility.Load("Assets/Full.png")) as Texture2D;
            style.border = new RectOffset(4, 4, 4, 4);
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
        public void ChangeState()
        {
            if(ThisData.Type == Types.Pointer)
            {
                GUIStyle style = new GUIStyle();
                style.normal.background = EditorGUIUtility.Load("Assets/Full.png") as Texture2D;
                style.border = new RectOffset(4, 4, 4, 4);
                ThisData.Set(Types.Base, style);
                ThisData.parent.ThisData.childs.Add(this);
                this.ThisData.OnEvent.OnRemoveNoneHexs?.Invoke();
            }
            else
            {
                ThisData.OnEvent.OnReproduction?.Invoke();
            }
        }
        public bool ProcessEvents(Event e)
        {
            switch(e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        ChangeState();
                        GUI.changed = true;
                    }
                    else if (e.button == 1 && this.ThisData.parent != null)
                    {
                        this.ThisData.OnEvent.OnRemove();
                        GUI.changed = true;
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
            public Action OnReproduction { get; private set; }
            public OnEvents(Action<EarthHex> OnEdit, Action OnRemove, Action OnReproduction, Action OnRemoveNoneHexs)
            {
                this.OnEdit = OnEdit;
                this.OnRemove = OnRemove;
                this.OnReproduction = OnReproduction;
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
