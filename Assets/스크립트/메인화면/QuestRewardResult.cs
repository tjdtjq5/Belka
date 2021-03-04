using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardResult : MonoBehaviour
{
    public Transform itemTransform;
    public Sprite diaSprite;
    public Sprite heartSprite;
    public void Open(RewardType[] rewardTypes, int[] IDs, int[] countList)
    {
        // 아이템 얻고 저장 
        for (int i = 0; i < rewardTypes.Length; i++)
        {
            switch (rewardTypes[i])
            {
                case RewardType.Goods:
                    switch (IDs[i])
                    {
                        case 1:
                            UserInfo.instance.PutUserHeart(countList[i]);
                            break;
                        case 2:
                            UserInfo.instance.PutUserCrystal(countList[i]);
                            break;
                    }
                    break;
                case RewardType.Item:
                    UserInfo.instance.PutUserItem(IDs[i], countList[i]);
                    break;
            }
        }
        UserInfo.instance.SaveUserCrystal(() => {
            UserInfo.instance.SaveUserHeartInfo(() => {
                UserInfo.instance.SaveUserItemInfo(() => { });
            });
        });

        // ui 셋팅 
        for (int i = 0; i < itemTransform.childCount; i++)
        {
            if (i < rewardTypes.Length - 1)
            {
                itemTransform.GetChild(i).gameObject.SetActive(true);

                Transform itemUI = itemTransform.GetChild(i);

                switch (rewardTypes[i])
                {
                    case RewardType.Goods:
                        if (IDs[i] == 1) // 하트 
                        {
                            itemUI.GetComponent<Image>().sprite = heartSprite;
                        }
                        if (IDs[i] == 2)
                        {
                            itemUI.GetComponent<Image>().sprite = diaSprite;
                        }
                        break;
                    case RewardType.Item:
                        itemUI.GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(IDs[i]).Image;
                        break;
                } // 아이템 이미지
                itemUI.GetChild(0).GetComponent<Text>().text = countList[i].ToString(); // 아이템 수량 
            }
            else
            {
                itemTransform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
