using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    public static UserInfo instance;
    private void Awake() { instance = this; }

    public string nickname;
    List<UserStageInfo> userStageInfos = new List<UserStageInfo>();
    UserHeartInfo userHeartInfo = null;
    int userCrystal;
    UserCharacterInfo userCharacterInfo = new UserCharacterInfo();
    List<UserItemInfo> userItemInfos = new List<UserItemInfo>();
    [ContextMenu("test")]
    public void Test()
    {
        for (int i = 0; i < userStageInfos.Count; i++)
        {
            Debug.Log("스테이지 id  :  " + userStageInfos[i].stageId);
        }
    }
    [Obsolete]
    public void AllLoadUserInfo(System.Action loadCallback)
    {
        LoadUserStageInfo(() => {
            LoadUserHeartInfo(() => {
                LoadUserCrystal(() => {
                    LoadUserCharacterInfo(() => {
                        LoadUserItemInfo(() => {                 
                        loadCallback();  });
                    });
                });
            });
        });
    }

    // 유저 스테이지 정보 : 어떤 스테이지를 깼는지, 클리어타임, 받은 별 개수 
    [System.Obsolete]
    public void SaveUserStageInfo(System.Action saveCallback)
    {
        Param param = new Param();
        List<string> dataList = new List<string>();
        for (int i = 0; i < userStageInfos.Count; i++)
        {
            dataList.Add(userStageInfos[i].stageId + "-" + userStageInfos[i].clearTime + "-" + userStageInfos[i].starCount);
        }
        param.Add("Stage", dataList);
        BackendGameInfo.instance.PrivateTableUpdate("UserInfo", param, ()=> { saveCallback(); });
    }
    [System.Obsolete]
    public void LoadUserStageInfo(System.Action loadCallback)
    {
        userStageInfos.Clear();
        BackendGameInfo.instance.GetPrivateContents("UserInfo", "Stage", () =>
        {
            
            for (int i = 0; i < BackendGameInfo.instance.serverDataList.Count; i++)
            {
                string[] data = BackendGameInfo.instance.serverDataList[i].Split('-');

                int stageId = int.Parse(data[0]);
                float clearTime = float.Parse(data[1]);
                int starCount = int.Parse(data[2]);

                userStageInfos.Add(new UserStageInfo(stageId, clearTime, starCount));
            }
            loadCallback();
        }, () => {
            loadCallback();
        });
    }
    public UserStageInfo GetUserStageInfo(int stageId)
    {
        for (int i = 0; i < userStageInfos.Count; i++)
        {
            if (userStageInfos[i].stageId == stageId)
            {
                return userStageInfos[i];
            }
        }
        return null;
    }
    public List<UserStageInfo> UserStageGroupInfo(int stageGroupId)
    {
        List<UserStageInfo> tempUserStageList = new List<UserStageInfo>();
        for (int i = 0; i < userStageInfos.Count; i++)
        {
            if (userStageInfos[i].stageGroupId == stageGroupId)
            {
                tempUserStageList.Add(userStageInfos[i]);
            }
        }
        return tempUserStageList;
    }
    public void PutUserStage(int stageId, float clearTime, int starCount)
    {
        userStageInfos.Add(new UserStageInfo(stageId, clearTime, starCount));
    }
    // 하트 정보 : 하트 개수, 하트를 받은 시간 
    public void SaveUserHeartInfo(System.Action saveCallback)
    {
        if (userHeartInfo == null) return;

        Param param = new Param();
        int numberOfHeart = userHeartInfo.numberOfHeart;
        int year = userHeartInfo.reciveTime.Year;
        int month = userHeartInfo.reciveTime.Month;
        int day = userHeartInfo.reciveTime.Day;
        int hour = userHeartInfo.reciveTime.Hour;
        int minute = userHeartInfo.reciveTime.Minute;
        int second = userHeartInfo.reciveTime.Second;

        param.Add("Heart", numberOfHeart + "-" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second);
        BackendGameInfo.instance.PrivateTableUpdate("UserInfo", param, () => { saveCallback(); });
    }
    [Obsolete]
    public void LoadUserHeartInfo(System.Action loadCallback)
    {
        BackendGameInfo.instance.GetPrivateContents("UserInfo", "Heart", () =>
        {
            string[] data = BackendGameInfo.instance.serverDataList[0].Split('-');
       
            int numberOfHeart = int.Parse(data[0]);
            int year = int.Parse(data[1]);
            int month = int.Parse(data[2]);
            int day = int.Parse(data[3]);
            int hour = int.Parse(data[4]);
            int minute = int.Parse(data[5]);
            int second = int.Parse(data[6]);

            userHeartInfo = new UserHeartInfo(numberOfHeart, new DateTime(year, month, day, hour, minute, second));

            loadCallback();
        }, () => {
            userHeartInfo = new UserHeartInfo();
            userHeartInfo.numberOfHeart = 999;
            SaveUserHeartInfo(() => { loadCallback(); });
        });
    }
    public UserHeartInfo GetUserHeartInfo()
    {
        return userHeartInfo;
    }
    public void PutUserHeart(int heart)
    {
        userHeartInfo.Put(heart);
    }
    public void PullUserHeart(int heart)
    {
        userHeartInfo.Pull(heart);
    }
    // 크리트탈 정보 : 유저가 가지고 있는 크리스탈 개수
    public void SaveUserCrystal(System.Action saveCallback)
    {
        Param param = new Param();
        param.Add("Crystal", userCrystal);
        BackendGameInfo.instance.PrivateTableUpdate("UserInfo", param, () => { saveCallback(); });
    }
    [Obsolete]
    public void LoadUserCrystal(System.Action loadCallback)
    {
        BackendGameInfo.instance.GetPrivateContents("UserInfo", "Crystal", () =>
        {
            string data = BackendGameInfo.instance.serverDataList[0];
            userCrystal = int.Parse(data);
            loadCallback();
        }, () => {
            userCrystal = 99999;
            SaveUserCrystal(() => { loadCallback(); });
        });
    }
    public int GetUserCrystal()
    {
        return userCrystal;
    }
    public void PutUserCrystal(int crystal)
    {
        userCrystal += crystal;
    }
    public void PullUserCrystal(int crystal)
    {
        userCrystal -= crystal;
    }
    // 캐릭터 정보 : 장착중인 캐릭터, 각 캐릭터의 등급
    public void SaveUserCharacterInfo(System.Action saveCallback)
    {
        Param param = new Param();
        int eqipCharacter = (int)userCharacterInfo.eqipCharacter;

        string characterGrade = "";
        for (int i = 0; i < userCharacterInfo.characterGrade.Length; i++)
        {
            characterGrade += (int)userCharacterInfo.characterGrade[i];
            if (i != userCharacterInfo.characterGrade.Length - 1)
            {
                characterGrade += "-";
            }
        }
        param.Add("Character", eqipCharacter + "-" + characterGrade);
        BackendGameInfo.instance.PrivateTableUpdate("UserInfo", param, () => { saveCallback(); });
    }
    [Obsolete]
    public void LoadUserCharacterInfo(System.Action loadCallback)
    {
        BackendGameInfo.instance.GetPrivateContents("UserInfo", "Character", () =>
        {
            string[] data = BackendGameInfo.instance.serverDataList[0].Split('-');
            userCharacterInfo.eqipCharacter = (CharacterType)int.Parse(data[0]);
            for (int i = 0; i < userCharacterInfo.characterGrade.Length; i++)
            {
                userCharacterInfo.characterGrade[i] = (GradeType)int.Parse(data[i + 1]);
            }
            loadCallback();
        }, () => {
            userCharacterInfo.eqipCharacter = (CharacterType)1;
            for (int i = 0; i < userCharacterInfo.characterGrade.Length; i++)
            {
                userCharacterInfo.characterGrade[i] = (GradeType)1;
            }
            SaveUserCharacterInfo(() => { loadCallback(); });
        });
    }
    public UserCharacterInfo GetUserCharacterInfo()
    {
        return userCharacterInfo;
    }

    // 아이템 정보 : 아이템 아이디, 아이템 수 
    [Obsolete]
    public void SaveUserItemInfo(System.Action saveCallback)
    {
        Param param = new Param();
        List<string> dataList = new List<string>();
        for (int i = 0; i < userItemInfos.Count; i++)
        {
            dataList.Add(userItemInfos[i].itemId + "-" + userItemInfos[i].numberOfItem);
        }
        param.Add("Item", dataList);

        BackendGameInfo.instance.PrivateTableUpdate("UserInfo", param, () => { saveCallback(); });
    }
    [System.Obsolete]
    public void LoadUserItemInfo(System.Action loadCallback)
    {
        userItemInfos.Clear();
        BackendGameInfo.instance.GetPrivateContents("UserInfo", "Item", () =>
        {
            for (int i = 0; i < BackendGameInfo.instance.serverDataList.Count; i++)
            {
                string[] data = BackendGameInfo.instance.serverDataList[i].Split('-');

                int itemId = int.Parse(data[0]);
                int numberOfItem = int.Parse(data[1]);

                userItemInfos.Add(new UserItemInfo(itemId, numberOfItem));

                loadCallback();
            }
        }, () => {
            PutUserItem(211, 10);
            PutUserItem(212, 10);
            PutUserItem(213, 10);
            PutUserItem(214, 10);
            PutUserItem(215, 10);
            SaveUserItemInfo(() => { loadCallback(); });
        });
    }
    public void PutUserItem(int itemId, int num = 1)
    {
        for (int i = 0; i < userItemInfos.Count; i++)
        {
            if (userItemInfos[i].itemId == itemId)
            {
                userItemInfos[i].numberOfItem += num;
                return;
            }
        }
        userItemInfos.Add(new UserItemInfo(itemId, num));
    }
    public List<UserItemInfo> GetUserItemInfos()
    {
        return userItemInfos;
    }
    public UserItemInfo GetUserItemInfo(int itemid)
    {
        for (int i = 0; i < userItemInfos.Count; i++)
        {
            if (userItemInfos[i].itemId == itemid)
            {
                return userItemInfos[i];
            }
        }
        return null;
    }
    public void SetUserItemInfo(int itemid, int num)
    {
        for (int i = 0; i < userItemInfos.Count; i++)
        {
            if (userItemInfos[i].itemId == itemid)
            {
                if (num <= 0)
                {
                    userItemInfos.RemoveAt(i);
                    return;
                }
                userItemInfos[i].numberOfItem = num;
                return;
            }
        }
        PutUserItem(itemid, num);
    }

    // 랭킹 접속 카운트
    public int GetRemainRankingCount()
    {
        BackendReturnObject bro = Backend.Utils.GetServerTime();
        string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime currentTime = DateTime.Parse(time);
        TimeSpan timeSpan = new TimeSpan(currentTime.Ticks);
        string key = ((int)timeSpan.TotalDays).ToString();

        if (PlayerPrefs.HasKey(key))
        {
            return PlayerPrefs.GetInt(key);
        }
        else
        {
            PlayerPrefs.SetInt(key, ConfigChart.instance.configChartInfo.RankingDailyCount);
            return PlayerPrefs.GetInt(key);
        }
    }
    public void RankingCounting()
    {
        BackendReturnObject bro = Backend.Utils.GetServerTime();
        string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime currentTime = DateTime.Parse(time);
        TimeSpan timeSpan = new TimeSpan(currentTime.Ticks);
        string key = ((int)timeSpan.TotalDays).ToString();
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, (PlayerPrefs.GetInt(key) - 1));
        }
        else
        {
            PlayerPrefs.SetInt(key, ConfigChart.instance.configChartInfo.RankingDailyCount - 1);
        }
    }
}
public class UserStageInfo
{
    public int stageId;
    public int stageGroupId;
    public int stageGroupInId;
    public float clearTime;
    public int starCount;

    public UserStageInfo(int stageId , float clearTime, int starCount)
    {
        this.stageId = stageId;
        stageGroupId = StageChart.instance.GetStageChartInfo(stageId).StageGroupId;
        stageGroupInId = StageChart.instance.GetStageChartInfo(stageId).StageGroupInId;
        this.clearTime = clearTime;
        this.starCount = starCount;
    }
}
public class UserHeartInfo
{
    int maxHeart = ConfigChart.instance.configChartInfo.heartMax;
    public int numberOfHeart;
    public DateTime reciveTime;

    public UserHeartInfo()
    {
        BackendReturnObject servertime = Backend.Utils.GetServerTime();
        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime dateTime = DateTime.Parse(time);

        numberOfHeart = maxHeart;
        reciveTime = dateTime;
    }
    public UserHeartInfo(int numberOfHeart, DateTime reciveTime)
    {
        this.numberOfHeart = numberOfHeart;
        this.reciveTime = reciveTime;
    }
    public void Put(int num)
    {
        numberOfHeart += num;
    }
    public void Pull(int num)
    {
        numberOfHeart -= num;
    }
}
public class UserCharacterInfo
{
    public CharacterType eqipCharacter;
    public GradeType[] characterGrade;

    public UserCharacterInfo()
    {
        eqipCharacter = CharacterType.없음;
        characterGrade = new GradeType[System.Enum.GetValues(typeof(CharacterType)).Length];
        for (int i = 0; i < characterGrade.Length; i++)
        {
            characterGrade[i] = GradeType.없음;
        }
    }
}
public class UserItemInfo
{
    public int itemId;
    public int numberOfItem;

    public UserItemInfo(int itemId, int numberOfItem)
    {
        this.itemId = itemId;
        this.numberOfItem = numberOfItem;
    }
}