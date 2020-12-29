using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public static RankingManager instance;
    private void Awake() { instance = this; }

    public List<RankingUserInfo> rankingUserInfos = new List<RankingUserInfo>();

    const string colum01 = "Score01"; // 홀수일
    const string colum02 = "Score02"; // 짝수일 

    [SerializeField] string[] uuid;

    [ContextMenu("테스트 점수 등록")]
    public void TestSetScore()
    {
        SetScore(50, () => { });
    }

    [ContextMenu("테스트 점수 가져오기")]
    public void TestGetScore()
    {
        GetRankingListToday(() => {

            for (int i = 0; i < rankingUserInfos.Count; i++)
            {
                Debug.Log("내 점수는" + rankingUserInfos[0].score + "     내 랭킹은" + rankingUserInfos[0].rank);
            }
        });
    }

    // 점수 등록하기 
    [Obsolete]
    public void SetScore(float score, System.Action scoreCallback = null)
    {
        //년-월-일-닉네임-점수
        BackendReturnObject servertime = Backend.Utils.GetServerTime();
        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime currentTime = DateTime.Parse(time);
        TimeSpan timeSpan = new TimeSpan(currentTime.Ticks);

        int year = currentTime.Year;
        int month = currentTime.Month;
        int day = currentTime.Day;

        long totalDays = (long)timeSpan.TotalDays;

        int row = (int)(totalDays % 8);

        string key = "Score" + row;

        Param param = new Param();
        param.Add(key, score);

        BackendGameInfo.instance.GetPrivateContents("Ranking", "inDate", () => {
            string inDate = BackendGameInfo.instance.serverDataList[0];
            BackendAsyncClass.BackendAsync(Backend.GameInfo.UpdateRTRankTable, "Ranking", key, inDate, (int)score, (updateRTRankTableCallback) => {
                switch (updateRTRankTableCallback.GetErrorCode())
                {
                    case "ForbiddenException":
                        Debug.Log("콘솔에서 실시간 랭킹을 활성화 하지 않고 갱신 요청을 한 경우");
                        return;
                    case "BadRankData":
                        Debug.Log("콘솔에서 실시간 랭킹을 생성하지 않고 갱신 요청을 한 경우");
                        Debug.Log("콘솔에서 Public 테이블로 실시간 랭킹을 생성한 경우");
                        Debug.Log("테이블 명 혹은 colum명이 존재하지 않는 경우");
                        return;
                    case "PreConditionError":
                        Debug.Log("한국시간(UTC+9) 4시 ~ 5시 사이에 실시간 랭킹 갱신 요청을 한 경우");
                        return;
                    case "ForbiddenError":
                        Debug.Log("퍼블릭테이블의 타인정보를 수정하고자 하였을 경우");
                        return;
                    case "NotFoundException":
                        Debug.Log("존재하지 않는 tableName인 경우");
                        return;
                    case "PreconditionFailed":
                        Debug.Log("비활성화 된 tableName인 경우");
                        return;
                }
                scoreCallback();
            });
        });
    }
    // 모든 유저 랭킹정보 가져오기 (오늘)
    [Obsolete]
    public void GetRankingListToday(System.Action callback)
    {
        rankingUserInfos.Clear();

        BackendReturnObject bro = Backend.Utils.GetServerTime();
        string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime dateTime = DateTime.Parse(time);
        TimeSpan timeSpan = new TimeSpan(dateTime.Ticks);
        long totalDays = (long)timeSpan.TotalDays;
        int row = (int)(totalDays % 8);

        string todayUuid = uuid[row];

        BackendAsyncClass.BackendAsync(Backend.RTRank.GetRTRankByUuid, todayUuid, 100, (rankingByUuidCallback) => {

            switch (rankingByUuidCallback.GetStatusCode())
            {
                case "200":
                    Debug.Log("랭킹이 없는 경우");
                    break;
                case "404":
                    Debug.Log("랭킹 Uuid가 틀린 경우");
                    break;
            }

            JsonData jsonData = rankingByUuidCallback.GetReturnValuetoJSON()["rows"];
            for (int i = 0; i < jsonData.Count; i++)
            {
                string nickname = jsonData[i]["nickname"].ToString();
                float score = float.Parse(jsonData[i]["score"]["N"].ToString());
                int rank = int.Parse(jsonData[i]["rank"]["N"].ToString());
                rankingUserInfos.Add(new RankingUserInfo(nickname, score, rank));
            }
            if (callback != null) callback();
        });
    }
    // 모든 유저 랭킹정보 가져오기 (어제)
    [Obsolete]
    public void GetRankingListYesterday(System.Action callback)
    {
        rankingUserInfos.Clear();

        BackendReturnObject bro = Backend.Utils.GetServerTime();
        string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime dateTime = DateTime.Parse(time);
        TimeSpan timeSpan = new TimeSpan(dateTime.Ticks);
        long totalDays = (long)timeSpan.TotalDays;
        int row = (int)(totalDays % 8) - 1;
        if (row < 0) row = 7;
        string yesterDay = uuid[row];

        BackendAsyncClass.BackendAsync(Backend.RTRank.GetRTRankByUuid, yesterDay, 100, (rankingByUuidCallback) => {

            switch (rankingByUuidCallback.GetStatusCode())
            {
                case "200":
                    Debug.Log("랭킹이 없는 경우");
                    return;
                case "404":
                    Debug.Log("랭킹 Uuid가 틀린 경우");
                    return;
            }


            JsonData jsonData = rankingByUuidCallback.GetReturnValuetoJSON()["rows"];
            for (int i = 0; i < jsonData.Count; i++)
            {
                string nickname = jsonData[i]["nickname"].ToString();
                float score = float.Parse(jsonData[i]["score"]["N"].ToString());
                int rank = int.Parse(jsonData[i]["rank"]["N"].ToString());
                rankingUserInfos.Add(new RankingUserInfo(nickname, score, rank));
            }
            if (callback != null) callback();
        });
    }
  
    // 내 랭킹정보 가져오기 (오늘)
    [Obsolete]
    public void GetMyRankingToday(System.Action callback)
    {
        rankingUserInfos.Clear();

        BackendReturnObject bro = Backend.Utils.GetServerTime();
        string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime dateTime = DateTime.Parse(time);
        TimeSpan timeSpan = new TimeSpan(dateTime.Ticks);
        long totalDays = (long)timeSpan.TotalDays;
        int row = (int)(totalDays % 8);

        string todayUuid = uuid[row];

        BackendAsyncClass.BackendAsync(Backend.RTRank.GetMyRTRank, todayUuid, (rankingByUuidCallback) => {

            switch (rankingByUuidCallback.GetStatusCode())
            {
                case "404":
                    Debug.Log("게이머가 랭킹에 없는 경우");
                    return;
            }
            JsonData jsonData = rankingByUuidCallback.GetReturnValuetoJSON()["rows"];
            for (int i = 0; i < jsonData.Count; i++)
            {
                string nickname = jsonData[i]["nickname"].ToString();
                float score = float.Parse(jsonData[i]["score"]["N"].ToString());
                int rank = int.Parse(jsonData[i]["rank"]["N"].ToString());
                rankingUserInfos.Add(new RankingUserInfo(nickname, score, rank));
            }
            if (callback != null) callback();
        });
    }

    // 내 랭킹정보 가져오기 (어제)
    [Obsolete]
    public void GetMyRankingYesterDay(System.Action callback)
    {
        rankingUserInfos.Clear();

        BackendReturnObject bro = Backend.Utils.GetServerTime();
        string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime dateTime = DateTime.Parse(time);
        TimeSpan timeSpan = new TimeSpan(dateTime.Ticks);
        long totalDays = (long)timeSpan.TotalDays;
        int row = (int)(totalDays % 8) - 1;
        if (row < 0) row = 7;
        string yesterDay = uuid[row];

        BackendAsyncClass.BackendAsync(Backend.RTRank.GetMyRTRank, yesterDay, (rankingByUuidCallback) => {

            switch (rankingByUuidCallback.GetStatusCode())
            {
                case "404":
                    Debug.Log("게이머가 랭킹에 없는 경우");
                    return;
            }
            JsonData jsonData = rankingByUuidCallback.GetReturnValuetoJSON()["rows"];
            for (int i = 0; i < jsonData.Count; i++)
            {
                string nickname = jsonData[i]["nickname"].ToString();
                float score = float.Parse(jsonData[i]["score"]["N"].ToString());
                int rank = int.Parse(jsonData[i]["rank"]["N"].ToString());
                rankingUserInfos.Add(new RankingUserInfo(nickname, score, rank));
            }
            if (callback != null) callback();
        });
    }

    // 날짜로 어떤 랭킹id인지 가져오기 
    public int GetRankingID(int year, int month, int day)
    {
        int len = RankingChart.instance.rankingChartChartInfos.Length;
        DateTime dateTime = new DateTime(year, month, day);
        TimeSpan timeSpan = new TimeSpan(dateTime.Ticks);
        long totalDay = (long)timeSpan.TotalDays;

        int rankingID = (int)(totalDay % len);
        return rankingID;
    }
}
public class RankingUserInfo
{
    public string nickname;
    public float score;
    public int rank;

    public RankingUserInfo(string nickname, float score, int rank)
    {
        this.nickname = nickname;
        this.score = score;
        this.rank = rank;
    }
}
