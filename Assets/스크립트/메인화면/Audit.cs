using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Audit : MonoBehaviour
{
    [SerializeField] public Transform item;
    [SerializeField] public Transform itemName;
    [SerializeField] public Button enhance;
    public void Open(int itemId)
    {
        item.GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(itemId).Image;
        item.GetChild(0).GetComponent<Text>().text = UserInfo.instance.GetUserItemInfo(itemId).numberOfItem.ToString();
        itemName.GetComponent<Text>().text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(itemId).Text);
        itemName.GetComponent<Text>().font = TextChart.instance.GetFont();

        enhance.gameObject.SetActive(ItemUpgradeChart.instance.GetItemUpgradeChartChartInfo(itemId) != null);

        enhance.onClick.RemoveAllListeners();
        enhance.onClick.AddListener(() => {

            this.gameObject.SetActive(false);
            PopupManager.instance.AuditEnhance(itemId);
        });
    }
}
