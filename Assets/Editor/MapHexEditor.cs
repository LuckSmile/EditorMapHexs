﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LuckSmile.Map;
public class MapHexEditor : EditorWindow
{
    //private SystemInteractionsCells interactionsCells;
    private DataMapHexs map;
    private Vector2 pointPostition = Vector2.zero;
    private Vector2 drag = Vector2.zero;
    [MenuItem("Tools/Map")] public static void Init()
    {
        MapHexEditor window = GetWindow<MapHexEditor>();
        window.maxSize = new Vector2(600, 600);
        window.minSize = new Vector2(600, 600);
        window.Show();
    }
    private void OnEnable()
    {
        this.map = new DataMapHexs(DataMapHexs.TypesHex.SharpTop, new Vector2(600, 600), new Vector2(80, 80));
    }
    private void OnGUI()
    {
        Draw();

        ProccesEarthHexEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }
    private void Draw()
    {
        for (int index = 0; index < map.Hexs.Length; index++)
        {
            EarthHex cell = map.Hexs[index];
            cell.Draw();
        }
    }
    private void ProccesEarthHexEvents(Event e)
    {
        List<EarthHex> earthHices = new List<EarthHex>();
        earthHices.AddRange(this.map.Hexs);
        for(int index = 0; index < earthHices.Count; index++)
        {
            if(earthHices[index].ThisData.RectPosition.Contains(e.mousePosition))
            {
                bool guiChanged = earthHices[index].ProcessEvents(e);
                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }
    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;
        switch (e.type)
        {
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }
    public void OnDrag(Vector2 delta)
    {
        drag = delta;
        pointPostition += drag;
        for (int index = 0; index < map.Hexs.Length; index++)
        {
            map.Hexs[index].Drag(drag);
        }
        GUI.changed = true;
    }
}
