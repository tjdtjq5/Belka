using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;


public class QuestChart : MonoBehaviour
{
    [SerializeField] string field;
    public static QuestChart instance;
    private void Awake() { instance = this; }

    [SerializeField] public QuestChartInfo[] questChartInfos;

    public QuestChartInfo GetQuestChartInfo(int ID)
    {
        for (int i = 0; i < questChartInfos.Length; i++)
        {
            if (questChartInfos[i].QuestID == ID)
            {
                TimeSpan startTimeSpan = new TimeSpan(questChartInfos[i].StartTime.Ticks);
                TimeSpan endTimeSpan = new TimeSpan(questChartInfos[i].EndTime.Ticks);
                TimeSpan currentTimeSpan = new TimeSpan(CurrentTime.instance.currentTime.Ticks);

                if (endTimeSpan.TotalSeconds == 0)
                {
                    return questChartInfos[i];
                }
                else
                {
                    if (startTimeSpan < currentTimeSpan && currentTimeSpan < endTimeSpan)
                    {
                        return questChartInfos[i];
                    }
                }
                return null;
            }
        }
        return null;
    }
    public List<QuestChartInfo> GetCategoryQuest(int category)
    {
        int sort = 1;
        TimeSpan currentTimeSpan = new TimeSpan(CurrentTime.instance.currentTime.Ticks);
        List<QuestChartInfo> temp = new List<QuestChartInfo>();

        for (int i = 0; i < questChartInfos.Length; i++)
        {
            if (questChartInfos[i].Category == category && questChartInfos[i].Sorting == sort)
            {
                TimeSpan startTimeSpan = new TimeSpan(questChartInfos[i].StartTime.Ticks);
                TimeSpan endTimeSpan = new TimeSpan(questChartInfos[i].EndTime.Ticks);

                if (endTimeSpan.TotalSeconds == 0)
                {
                    sort++;
                    temp.Add(questChartInfos[i]);
                }
                else
                {
                    if (startTimeSpan < currentTimeSpan && currentTimeSpan < endTimeSpan)
                    {
                        sort++;
                        temp.Add(questChartInfos[i]);
                    }
                }
            }
        }
        return temp;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            questChartInfos = new QuestChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                questChartInfos[i] = new QuestChartInfo();

                JsonData rowData = jsonData[i];
                if (rowData["QuestID"]["S"].ToString() != "null") questChartInfos[i].QuestID = int.Parse(rowData["QuestID"]["S"].ToString());
                if (rowData["Category"]["S"].ToString() != "null") questChartInfos[i].Category = int.Parse(rowData["Category"]["S"].ToString());
                if (rowData["Sorting"]["S"].ToString() != "null") questChartInfos[i].Sorting = int.Parse(rowData["Sorting"]["S"].ToString());
                if (rowData["QuestName"]["S"].ToString() != "null") questChartInfos[i].QuestName = rowData["QuestName"]["S"].ToString();
                if (rowData["QuestCondition"]["S"].ToString() != "null") questChartInfos[i].QuestCondition = rowData["QuestCondition"]["S"].ToString();
                if (rowData["QuestCondition2"]["S"].ToString() != "null") questChartInfos[i].QuestCondition2 = int.Parse(rowData["QuestCondition2"]["S"].ToString());
                if (rowData["QuestCount"]["S"].ToString() != "null") questChartInfos[i].QuestCount = int.Parse(rowData["QuestCount"]["S"].ToString());

                if (rowData["RewardType"]["S"].ToString() != "null")
                {
                    string[] rewardTypeStringList = rowData["RewardType"]["S"].ToString().Split('-');
                    questChartInfos[i].RewardType = new RewardType[rewardTypeStringList.Length];
                    for (int j = 0; j < rewardTypeStringList.Length; j++)
                    {
                        questChartInfos[i].RewardType[j] = (RewardType)(int.Parse(rewardTypeStringList[j]));
                    }
                }

                if (rowData["RewardId"]["S"].ToString() != "null")
                {
                    string[] rewardIdStringList = rowData["RewardId"]["S"].ToString().Split('-');
                    questChartInfos[i].RewardId = new int[rewardIdStringList.Length];
                    for (int j = 0; j < rewardIdStringList.Length; j++)
                    {
                        questChartInfos[i].RewardId[j] = int.Parse(rewardIdStringList[j]);
                    }
                }

                if (rowData["RewardCount"]["S"].ToString() != "null")
                {
                    string[] rewardCountStringList = rowData["RewardCount"]["S"].ToString().Split('-');
                    questChartInfos[i].RewardCount = new int[rewardCountStringList.Length];
                    for (int j = 0; j < rewardCountStringList.Length; j++)
                    {
                        questChartInfos[i].RewardCount[j] = int.Parse(rewardCountStringList[j]);
                    }
                }

                if (rowData["StartTime"]["S"].ToString() != "null")
                {
                    string[] startTimeStringList = rowData["StartTime"]["S"].ToString().Split('-');
                    int year = int.Parse(startTimeStringList[0]);
                    int month = int.Parse(startTimeStringList[1]);
                    int day = int.Parse(startTimeStringList[2]);
                    questChartInfos[i].StartTime = new DateTime(year, month, day);
                }

                if (rowData["EndTime"]["S"].ToString() != "null")
                {
                    string[] endTimeStringList = rowData["EndTime"]["S"].ToString().Split('-');
                    int year = int.Parse(endTimeStringList[0]);
                    int month = int.Parse(endTimeStringList[1]);
                    int day = int.Parse(endTimeStringList[2]);
                    questChartInfos[i].EndTime = new DateTime(year, month, day);
                }
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class QuestChartInfo
{
    public int QuestID;
    public int Category;
    public int Sorting;
    public string QuestName;
    public string QuestCondition;
    public int QuestCondition2;
    public int QuestCount;
    public RewardType[] RewardType = null;
    public int[] RewardId = null;
    public int[] RewardCount = null;
    public DateTime StartTime = new DateTime();
    public DateTime EndTime = new DateTime();
}