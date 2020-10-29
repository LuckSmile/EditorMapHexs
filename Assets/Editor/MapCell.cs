namespace LuckSmile.Map
{
    using UnityEditor;
    using UnityEngine;
    using System;
    public class MapCell
    {
        public Data ThisData { get; private set; }
        private GUIStyle style;
        public MapCell(Data data)
        {
            this.ThisData = data;
            style = new GUIStyle();
            style.normal.background = (data.Type == Data.Types.None ? EditorGUIUtility.Load("Assets/None.png") : EditorGUIUtility.Load("Assets/Full.png")) as Texture2D;
            style.border = new RectOffset(4, 4, 4, 4);
        }
        public void Draw()
        {
            GUI.Box(ThisData.Position, "", style);
        }
        public class Data
        {
            public (int x, int y) Index { get; private set; }
            public Types Type { get; private set; }
            public Rect Position { get; set; }
            public OnEvents OnEvent { get; private set; }
            public Data(Rect position, OnEvents onEvent, Types type, (int x, int y) index)
            {
                this.Index = index;
                this.Position = position;
                this.OnEvent = onEvent;
                this.Type = type;
            }
            public struct OnEvents
            {
                public Action<MapCell> OnEdit { get; private set; }
                public Action<MapCell> OnRemove { get; private set; }
                public Action<MapCell> OnCreate { get; private set; }
                public OnEvents(Action<MapCell> OnEdit, Action<MapCell> OnRemove, Action<MapCell> OnCreate)
                {
                    this.OnEdit = OnEdit;
                    this.OnRemove = OnRemove;
                    this.OnCreate = OnCreate;
                }
            }
            public enum Types
            {
                None,
                Full
            }
        }
    }
}
