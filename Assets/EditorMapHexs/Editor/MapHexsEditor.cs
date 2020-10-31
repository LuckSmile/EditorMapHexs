using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LuckSmile.EditorMapHexs;
public class MapHexsEditor : EditorWindow
{
    //private SystemInteractionsCells interactionsCells;
    private ParametersEarthHex parametersEarthHex;
    private DataMapHexs map;
    private Vector2 pointPostition = Vector2.zero;
    private Vector2 drag = Vector2.zero;
    [MenuItem("Tools/Map")] public static void Init()
    {
        MapHexsEditor window = GetWindow<MapHexsEditor>();
        window.maxSize = new Vector2(600, 600);
        window.minSize = new Vector2(600, 600);
        window.Show();
    }
    private void OnEnable()
    {
        this.parametersEarthHex = EditorGUIUtility.Load("Assets/EditorMapHexs/Settings/ParametersEarthHex.asset") as ParametersEarthHex;
        Debug.Log(this.parametersEarthHex);
        if(this.parametersEarthHex == null)
        {
            this.parametersEarthHex = ScriptableObject.CreateInstance<ParametersEarthHex>();
            AssetDatabase.CreateAsset(this.parametersEarthHex, "Assets/EditorMapHexs/Settings/ParametersEarthHex.asset");
            EditorUtility.SetDirty(parametersEarthHex);
            AssetDatabase.SaveAssets();
        }
        this.map = new DataMapHexs(DataMapHexs.TypesHex.SharpTop, parametersEarthHex, new Vector2(600, 600), new Vector2(80, 80));
    }
    private void OnGUI()
    {
        DrawConnections();
        Draw();

        ProccesEarthHexEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }
    private void DrawConnections()
    {
        for(int index = 0; index < map.Hexs.Length; index++)
        {
            EarthHex hex = map.Hexs[index];
            Vector2 start = hex.RectPosition.center + new Vector2(0f, 5f);
            for (int indexChild = 0; indexChild < hex.Childs.Length; indexChild++)
            {
                EarthHex child = hex.Childs[indexChild];
                Vector2 end = child.RectPosition.center + new Vector2(0f, 5f);
                Vector2 center = (start + end) / 2;
                Handles.DrawBezier(start, end, center, center, this.parametersEarthHex.ColorArrow, null, 4f);
            }
        }
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
        List<EarthHex> hices = new List<EarthHex>();
        hices.AddRange(this.map.Hexs);
        for(int index = 0; index < hices.Count; index++)
        {
            EarthHex hex = hices[index];
            if(hex.RectPosition.Contains(e.mousePosition))
            {
                bool guiChanged = hex.ProcessEvents(e);
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
    private void OnDrag(Vector2 delta)
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
