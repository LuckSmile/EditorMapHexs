using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Security.Policy;

namespace LuckSmile.EditorMapHexs
{
    public class EarthHex
    {
        private readonly DataMapHexs mapHexs;
        private readonly List<EarthHex> childs = null;
        public DataEarthHex Data { get; private set; }
        public EarthHex[] Childs { get => childs.ToArray(); }
        public EarthHex parent;
        public Types Type { get; private set; }
        public OnEvents OnEvent { get; private set; }
        private GUIStyle style;
        public Rect RectPosition;

        public EarthHex(DataEarthHex data, Types type, OnEvents onEvent, DataMapHexs mapHexs, GUIStyle style)
        {
            this.mapHexs = mapHexs;
            this.Data = data;
            this.Type = type;
            this.OnEvent = onEvent;
            this.style = style;
            this.childs = new List<EarthHex>();
        }
        public void AddChild(EarthHex hex)
        {
            Data.Childs.Add(hex.Data);
            childs.Add(hex);
        }
        public void RemoveChild(EarthHex hex)
        {
            Data.Childs.Remove(hex.Data);
            childs.Remove(hex);
        }
        public void Draw()
        {
            GUI.Box(RectPosition, $"x: {Data.Index.x} / y: {Data.Index.y}" + $"\nx:{RectPosition.x}/y:{RectPosition.y}", style);
        }
        public void Drag(Vector2 delta)
        {
            this.RectPosition.position += delta;
        }
        public void ChangeState()
        {
            if (this.Type == EarthHex.Types.Pointer)
            {
                this.style.normal.background = mapHexs.parametersEarthHex.TextureHexBase;
                this.style.border = new RectOffset(4, 4, 4, 4);

                this.Type = Types.Base;
                this.parent.AddChild(this);
                this.OnEvent.OnRemoveNoneHexs?.Invoke();
            }
            else
            {
                mapHexs.ReproductionPointers(this);
            }
        }
        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Удалить"), false, () => OnEvent.OnRemove());
            genericMenu.ShowAsContext();
        }
        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        ChangeState();
                        GUI.changed = true;
                    }
                    if (this.Type == Types.Base)
                    {
                        if (e.button == 1 && this.parent != null)
                        {
                            this.ProcessContextMenu();
                            GUI.changed = true;
                        }
                    }
                    break;
            }
            return false;
        }
        public struct OnEvents
        {
            public Action<EarthHex> OnEdit { get; private set; }
            public Action OnRemove { get; private set; }
            public Action OnRemoveNoneHexs { get; private set; }
            public OnEvents(Action<EarthHex> OnEdit, Action OnRemove, Action OnRemoveNoneHexs)
            {
                this.OnEdit = OnEdit;
                this.OnRemove = OnRemove;
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
