using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LuckSmile.EditorMapHexs;
using LuckSmile.MapHexs;
public class MapHexsEditor : EditorWindow
{
    public static EarthHex selectedHex;
    private ParametersEarthHex parametersEarthHex;

    private MapHexs map;

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
        if(this.parametersEarthHex == null)
        {
            this.parametersEarthHex = ScriptableObject.CreateInstance<ParametersEarthHex>();
            AssetDatabase.CreateAsset(this.parametersEarthHex, "Assets/EditorMapHexs/Settings/ParametersEarthHex.asset");
            EditorUtility.SetDirty(parametersEarthHex);
        }

        DataMapHexs data = Resources.Load<DataMapHexs>("DataMapHexs");
        if(data == null)
        {
            data = ScriptableObject.CreateInstance<DataMapHexs>();
            data.hexs = new List<DataEarthHex>();
            AssetDatabase.CreateAsset(data, "Assets/EditorMapHexs/Resources/DataMapHexs.asset");
        }
        
        data.parametersEarthHex = parametersEarthHex;
        EditorUtility.SetDirty(data);

        AssetDatabase.SaveAssets();
        this.map = new MapHexs(data, new Vector2(600, 600), new Vector2(80, 80));
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
            case EventType.MouseDown:
                if (e.button == 1 && selectedHex == null)
                {
                    ProcessContextMenu();
                }
                break;
        }
    }
    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Сохранить"), false, () => map.Saving());
        genericMenu.ShowAsContext();
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
