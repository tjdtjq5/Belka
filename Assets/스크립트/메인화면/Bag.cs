using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag : MonoBehaviour
{
    public Transform contents;
    public Sprite nullImg;

    public void Open()
    {
        Initialized();
        List<UserItemInfo> userItemInfos = UserInfo.instance.GetUserItemInfos();
        UserInfoSet(userItemInfos);
    }

    void Initialized()
    {
        for (int i = 0; i < contents.childCount; i++)
        {
            contents.GetChild(i).GetComponent<Image>().sprite = nullImg;
            contents.GetChild(i).GetChild(0).gameObject.SetActive(false);
            contents.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            contents.GetChild(i).GetComponent<Button>().enabled = false;
        }
    }
    void UserInfoSet(List<UserItemInfo> userItemInfos)
    {
        int len = userItemInfos.Count;
        if (len > contents.childCount) len = contents.childCount;
        for (int i = 0; i < len; i++)
        {
            contents.GetChild(i).GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(userItemInfos[i].itemId).Image;
            contents.GetChild(i).GetChild(0).gameObject.SetActive(true);
            contents.GetChild(i).GetChild(0).GetComponent<Text>().text = userItemInfos[i].numberOfItem.ToString();
            int itemId = 0;
            switch (ItemChart.instance.GetItemChartChartInfo(userItemInfos[i].itemId).Type)
            {
                case ItemType.심사서조각:
                    contents.GetChild(i).GetComponent<Button>().enabled = true;
                    itemId = userItemInfos[i].itemId;
                    contents.GetChild(i).GetComponent<Button>().onClick.AddListener(() => { PopupManager.instance.Piece(itemId); });
                    break;
                case ItemType.심사서:
                    contents.GetChild(i).GetComponent<Button>().enabled = true;
                    itemId = userItemInfos[i].itemId;
                    contents.GetChild(i).GetComponent<Button>().onClick.AddListener(() => { PopupManager.instance.Audit(itemId); });
                    break;
                case ItemType.가챠박스:
                    contents.GetChild(i).GetComponent<Button>().enabled = true;
                    itemId = userItemInfos[i].itemId;
                    contents.GetChild(i).GetComponent<Button>().onClick.AddListener(() => { PopupManager.instance.GachaBoxOpen(itemId); });
                    break;
            }
        }
    }
}
