using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using LuckSmile.MapHexs;

public class TestringGUIMAp : MonoBehaviour
{
    [SerializeField] private Vector2 size = Vector2.zero;
    [SerializeField] private UIHexCell prefab = null;
    [SerializeField] private Transform content = null;
    private DataMapHexs mapHexs;
    private Dictionary<Vector2, UIHexCell> uiHexs;
    private void Full()
    {
        for(int index = 0; index < mapHexs.hexs.Count; index++)
        {
            DataEarthHex data = mapHexs.hexs[index];
            UIHexCell cell = UIHexCell.Instantiate(prefab, content, data);
            cell.transform.localPosition = new Vector3(-size.x * data.Index.x, size.y * data.Index.y);
            uiHexs.Add(data.Index, cell);
        }
        for(int index = 0; index < mapHexs.hexs.Count; index++)
        {
            DataEarthHex data = mapHexs.hexs[index];
            UIHexCell parent = uiHexs[data.Index];
            for (int indexChild = 0; indexChild < data.childs.Count; indexChild++)
            {
                DataEarthHex dataChild = data.childs[indexChild];
                UIHexCell cell = uiHexs[dataChild.Index];
                parent.Chillds.Add(cell);
            }
        }
    }
    private void Start()
    {
        mapHexs = Resources.Load<DataMapHexs>("DataMapHexs");
        uiHexs = new Dictionary<Vector2, UIHexCell>();
        Full();
    }

}
