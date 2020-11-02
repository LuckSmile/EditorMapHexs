using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class UIHexCell : MonoBehaviour, IPointerDownHandler
{
    public TypesStatus TypeStatus { get; private set; } = TypesStatus.Close;
    public DataEarthHex Data { get; private set; }
    public List<UIHexCell> Chillds { get; private set; }
    public static UIHexCell Instantiate(UIHexCell prefab, Transform parent, DataEarthHex data)
    {
        UIHexCell cell = Instantiate(prefab, parent);
        cell.Data = data;
        return cell;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        switch(this.TypeStatus)
        {
            case TypesStatus.CanBeOpened:
                for(int index = 0; index < Chillds.Count; index++)
                {
                    UIHexCell uICell = Chillds[index];
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
