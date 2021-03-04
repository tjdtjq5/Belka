using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public Sprite onBtnSprite;
    public Sprite offBtnSprite;
    public Sprite onTabBtnSprite;
    public Sprite offTabBtnSprite;

    Image dayBtnImg;
    Image eventBtnImg;
    Image achiveBtnImg;
    public Sprite diaItemSprite;
    public Sprite heartItemSprite;

    public Transform tap;
    public Transform middle;
    public Transform eventContent;
    public Transform dayContent;
    public Transform achiveContent;

    public GameObject cardPrepab;

    private void Awake()
    {
        CardInit();
    }

    private void Start()
    {
        UserInfo.instance.ClearUserQuest("AccountLogin");
        UserInfo.instance.SaveUserQuest(() => { });
        TapUI_Setting();
        OnClickDay();
    }

    void CardInit()
    {
        for (int i = 0; i < eventContent.childCount; i++)
        {
            Destroy(eventContent.GetChild(i).gameObject);
        }
        for (int i = 0; i < dayContent.childCount; i++)
        {
            Destroy(dayContent.GetChild(i).gameObject);
        }
        for (int i = 0; i < achiveContent.childCount; i++)
        {
            Destroy(achiveContent.GetChild(i).gameObject);
        }

        int eventLen = QuestChart.instance.GetCategoryQuest(3).Count;
        int dayLen = QuestChart.instance.GetCategoryQuest(1).Count;
        int achiveLen = QuestChart.instance.GetCategoryQuest(2).Count;

        for (int i = 0; i < eventLen; i++)
        {
            Instantiate(cardPrepab, Vector3.zero, Quaternion.identity, eventContent);
        }
        for (int i = 0; i < dayLen; i++)
        {
            Instantiate(cardPrepab, Vector3.zero, Quaternion.identity, dayContent);
        }
        for (int i = 0; i < achiveLen; i++)
        {
            Instantiate(cardPrepab, Vector3.zero, Quaternion.identity, achiveContent);
        }
    }

    public void OnClickDay()
    {
        dayBtnImg.sprite = onTabBtnSprite;
        eventBtnImg.sprite = offTabBtnSprite;
        achiveBtnImg.sprite = offTabBtnSprite;

        middle.GetChild(0).gameObject.SetActive(false);
        middle.GetChild(1).gameObject.SetActive(true);
        middle.GetChild(2).gameObject.SetActive(false);

        DayUI_Setting();
    } // 탭 버튼 : 일일퀘스트 
    public void OnClickEvent()
    {
        dayBtnImg.sprite = offTabBtnSprite;
        eventBtnImg.sprite = onTabBtnSprite;
        achiveBtnImg.sprite = offTabBtnSprite;

        middle.GetChild(0).gameObject.SetActive(true);
        middle.GetChild(1).gameObject.SetActive(false);
        middle.GetChild(2).gameObject.SetActive(false);

        EventUI_Setting();
    } // 탭 버튼 : 이벤트
    public void OnClickAchive()
    {
        dayBtnImg.sprite = offTabBtnSprite;
        eventBtnImg.sprite = offTabBtnSprite;
        achiveBtnImg.sprite = onTabBtnSprite;

        middle.GetChild(0).gameObject.SetActive(false);
        middle.GetChild(1).gameObject.SetActive(false);
        middle.GetChild(2).gameObject.SetActive(true);

        AchiveUI_Setting();
    } // 탭 버튼 : 업적

    bool isExistEvent()
    {
        if (QuestChart.instance.GetCategoryQuest(3).Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    } // 이벤트가 있는지 여부 

    void TapUI_Setting()
    {
        if (isExistEvent())
        {
            tap.GetChild(0).gameObject.SetActive(true);
            tap.GetChild(1).gameObject.SetActive(false);

            dayBtnImg = tap.transform.GetChild(0).Find("일일미션").GetComponent<Image>();
            eventBtnImg = tap.transform.GetChild(0).Find("이벤트").GetComponent<Image>();
            achiveBtnImg = tap.transform.GetChild(0).Find("업적").GetComponent<Image>();
        }
        else
        {
            tap.GetChild(0).gameObject.SetActive(false);
            tap.GetChild(1).gameObject.SetActive(true);

            dayBtnImg = tap.transform.GetChild(1).Find("일일미션").GetComponent<Image>();
            achiveBtnImg = tap.transform.GetChild(1).Find("업적").GetComponent<Image>();
        }
    } // 탭 ui 셋팅 

    public void EventUI_Setting()
    {
        List<QuestChartInfo> questChartInfos = QuestChart.instance.GetCategoryQuest(3);

        for (int i = 0; i < eventContent.childCount; i++)
        {
            Transform card = eventContent.GetChild(i);

            if (i < questChartInfos.Count)
            {
                int ID = questChartInfos[i].QuestID;
                string questName = questChartInfos[i].QuestName;
                int questCount = questChartInfos[i].QuestCount;
                RewardType[] rewardTypes = questChartInfos[i].RewardType;
                int[] rewardIds = questChartInfos[i].RewardId;
                int[] rewardCounts = questChartInfos[i].RewardCount;

                UserQuest userQuest = UserInfo.instance.GetUserQuestInfo(ID);

                card.gameObject.SetActive(true);

                card.Find("Title").GetChild(0).GetComponent<Text>().text = TextChart.instance.GetText(questName);

                for (int j = 0; j < card.Find("보상").childCount; j++)
                {
                    if (j < rewardTypes.Length)
                    {
                        card.Find("보상").GetChild(j).gameObject.SetActive(true);

                        switch (rewardTypes[j])
                        {
                            case RewardType.Goods:
                                if(rewardIds[j] == 1) card.Find("보상").GetChild(j).GetComponent<Image>().sprite = heartItemSprite;
                                if (rewardIds[j] == 2) card.Find("보상").GetChild(j).GetComponent<Image>().sprite = diaItemSprite;
                                break;
                            case RewardType.Item:
                                card.Find("보상").GetChild(j).GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(rewardIds[j]).Image;
                                break;
                        }

                        card.Find("보상").GetChild(j).GetChild(0).GetComponent<Text>().text = rewardCounts[j].ToString();
                    }
                    else
                    {
                        card.Find("보상").GetChild(j).gameObject.SetActive(false);
                    }
                }

                if (userQuest.isComplete == false && userQuest.clearCount >= questCount)
                {
                    card.Find("수령버튼").GetComponent<Image>().sprite = onBtnSprite;
                    card.Find("수령버튼").GetComponent<Button>().onClick.RemoveAllListeners();
                    card.Find("수령버튼").GetComponent<Button>().onClick.AddListener(() => {
                        RewardOnClick(ID, rewardTypes, rewardIds, rewardCounts);
                    });
                }
                else
                {
                    card.Find("수령버튼").GetComponent<Image>().sprite = offBtnSprite;
                }

                if (userQuest.isComplete == true)
                {
                    card.Find("수령버튼").GetChild(0).GetComponent<Text>().text = "완료";
                }
                else
                {
                    card.Find("수령버튼").GetChild(0).GetComponent<Text>().text = "수령";
                }

                card.Find("카운트").GetComponent<Text>().text = userQuest.clearCount + "/" + questCount;
            }
            else
            {
                card.gameObject.SetActive(false);
            }
        }
    }
    public void DayUI_Setting()
    {
        List<QuestChartInfo> questChartInfos = QuestChart.instance.GetCategoryQuest(1);

        for (int i = 0; i < dayContent.childCount; i++)
        {
            Transform card = dayContent.GetChild(i);

            if (i < questChartInfos.Count)
            {
                int ID = questChartInfos[i].QuestID;
                string questName = questChartInfos[i].QuestName;
                int questCount = questChartInfos[i].QuestCount;
                RewardType[] rewardTypes = questChartInfos[i].RewardType;
                int[] rewardIds = questChartInfos[i].RewardId;
                int[] rewardCounts = questChartInfos[i].RewardCount;

                UserQuest userQuest = UserInfo.instance.GetUserQuestInfo(ID);

                card.gameObject.SetActive(true);

                card.Find("Title").GetChild(0).GetComponent<Text>().text = TextChart.instance.GetText(questName);

                for (int j = 0; j < card.Find("보상").childCount; j++)
                {
                    if (j < rewardTypes.Length)
                    {
                        card.Find("보상").GetChild(j).gameObject.SetActive(true);

                        switch (rewardTypes[j])
                        {
                            case RewardType.Goods:
                                if (rewardIds[j] == 1) card.Find("보상").GetChild(j).GetComponent<Image>().sprite = heartItemSprite;
                                if (rewardIds[j] == 2) card.Find("보상").GetChild(j).GetComponent<Image>().sprite = diaItemSprite;
                                break;
                            case RewardType.Item:
                                card.Find("보상").GetChild(j).GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(rewardIds[j]).Image;
                                break;
                        }

                        card.Find("보상").GetChild(j).GetChild(0).GetComponent<Text>().text = rewardCounts[j].ToString();
                    }
                    else
                    {
                        card.Find("보상").GetChild(j).gameObject.SetActive(false);
                    }
                }

                if (userQuest.isComplete == false && userQuest.clearCount >= questCount)
                {
                    card.Find("수령버튼").GetComponent<Image>().sprite = onBtnSprite;
                    card.Find("수령버튼").GetComponent<Button>().onClick.RemoveAllListeners();
                    card.Find("수령버튼").GetComponent<Button>().onClick.AddListener(() => {
                        RewardOnClick(ID, rewardTypes, rewardIds, rewardCounts);
                    });
                }
                else
                {
                    card.Find("수령버튼").GetComponent<Image>().sprite = offBtnSprite;
                }

                if (userQuest.isComplete == true)
                {
                    card.Find("수령버튼").GetChild(0).GetComponent<Text>().text = "완료";
                }
                else
                {
                    card.Find("수령버튼").GetChild(0).GetComponent<Text>().text = "수령";
                }

                card.Find("카운트").GetComponent<Text>().text = userQuest.clearCount + "/" + questCount;
            }
            else
            {
                card.gameObject.SetActive(false);
            }
        }
    }
    public void AchiveUI_Setting()
    {
        List<QuestChartInfo> questChartInfos = QuestChart.instance.GetCategoryQuest(2);

        for (int i = 0; i < achiveContent.childCount; i++)
        {
            Transform card = achiveContent.GetChild(i);

            if (i < questChartInfos.Count)
            {
                int ID = questChartInfos[i].QuestID;
                string questName = questChartInfos[i].QuestName;
                int questCount = questChartInfos[i].QuestCount;
                RewardType[] rewardTypes = questChartInfos[i].RewardType;
                int[] rewardIds = questChartInfos[i].RewardId;
                int[] rewardCounts = questChartInfos[i].RewardCount;

                UserQuest userQuest = UserInfo.instance.GetUserQuestInfo(ID);

                card.gameObject.SetActive(true);

                card.Find("Title").GetChild(0).GetComponent<Text>().text = TextChart.instance.GetText(questName);

                for (int j = 0; j < card.Find("보상").childCount; j++)
                {
                    if (j < rewardTypes.Length)
                    {
                        card.Find("보상").GetChild(j).gameObject.SetActive(true);

                        switch (rewardTypes[j])
                        {
                            case RewardType.Goods:
                                if (rewardIds[j] == 1) card.Find("보상").GetChild(j).GetComponent<Image>().sprite = heartItemSprite;
                                if (rewardIds[j] == 2) card.Find("보상").GetChild(j).GetComponent<Image>().sprite = diaItemSprite;
                                break;
                            case RewardType.Item:
                                card.Find("보상").GetChild(j).GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(rewardIds[j]).Image;
                                break;
                        }

                        card.Find("보상").GetChild(j).GetChild(0).GetComponent<Text>().text = rewardCounts[j].ToString();
                    }
                    else
                    {
                        card.Find("보상").GetChild(j).gameObject.SetActive(false);
                    }
                }

                if (userQuest.isComplete == false && userQuest.clearCount >= questCount)
                {
                    card.Find("수령버튼").GetComponent<Image>().sprite = onBtnSprite;
                    card.Find("수령버튼").GetComponent<Button>().onClick.RemoveAllListeners();
                    card.Find("수령버튼").GetComponent<Button>().onClick.AddListener(() => {
                        RewardOnClick(ID, rewardTypes, rewardIds, rewardCounts);
                    });
                }
                else
                {
                    card.Find("수령버튼").GetComponent<Image>().sprite = offBtnSprite;
                }

                if (userQuest.isComplete == true)
                {
                    card.Find("수령버튼").GetChild(0).GetComponent<Text>().text = "완료";
                }
                else
                {
                    card.Find("수령버튼").GetChild(0).GetComponent<Text>().text = "수령";
                }

                card.Find("카운트").GetComponent<Text>().text = userQuest.clearCount + "/" + questCount;
            }
            else
            {
                card.gameObject.SetActive(false);
            }
        }
    }

    void RewardOnClick(int questID ,RewardType[] rewardTypes, int[] IDs , int[] countList)
    {
        UserInfo.instance.CompleteUserQuest(questID);
        int category = QuestChart.instance.GetQuestChartInfo(questID).Category;
        switch (category)
        {
            case 1:
                DayUI_Setting();
                break;
            case 2:
                AchiveUI_Setting();
                break;
            case 3:
                EventUI_Setting();
                break;
        }
        UserInfo.instance.SaveUserQuest(() => {
            Debug.Log("aa");
            PopupManager.instance.QuestRewardResult(rewardTypes, IDs, countList);
        });
    }
}
