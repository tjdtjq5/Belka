using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackendInitial : MonoBehaviour
{
    public LobbyLoding lobbyLoding;
    public GameObject[] loginBtn;
    void Awake()
    {
        Backend.Initialize(HandleBackendCallback);
    }

    void HandleBackendCallback()
    {
        if(Backend.IsInitialized)
        {
            Debug.Log("뒤끝SDK 초기화 완료");

            string googleKey = Backend.Utils.GetGoogleHash();
            for (int i = 0; i < googleKey.Length; i++)
            {
                if (googleKey[i] == 'l')
                {
                    Debug.Log("L의 소문자 입니다. ");
                }
                if (googleKey[i] == 'I')
                {
                    Debug.Log("i의 대문자 입니다. ");
                }
                Debug.Log(googleKey[i]);
            }

            if (PlayerPrefs.HasKey("login"))
            {
                switch (PlayerPrefs.GetString("login"))
                {
                    case "Custom":
                        CustomLogin(() => { lobbyLoding.Loading(); });
                        break;
                    case "Google":
                        GoogleAuth(() => { lobbyLoding.Loading(); });
                        break;
                    default:
                        for (int i = 0; i < loginBtn.Length; i++)
                        {
                            loginBtn[i].gameObject.SetActive(true);
                        }
                        break;
                }
            }
            else
            {
                // 버튼을 눌러서 로그인 
                for (int i = 0; i < loginBtn.Length; i++)
                {
                    loginBtn[i].gameObject.SetActive(true);
                }
            }
        }
        // 실패
        else
        {
            Debug.LogError("Failed to initialize the backend");
        }
    }

    public void OnClickGuest()
    {
        if (!PlayerPrefs.HasKey("id") || !PlayerPrefs.HasKey("password"))
        {
            CustumSignUp();
        }
        else
        {
            CustomLogin(()=> { lobbyLoding.Loading(); });
        }
    }
    public void OnClickGoogle()
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
        GoogleAuth(() => { lobbyLoding.Loading(); });
    }
    [System.Obsolete]
    public void CustumSignUp()
    {
        BackendReturnObject servertime = Backend.Utils.GetServerTime();
        string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime parsedDate = DateTime.Parse(time);

        string id = parsedDate.Year.ToString() + parsedDate.Month.ToString() + parsedDate.Day.ToString()+ parsedDate.Hour.ToString() + parsedDate.Minute.ToString() + parsedDate.Second.ToString() + UnityEngine.Random.Range(0,1000).ToString();
        string password = time;
        PlayerPrefs.SetString("id", id);
        PlayerPrefs.SetString("password", password);

        BackendAsyncClass.BackendAsync(Backend.BMember.CustomSignUp, id, password, (callback) =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("회원가입 완료");
                PlayerPrefs.SetString("login", "Custom");
                CustomLogin(() => { lobbyLoding.Loading(); });
            }
        });
    }
    [System.Obsolete]
    public void CustomLogin(System.Action loginCallback)
    {
        if (!PlayerPrefs.HasKey("id")|| !PlayerPrefs.HasKey("password"))
        {
            Debug.Log("id, password 데이터가 없습니다.");
            PlayerPrefs.SetString("login", "False");
            return;
        }
        Debug.Log("CustomLogin");
        BackendAsyncClass.BackendAsync(Backend.BMember.CustomLogin, PlayerPrefs.GetString("id"), PlayerPrefs.GetString("password"), (callback) =>
        {
            if (callback.IsSuccess())
            {
                BackendAsyncClass.BackendAsync(Backend.BMember.GetUserInfo, (callback1) =>
                {
                    if (callback1.IsSuccess())
                    {
                        JsonData json = callback1.GetReturnValuetoJSON()["row"];

                        if (json["nickname"] != null)
                        {
                            string nickName = json["nickname"].ToString();
                            UserInfo.instance.nickname = nickName;
                            string indate = json["inDate"].ToString();
                            Debug.Log("로그인 성공 닉네임 : " + nickName);
                            PlayerPrefs.SetString("login", "Custom");
                            loginCallback();
                            //lobbyLoding.Loading();
                        }
                        else
                        {
                            BackendAsyncClass.BackendAsync(Backend.BMember.CreateNickname, PlayerPrefs.GetString("id"), (callback2) =>
                            {
                                if (callback2.IsSuccess())
                                {
                                    Debug.Log("닉네임 만들기 성공");
                                    UserInfo.instance.nickname = PlayerPrefs.GetString("id");
                                    PlayerPrefs.SetString("login", "Custom");
                                    loginCallback();
                                }
                                else
                                {
                                    switch (callback2.GetStatusCode())
                                    {
                                        case "400":
                                            Debug.Log("빈 닉네임으로 생성");
                                            Debug.Log("또는 20자 이상의 닉네임");
                                            Debug.Log("닉네임 앞 뒤 공백이 있는 경우");
                                            Debug.Log("에러 메세지" + callback2.GetMessage());
                                            break;
                                        case "409":
                                            Debug.Log("이미 중복된 닉네임");
                                            break;
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        Debug.Log(callback1.GetErrorCode());
                    }
                });
            }
            else
            {
                switch (callback.GetStatusCode())
                {
                    case "401": // 잘못된 아이디 
                        CustumSignUp();
                        return;
                    case "403":
                        Debug.Log("콘솔차단 및 au 10 초과");
                        return;
                }
            }
        });
    }

    // 구글에 로그인하기
    [Obsolete]
    private void GoogleAuth(System.Action loginAction)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated == false)
        {
            Social.localUser.Authenticate(success =>
            {
                if (success == false)
                {
                    Debug.Log("구글 로그인 실패");
                    return;
                }
                /*
                // 로그인이 성공되었습니다.
                Debug.Log("GetIdToken - " + PlayGamesPlatform.Instance.GetIdToken());
                Debug.Log("Email - " + ((PlayGamesLocalUser)Social.localUser).Email);
                Debug.Log("GoogleId - " + Social.localUser.id);
                Debug.Log("UserName - " + Social.localUser.userName);
                Debug.Log("UserName - " + PlayGamesPlatform.Instance.GetUserDisplayName());*/

                BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs로 만든계정");
                if (BRO.IsSuccess())
                {
                    Debug.Log("구글 로그인 동기화 성공");
             

                    BackendAsyncClass.BackendAsync(Backend.BMember.GetUserInfo, (callback1) =>
                    {
                        if (callback1.IsSuccess())
                        {
                            JsonData json = callback1.GetReturnValuetoJSON()["row"];

                            if (json["nickname"] != null)
                            {
                                string nickName = json["nickname"].ToString();
                                UserInfo.instance.nickname = nickName;
                                string indate = json["inDate"].ToString();
                                Debug.Log("로그인 성공 닉네임 : " + nickName);
                                PlayerPrefs.SetString("login", "Google");
                                loginAction();
                                //lobbyLoding.Loading();
                            }
                            else
                            {
                                BackendReturnObject servertime = Backend.Utils.GetServerTime();
                                string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
                                DateTime parsedDate = DateTime.Parse(time);
                                string nickname = parsedDate.Year.ToString() + parsedDate.Month.ToString() + parsedDate.Day.ToString() + parsedDate.Hour.ToString() + parsedDate.Minute.ToString() + parsedDate.Second.ToString() + UnityEngine.Random.Range(0, 1000).ToString();

                                BackendAsyncClass.BackendAsync(Backend.BMember.CreateNickname, nickname, (callback2) =>
                                {
                                    if (callback2.IsSuccess())
                                    {
                                        Debug.Log("닉네임 만들기 성공");
                                        UserInfo.instance.nickname = nickname;
                                        PlayerPrefs.SetString("login", "Google");
                                        loginAction();
                                    }
                                    else
                                    {
                                        switch (callback2.GetStatusCode())
                                        {
                                            case "400":
                                                Debug.Log("빈 닉네임으로 생성");
                                                Debug.Log("또는 20자 이상의 닉네임");
                                                Debug.Log("닉네임 앞 뒤 공백이 있는 경우");
                                                Debug.Log("에러 메세지" + callback2.GetMessage());
                                                break;
                                            case "409":
                                                Debug.Log("이미 중복된 닉네임");
                                                break;
                                        }
                                    }
                                });
                            }
                        }
                        else
                        {
                            Debug.Log(callback1.GetErrorCode());
                        }
                    });
                }
                else
                {
                    Debug.Log("구글 로그인 동기화 실패");
                    Debug.Log(BRO.GetStatusCode());
                    Debug.Log(BRO.GetMessage());
                    switch (BRO.GetStatusCode())
                    {
                        case "200":
                            Debug.Log("이미 회원가입된 회원");
                            break;

                        case "403":
                            Debug.Log("차단된 사용자 입니다. 차단 사유 : " + BRO.GetErrorCode());
                            break;
                    }
                }
            });
        }
        else
        {
            BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs로 만든계정");
            if (BRO.IsSuccess())
            {
                Debug.Log("구글 로그인 동기화 성공");


                BackendAsyncClass.BackendAsync(Backend.BMember.GetUserInfo, (callback1) =>
                {
                    if (callback1.IsSuccess())
                    {
                        JsonData json = callback1.GetReturnValuetoJSON()["row"];

                        if (json["nickname"] != null)
                        {
                            string nickName = json["nickname"].ToString();
                            string indate = json["inDate"].ToString();
                            Debug.Log("로그인 성공 닉네임 : " + nickName);
                            PlayerPrefs.SetString("login", "Google");
                            loginAction();
                            //lobbyLoding.Loading();
                        }
                        else
                        {
                            BackendReturnObject servertime = Backend.Utils.GetServerTime();
                            string time = servertime.GetReturnValuetoJSON()["utcTime"].ToString();
                            DateTime parsedDate = DateTime.Parse(time);
                            string nickname = parsedDate.Year.ToString() + parsedDate.Month.ToString() + parsedDate.Day.ToString() + parsedDate.Hour.ToString() + parsedDate.Minute.ToString() + parsedDate.Second.ToString() + UnityEngine.Random.Range(0, 1000).ToString();

                            BackendAsyncClass.BackendAsync(Backend.BMember.CreateNickname, nickname, (callback2) =>
                            {
                                if (callback2.IsSuccess())
                                {
                                    Debug.Log("닉네임 만들기 성공");
                                    PlayerPrefs.SetString("login", "Google");
                                    loginAction();
                                }
                                else
                                {
                                    switch (callback2.GetStatusCode())
                                    {
                                        case "400":
                                            Debug.Log("빈 닉네임으로 생성");
                                            Debug.Log("또는 20자 이상의 닉네임");
                                            Debug.Log("닉네임 앞 뒤 공백이 있는 경우");
                                            Debug.Log("에러 메세지" + callback2.GetMessage());
                                            break;
                                        case "409":
                                            Debug.Log("이미 중복된 닉네임");
                                            break;
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        Debug.Log(callback1.GetErrorCode());
                    }
                });
            }
            else
            {
                Debug.Log("구글 로그인 동기화 실패");
                Debug.Log(BRO.GetStatusCode());
                Debug.Log(BRO.GetMessage());
                switch (BRO.GetStatusCode())
                {
                    case "200":
                        Debug.Log("이미 회원가입된 회원");
                        break;

                    case "403":
                        Debug.Log("차단된 사용자 입니다. 차단 사유 : " + BRO.GetErrorCode());
                        break;
                }
            }
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
                    break;
            }
        }
    }
}
