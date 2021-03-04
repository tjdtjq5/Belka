using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttendanceManager : MonoBehaviour
{
    public GameObject prepab;
    public Transform canvus;
    public Sprite diaSprite;
    public Sprite heartSprite;

    private void Start()
    {
        UI_Open();
    }

    void UI_Open()
    {
        List<AttendanceChartInfo> attendanceChartInfos = AttendanceChart.instance.GetAttendanceInfoList();
        attendanceChartInfos.Reverse();

        for (int i = 0; i < attendanceChartInfos.Count; i++)
        {
            int ID = attendanceChartInfos[i].AttendanceId;
            DateTime currentTime = CurrentTime.instance.currentTime;
            DateTime startTime = attendanceChartInfos[i].StartTime;
            DateTime endTime = attendanceChartInfos[i].EndTime;
            int type = attendanceChartInfos[i].Type;
            RewardType[] rewardTypes = attendanceChartInfos[i].RewardType;
            int[] rewardId = attendanceChartInfos[i].RewardId;
            int[] rewardCount = attendanceChartInfos[i].RewardCount;

            UserAttendance userAttendance = UserInfo.instance.GetUserAttendance(ID);
            int attendanceCount = 0;
            bool isAttendance = false;
            if (userAttendance != null)
            {
                attendanceCount = userAttendance.count;
                if (userAttendance.attendanceTime.Year == currentTime.Year && userAttendance.attendanceTime.Month == currentTime.Month && userAttendance.attendanceTime.Day == currentTime.Day)
                {
                    isAttendance = true;
                }
            }


            if (currentTime > startTime && currentTime < endTime)
            {
                GameObject card = Instantiate(prepab, Vector3.zero, Quaternion.identity, canvus);

                card.transform.Find("나가기").GetComponent<Button>().onClick.AddListener(() => {
                    Destroy(card);
                });

                Transform rewardTransform = card.transform.Find("보상");
                for (int j = 0; j < rewardTransform.childCount; j++)
                {
                    switch (rewardTypes[j])
                    {
                        case RewardType.Goods:
                            if (rewardId[j] == 1)
                            {
                                rewardTransform.GetChild(j).Find("Item").GetComponent<Image>().sprite = heartSprite;
                            }
                            if (rewardId[j] == 2)
                            {
                                rewardTransform.GetChild(j).Find("Item").GetComponent<Image>().sprite = diaSprite;
                            }
                            break;
                        case RewardType.Item:
                            rewardTransform.GetChild(j).Find("Item").GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(rewardId[j]).Image;
                            break;
                    } // 이미지 
                    rewardTransform.GetChild(j).Find("Item").GetChild(0).GetComponent<Text>().text = rewardCount[j].ToString(); // 카운트

                    if (j <= attendanceCount)
                    {
                        rewardTransform.GetChild(j).Find("Clear").gameObject.SetActive(true);
                    }
                    else
                    {
                        rewardTransform.GetChild(j).Find("Clear").gameObject.SetActive(false);
                    }

                    if (j == attendanceCount && !isAttendance)
                    {
                        GameManager.instance.UserItemReward(rewardTypes[j], rewardId[j], rewardCount[j]);
                        UserInfo.instance.SetAttendance(ID);
                    }
                }


            }
        }

        UpperInfo.instance.CrystalSet();
    
        UserInfo.instance.SaveUserCrystal(() => {
            UserInfo.instance.SaveUserHeartInfo(() => {
                UserInfo.instance.SaveUserItemInfo(() => {
                    UserInfo.instance.SaveUserAttendance(() => { });
                });
            });
        });
    }
}
