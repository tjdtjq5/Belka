using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaBoxOpen : MonoBehaviour
{
    public Image itemImg;
    public Text itemName;
    public Text itemCount;

    int boxItemId;
    int count;
    int openCount;

    // 스위치콘의 위치 
    public Transform switchCon;
    public Image foreSlider;
    public Text countText;
    public Transform minXtransform;
    public Transform maxXtransform;
    float min;
    float max;
    public Camera theCam;

    public void Open(int itemId)
    {
        min = minXtransform.position.x;
        max = maxXtransform.position.x;

        count = 0;
        if (UserInfo.instance.GetUserItemInfo(itemId) != null)
        {
            count = UserInfo.instance.GetUserItemInfo(itemId).numberOfItem;
        }

        int maxCount = count;
        int minCount = 1;
        foreSlider.fillAmount = minCount / (float)maxCount;
        countText.text = "1";
        switchCon.position = new Vector2(min + ((max - min) * foreSlider.fillAmount), switchCon.position.y);
     

        boxItemId = itemId;
        openCount = 1;

       

        itemImg.sprite = ItemChart.instance.GetItemChartChartInfo(itemId).Image;
        itemName.text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(itemId).Text);
        itemName.font = TextChart.instance.GetFont();
        itemCount.text = count.ToString();
    }
    public void DragSwitch()
    {
        Vector2 pos = theCam.ScreenToWorldPoint(Input.mousePosition);
        int maxCount = count;
        int minCount = 1;
        float f = minCount / (float)maxCount;
        pos = new Vector2(Mathf.Clamp(pos.x, min + ((max - min) * f), max), switchCon.position.y);
        switchCon.position = pos;

        float maxValue = max - min;
        float currentValue = pos.x - min;

        foreSlider.fillAmount = currentValue / maxValue;

        openCount = (int)(count * foreSlider.fillAmount);
        if (openCount <= 0) openCount = 1;
        countText.text = openCount.ToString();
    }

    public void OK()
    {
        this.gameObject.SetActive(false);

        if (openCount <= 0)
        {
            PopupManager.instance.blackPannel.SetActive(false);
            return;
        }

        List<int> itemIdList = new List<int>();
        List<int> itemCount = new List<int>();

        int gachaCount = GachaBoxChart.instance.GetGachaBoxChartInfo(boxItemId).GachaCount;
        List<GachaChartInfo> gachaChartInfoList = GachaChart.instance.GetGachaChartInfos(boxItemId);
        for (int a = 0; a < openCount; a++)
        {
            for (int i = 0; i < gachaCount; i++)
            {
                float r = Random.Range(0, 100);
                float p = 0;
                for (int j = 0; j < gachaChartInfoList.Count; j++)
                {
                    p += gachaChartInfoList[j].Percent;
                    if (r < p)
                    {
                        // 아이템 저장 
                        if (itemIdList.Contains(gachaChartInfoList[j].RewardId))
                        {
                            for (int k = 0; k < itemIdList.Count; k++)
                            {
                                if (itemIdList[k] == gachaChartInfoList[j].RewardId)
                                {
                                    itemCount[k] += gachaChartInfoList[j].RewardCount;
                                }
                            }
                        }
                        else
                        {
                            itemIdList.Add(gachaChartInfoList[j].RewardId);
                            itemCount.Add(gachaChartInfoList[j].RewardCount);
                        }
                        break;
                    }
                }
            }
        }

        UserInfo.instance.SetUserItemInfo(boxItemId, UserInfo.instance.GetUserItemInfo(boxItemId).numberOfItem - openCount);

        for (int i = 0; i < itemIdList.Count; i++)
        {
            UserItemReward(itemIdList[i], itemCount[i]);
        }

        UserInfo.instance.SaveUserItemInfo(() => { });

        PopupManager.instance.GachaResult(itemIdList.Count, itemIdList.ToArray(), itemCount.ToArray(), () => { });
    }

    public void UserItemReward(int rewardItem, int rewardCount)
    {
        UserInfo.instance.PutUserItem(rewardItem, rewardCount);
    }
}
