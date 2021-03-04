using BackEnd;
using GoogleMobileAds.Api;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttendanceChart : MonoBehaviour
{
    [SerializeField] string field;
    public static AttendanceChart instance;
    private void Awake() { instance = this; }

    [SerializeField] AttendanceChartInfo[] attendanceChartInfos;

    public List<AttendanceChartInfo> GetAttendanceInfoList()
    {
        List<AttendanceChartInfo> temp = new List<AttendanceChartInfo>();
        for (int i = 0; i < attendanceChartInfos.Length; i++)
        {
            if (attendanceChartInfos[i].Sorting == i + 1)
            {
                temp.Add(attendanceChartInfos[i]);
            }
        }
        return temp;
    }

    public AttendanceChartInfo GetAttendanceInfo(int ID)
    {
        for (int i = 0; i < attendanceChartInfos.Length; i++)
        {
            if (attendanceChartInfos[i].AttendanceId == ID)
            {
                return attendanceChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            attendanceChartInfos = new AttendanceChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                attendanceChartInfos[i] = new AttendanceChartInfo();

                JsonData rowData = jsonData[i];
                if (rowData["AttendanceId"]["S"].ToString() != "null") attendanceChartInfos[i].AttendanceId = int.Parse(rowData["AttendanceId"]["S"].ToString());
                if (rowData["Sorting"]["S"].ToString() != "null") attendanceChartInfos[i].Sorting = int.Parse(rowData["Sorting"]["S"].ToString());
                if (rowData["Type"]["S"].ToString() != "null") attendanceChartInfos[i].Type = int.Parse(rowData["Type"]["S"].ToString());

                if (rowData["RewardType"]["S"].ToString() != "null")
                {
                    string[] rewardTypeStringList = rowData["RewardType"]["S"].ToString().Split('-');
                    attendanceChartInfos[i].RewardType = new RewardType[rewardTypeStringList.Length];
                    for (int j = 0; j < rewardTypeStringList.Length; j++)
                    {
                        attendanceChartInfos[i].RewardType[j] = (RewardType)(int.Parse(rewardTypeStringList[j]));
                    }
                }

                if (rowData["RewardId"]["S"].ToString() != "null")
                {
                    string[] rewardIdStringList = rowData["RewardId"]["S"].ToString().Split('-');
                    attendanceChartInfos[i].RewardId = new int[rewardIdStringList.Length];
                    for (int j = 0; j < rewardIdStringList.Length; j++)
                    {
                        attendanceChartInfos[i].RewardId[j] = int.Parse(rewardIdStringList[j]);
                    }
                }

                if (rowData["RewardCount"]["S"].ToString() != "null")
                {
                    string[] rewardCountStringList = rowData["RewardCount"]["S"].ToString().Split('-');
                    attendanceChartInfos[i].RewardCount = new int[rewardCountStringList.Length];
                    for (int j = 0; j < rewardCountStringList.Length; j++)
                    {
                        attendanceChartInfos[i].RewardCount[j] = int.Parse(rewardCountStringList[j]);
                    }
                }

                if (rowData["StartTime"]["S"].ToString() != "null")
                {
                    string[] startTimeStringList = rowData["StartTime"]["S"].ToString().Split('-');
                    int year = int.Parse(startTimeStringList[0]);
                    int month = int.Parse(startTimeStringList[1]);
                    int day = int.Parse(startTimeStringList[2]);
                    int hour = int.Parse(startTimeStringList[3]);
                    int minute = int.Parse(startTimeStringList[4]);
                    attendanceChartInfos[i].StartTime = new DateTime(year, month, day,hour,minute,0);
                }

                if (rowData["EndTime"]["S"].ToString() != "null")
                {
                    string[] endTimeStringList = rowData["EndTime"]["S"].ToString().Split('-');
                    int year = int.Parse(endTimeStringList[0]);
                    int month = int.Parse(endTimeStringList[1]);
                    int day = int.Parse(endTimeStringList[2]);
                    int hour = int.Parse(endTimeStringList[3]);
                    int minute = int.Parse(endTimeStringList[4]);
                    attendanceChartInfos[i].EndTime = new DateTime(year, month, day, hour, minute, 0);
                }
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class AttendanceChartInfo
{
    public int AttendanceId;
    public int Sorting;
    public int Type;
    public RewardType[] RewardType;
    public int[] RewardId;
    public int[] RewardCount;
    public DateTime StartTime;
    public DateTime EndTime;
}