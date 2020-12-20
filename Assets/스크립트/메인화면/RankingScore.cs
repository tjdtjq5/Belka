using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingScore : MonoBehaviour
{
    public Transform content;
    public Transform userRankObj;

    public void Open()
    {
        OnClickToday();
    }

    void contentDelete()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    public void OnClickToday()
    {
        contentDelete();

        GameObject rankingScorePrepab;
        int min;
        int second;
        RankingManager.instance.GetRankingListToday(() => {
            List<RankingUserInfo> rankingUserInfos = RankingManager.instance.rankingUserInfos;
            int len = rankingUserInfos.Count;
            if (len > 100) len = 100;
            for (int i = 0; i < len; i++)
            {
                rankingScorePrepab = Resources.Load<GameObject>("프리팹/랭킹유저정보프리팹") as GameObject;
                rankingScorePrepab = Instantiate(rankingScorePrepab, Vector2.zero, Quaternion.identity, content);
                rankingScorePrepab.transform.Find("rank").GetComponent<Text>().text = rankingUserInfos[i].rank.ToString(); 
                rankingScorePrepab.transform.Find("nickname").GetComponent<Text>().text = rankingUserInfos[i].nickname; 
                min = (int)rankingUserInfos[i].score / 60; 
                second = (int)rankingUserInfos[i].score % 60;
                rankingScorePrepab.transform.Find("score").GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}", min, second); 
            }
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, 100 * len);

            TodayUserRank();
        });
    }
    public void OnClickYesterDay()
    {
        contentDelete();

        GameObject rankingScorePrepab;
        int min;
        int second;
        RankingManager.instance.GetRankingListYesterday(() => {
            List<RankingUserInfo> rankingUserInfos = RankingManager.instance.rankingUserInfos;
            int len = rankingUserInfos.Count;
            if (len > 100) len = 100;
            for (int i = 0; i < len; i++)
            {
                rankingScorePrepab = Resources.Load<GameObject>("프리팹/랭킹유저정보프리팹") as GameObject;
                rankingScorePrepab = Instantiate(rankingScorePrepab, Vector2.zero, Quaternion.identity, content);
                rankingScorePrepab.transform.Find("rank").GetComponent<Text>().text = rankingUserInfos[i].rank.ToString();
                rankingScorePrepab.transform.Find("nickname").GetComponent<Text>().text = rankingUserInfos[i].nickname;
                min = (int)rankingUserInfos[i].score / 60;
                second = (int)rankingUserInfos[i].score % 60;
                rankingScorePrepab.transform.Find("score").GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}", min, second);
            }
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(content.GetComponent<RectTransform>().sizeDelta.x, 100 * len);
            YesterDayUserRank();
        });
    }

    [System.Obsolete]
    void TodayUserRank()
    {
        RankingManager.instance.GetMyRankingToday(() => {
            List<RankingUserInfo> rankingUserInfos = RankingManager.instance.rankingUserInfos;
            if (rankingUserInfos.Count > 0)
            {
                userRankObj.Find("rank").GetComponent<Text>().text = rankingUserInfos[0].rank.ToString();
                userRankObj.Find("nickname").GetComponent<Text>().text = rankingUserInfos[0].nickname;
                int min = (int)rankingUserInfos[0].score / 60;
                int second = (int)rankingUserInfos[0].score % 60;
                userRankObj.Find("score").GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}", min, second);
            }
            else
            {
                userRankObj.Find("rank").GetComponent<Text>().text = "--";
                userRankObj.Find("nickname").GetComponent<Text>().text = UserInfo.instance.nickname;
                userRankObj.Find("score").GetComponent<Text>().text = "--:--";
            }
           
        });
    }
    [System.Obsolete]
    void YesterDayUserRank()
    {
        RankingManager.instance.GetMyRankingYesterDay(() => {
            List<RankingUserInfo> rankingUserInfos = RankingManager.instance.rankingUserInfos;
            if (rankingUserInfos.Count > 0)
            {
                userRankObj.Find("rank").GetComponent<Text>().text = rankingUserInfos[0].rank.ToString();
                userRankObj.Find("nickname").GetComponent<Text>().text = rankingUserInfos[0].nickname;
                int min = (int)rankingUserInfos[0].score / 60;
                int second = (int)rankingUserInfos[0].score % 60;
                userRankObj.Find("score").GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}", min, second);
            }
            else
            {
                userRankObj.Find("rank").GetComponent<Text>().text = "--";
                userRankObj.Find("nickname").GetComponent<Text>().text = UserInfo.instance.nickname;
                userRankObj.Find("score").GetComponent<Text>().text = "--:--";
            }
        });
    }
}
