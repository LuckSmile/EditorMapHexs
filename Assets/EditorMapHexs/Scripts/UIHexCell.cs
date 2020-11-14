using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;
public class UIHexCell : MonoBehaviour, IPointerDownHandler
{
    public Image Icon => icon;
    [SerializeField] private Image icon = null;
    public TypesStatus TypeStatus { get; private set; } = TypesStatus.Close;
    public DataEarthHex Data { get; private set; }
    public List<UIHexCell> Chillds { get; private set; }
    public static UIHexCell Instantiate(UIHexCell prefab, Transform parent, DataEarthHex data)
    {
        UIHexCell cell = Instantiate(prefab, parent);
        cell.Chillds = new List<UIHexCell>();
        if (data.Index == Vector2.zero)
        {
            cell.icon.color = Color.grey;
            cell.TypeStatus = TypesStatus.CanBeOpened;
        }
        else
        {
            cell.icon.color = Color.black;
        }
        cell.Data = data;
        return cell;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        switch(this.TypeStatus)
        {
            case TypesStatus.CanBeOpened:
                this.TypeStatus = TypesStatus.Open;
                this.Icon.color = Color.white;
                for(int index = 0; index < Chillds.Count; index++)
                {
                    UIHexCell uICell = Chillds[index];
                    uICell.Icon.color = Color.grey;
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
