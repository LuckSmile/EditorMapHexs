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
        this.parametersEarthHex = Resources.Load<ParametersEarthHex>("Assets/EditorMapHexs/Settings/ParametersEarthHex");
        if(this.parametersEarthHex == null)
        {
            this.parametersEarthHex = ScriptableObject.CreateInstance<ParametersEarthHex>();
            AssetDatabase.CreateAsset(this.parametersEarthHex, "Assets/EditorMapHexs/Settings/ParametersEarthHex.asset");
            AssetDatabase.SaveAssets();
        }
        this.map = new DataMapHexs(DataMapHexs.TypesHex.SharpTop, new Vector2(600, 600), new Vector2(80, 80));
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
            Vector2 start = hex.ThisData.RectPosition.center;
            for (int indexChild = 0; indexChild < hex.ThisData.childs.Count; indexChild++)
            {
                EarthHex child = hex.ThisData.childs[indexChild];
                Vector2 end = child.ThisData.RectPosition.center;
                Vector2 center = (start + end) / 2;
                Handles.DrawBezier(start, end, center, center, Color.white, null, 4f);
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
