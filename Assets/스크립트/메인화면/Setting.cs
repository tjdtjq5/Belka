using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] Transform soundSwitch;
    [SerializeField] Transform musicSwitch;
    [SerializeField] Transform vibrationSwitch;
    [SerializeField] Transform language;

    private void OnEnable()
    {
        if (TextChart.instance.nationType == NationType.Ko)
        {
            OnClickLanguage(false);
        }
        else
        {
            OnClickLanguage(true);
        }

        if (SoundManager.instance.bgmVolume > 0)
        {
            MusicSwitch(false);
        }
        else
        {
            MusicSwitch(true);
        }

        if (SoundManager.instance.sfxVolume > 0)
        {
            SoundSwitch(false);
        }
        else
        {
            SoundSwitch(true);
        }
    }

    public void SoundSwitch(bool flag)
    {
        switch (flag)
        {
            case true:
                SoundManager.instance.SfxVolumeSet(0);
                soundSwitch.Find("Off").gameObject.SetActive(true);
                soundSwitch.Find("On").gameObject.SetActive(false);
                break;
            case false:
                SoundManager.instance.SfxVolumeSet(1);
                soundSwitch.Find("Off").gameObject.SetActive(false);
                soundSwitch.Find("On").gameObject.SetActive(true);
                break;
        }
    }
    public void MusicSwitch(bool flag)
    {
        switch (flag)
        {
            case true:
                SoundManager.instance.BgmVolumeSet(0);
                musicSwitch.Find("Off").gameObject.SetActive(true);
                musicSwitch.Find("On").gameObject.SetActive(false);
                break;
            case false:
                SoundManager.instance.BgmVolumeSet(1);
                musicSwitch.Find("Off").gameObject.SetActive(false);
                musicSwitch.Find("On").gameObject.SetActive(true);
                break;
        }
    }
    public void VibrationSwitch(bool flag)
    {
        switch (flag)
        {
            case true:
                vibrationSwitch.Find("Off").gameObject.SetActive(true);
                vibrationSwitch.Find("On").gameObject.SetActive(false);
                break;
            case false:
                vibrationSwitch.Find("Off").gameObject.SetActive(false);
                vibrationSwitch.Find("On").gameObject.SetActive(true);
                break;
        }
    }
    public void OnClickLanguage(bool flag)
    {
        switch (flag)
        {
            case true: // EN
                language.Find("EN").gameObject.SetActive(true);
                language.Find("KO").gameObject.SetActive(false);
                language.Find("LanguageText").GetComponent<Text>().text = "English";
                language.Find("LanguageText").GetComponent<Text>().font = TextChart.instance.enFont;
                PlayerPrefs.SetString("NationType", "En");
                TextChart.instance.nationType = NationType.En;
                break;
            case false: // KO
                language.Find("EN").gameObject.SetActive(false);
                language.Find("KO").gameObject.SetActive(true);
                language.Find("LanguageText").GetComponent<Text>().text = "한국어";
                language.Find("LanguageText").GetComponent<Text>().font = TextChart.instance.koFont;
                PlayerPrefs.SetString("NationType", "Ko");
                TextChart.instance.nationType = NationType.Ko;
                break;
        }
    }

    public void Privacy()
    {
        PopupManager.instance.Privacy();
    }
    public void NicknameChange()
    {
        PopupManager.instance.NicknameChange();
    }
    public void LogOut()
    {
        PopupManager.instance.BtnAlram(() => {
            Backend.BMember.Logout();
            PlayerPrefs.DeleteKey("login");
            SceneManager.LoadScene("로비");
        }, "Setting_LogOut", "Logout");
    }

    [Obsolete]
    public void DeleteAccount()
    {
        PopupManager.instance.BtnAlram(() => {
            
            if (PlayerPrefs.GetString("login") == "Custom")
            {
                PlayerPrefs.DeleteKey("id");
                PlayerPrefs.DeleteKey("password");
            }

            BackendGameInfo.instance.GetPrivateContents("UserInfo", "inDate", () => {
                Backend.GameInfo.Delete("UserInfo", BackendGameInfo.instance.serverDataList[0]);
                BackendReturnObject servertime = Backend.Utils.GetServerTime();
                string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
                DateTime parsedDate = DateTime.Parse(time);
                string updateNickname = parsedDate.Year.ToString() + parsedDate.Month.ToString() + parsedDate.Day.ToString() + parsedDate.Hour.ToString() + parsedDate.Minute.ToString() + parsedDate.Second.ToString() + UnityEngine.Random.Range(0, 1000).ToString();
                Backend.BMember.UpdateNickname(updateNickname);

                Backend.BMember.SignOut();

                PlayerPrefs.DeleteKey("login");
                SceneManager.LoadScene("로비");
            });
        }, "Setting_DeleteAccount", "AccountDelete");
    }
    public void GooglePederation()
    {
        GoogleAuth();
    }

    ///


    // 구글에 로그인하기
    [Obsolete]
    private void GoogleAuth()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
       .Builder()
       .RequestServerAuthCode(false)
       .RequestEmail()                 // 이메일 요청
       .RequestIdToken()               // 토큰 요청
       .Build();

        //커스텀된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;

        //GPGS 시작.
        PlayGamesPlatform.Activate();

        if (PlayGamesPlatform.Instance.localUser.authenticated == false)
        {
            Social.localUser.Authenticate(success =>
            {
                if (success == false)
                {
                    Debug.Log("구글 로그인 실패");
                    return;
                }
                else
                {
                    Debug.Log("구글 로그인 성공");
                    OnClickChangeCustomToFederation();
                }
            });
        }
    }
    // 구글 토큰 받아오기
    private string GetTokens()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // 유저 토큰 받기 첫번째 방법
            string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
            // 두번째 방법
            // string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
            Debug.Log("토큰 받아오기 성공 : " + _IDtoken);
            return _IDtoken;
        }
        else
        {
            Debug.Log("접속되어있지 않습니다. 잠시 후 다시 시도하세요.");
            return null;
        }
    }

    // 커스텀 계정을 구글 계정으로 
    public void OnClickChangeCustomToFederation()
    {
        BackendReturnObject BRO = Backend.BMember.ChangeCustomToFederation(GetTokens(), FederationType.Google);

        if (BRO.IsSuccess())
        {
            Debug.Log("페더레이션 계정으로 변경 완료");


            if (PlayerPrefs.GetString("login") == "Custom")
            {
                PlayerPrefs.DeleteKey("id");
                PlayerPrefs.DeleteKey("password");

                PlayerPrefs.SetString("login", "Google");
            }
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "400":
                    if (BRO.GetErrorCode() == "BadParameterException")
                    {
                        Debug.Log("이미 ChangeCustomToFederation 완료 되었는데 다시 시도한 경우");
                    }

                    else if (BRO.GetErrorCode() == "UndefinedParameterException")
                    {
                        Debug.Log("customLogin 하지 않은 상황에서 시도한 경우");
                    }
                    break;

                case "409":
                    // 이미 가입되어 있는 경우
                    Debug.Log("Duplicated federationId, 중복된 federationId 입니다");
                    PopupManager.instance.Alram("Error_05");
                    break;
            }
        }
    }

}
