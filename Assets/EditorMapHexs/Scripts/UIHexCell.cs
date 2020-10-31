using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class UIHexCell : MonoBehaviour, IPointerDownHandler
{
    public TypesStatus TypeStatus { get; private set; } = TypesStatus.Close;
    private DataEarthHex data = null;
    private List<UIHexCell> chillds = null;
    public static UIHexCell Instantiate(UIHexCell prefab, Transform parent, DataEarthHex data)
    {
        UIHexCell cell = Instantiate(prefab, parent);
        cell.data = data;
        return cell;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        switch(this.TypeStatus)
        {
            case TypesStatus.CanBeOpened:
                for(int index = 0; index < chillds.Count; index++)
                {
                    UIHexCell uICell = chillds[index];
                    uICell.TypeStatus = TypesStatus.CanBeOpened;
                }
                break;
        }
    }
    public enum TypesStatus
    {
        Open,
        CanBeOpened,
        Close
    }
}
