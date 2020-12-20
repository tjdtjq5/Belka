using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    [SerializeField] public Transform item;
    [SerializeField] public Transform itemName;
    [SerializeField] public Button assosiation;
    public void Open(int itemId)
    {
        item.GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(itemId).Image;
        item.GetChild(0).GetComponent<Text>().text = UserInfo.instance.GetUserItemInfo(itemId).numberOfItem.ToString();
        itemName.GetComponent<Text>().text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(itemId).Text);
        itemName.GetComponent<Text>().font = TextChart.instance.GetFont();

        assosiation.onClick.RemoveAllListeners();
        assosiation.onClick.AddListener(() => { this.gameObject.SetActive(false); PopupManager.instance.PieceAssosiation(itemId); });
    }
}
