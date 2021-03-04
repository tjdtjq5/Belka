using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using LitJson;
using System;
using UnityEngine;
using Facebook.Unity;
using System.Collections.Generic;

public class BackendInitial : MonoBehaviour
{
    public LobbyLoding lobbyLoding;
    public GameObject guestLoginBtn;
    public GameObject googleLoginBtn;
    public GameObject facebookLoginBtn;

    void Awake()
    {
        // IsInitialized : 초기화가 됐는지 확인하는 함수
        if (!FB.IsInitialized)
        {
            //초기화가 되지 않았다면 초기화를 진행(콜백,콜백)
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            //초기화가 진행됐다면 개발자페이지의 앱을 활성화
            FB.ActivateApp();
        }

        Backend.Initialize(HandleBackendCallback);
    }

    void HandleBackendCallback()
    {
        if(Backend.IsInitialized)
        {
            Debug.Log("뒤끝SDK 초기화 완료");

          

            if (PlayerPrefs.HasKey("login"))
            {
                switch (PlayerPrefs.GetString("login"))
                {
                    case "Custom":
                        OnClickGuest();
                        break;
                    case "Google":
                        OnClickGoogle();
                        break;
                    case "Facebook":
                        OnClickFacebook();
                        break;
                    default:
                        guestLoginBtn.SetActive(true);
                        googleLoginBtn.SetActive(false);
                        facebookLoginBtn.SetActive(false);
                        break;
                }
            }
            else
            {
                // 버튼을 눌러서 로그인 
                guestLoginBtn.SetActive(true);
                facebookLoginBtn.SetActive(true);
                googleLoginBtn.SetActive(true);
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
    public void OnClickFacebook()
    {
        //잘 모르겠다. 아마 권한인것같다.
        var Perms = new List<string>() { "public_profile" };

        //로그인 됐는지 확인하는 함수
        if (!FB.IsLoggedIn)
        {
            //로그인 안됐다면 로그인하는 함수(권한,콜백)
            FB.LogInWithReadPermissions(Perms, FacebookCallback);
        }
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
                /*a
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
            Debug.Log("bb");
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
        Debug.Log("Token isSuccess :  " + PlayGamesPlatform.Instance.localUser.authenticated);
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


    // 페이스북 
    private void InitCallback()
    {
        //초기화가 됐다면
        if (FB.IsInitialized)
            //개발자페이지의 앱활성화
            FB.ActivateApp();
        else
            Debug.Log("Failed Init");
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // 앱을 일시 정지 시킨다.
            Time.timeScale = 0;
        }
        else
        {
            // 앱을 다시 플레이 시킨다.
            Time.timeScale = 1;
        }
    }

    void FBLogin()
    {
        var Perms = new List<string>() { "public_profile" };
        FB.LogInWithReadPermissions(Perms, FacebookCallback);
    }

    void FacebookCallback(ILoginResult result)
    {
        Debug.Log("페이스북 로그인 성공");
        // 페이스북에 로그인 성공

        if (result.Cancelled)
        {
            Debug.Log("User cancelled login");
        }
        else
        {
            // 토큰 정보 참조
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            // 토큰을 string 으로 변환
            string facebookToken = aToken.TokenString;

            // 뒤끝 서버에 획득한 페이스북 토큰으로 가입요청
            // 동기 방법으로 가입 요청
            BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(facebookToken, FederationType.Facebook, "페이스북 로그인");

            if (BRO.IsSuccess())
            {
                Debug.Log("페북 토큰으로 뒤끝서버 로그인 성공 - 동기 방식-");
            }
            else
            {
                switch (BRO.GetStatusCode())
                {
                    case "200":
                        Debug.Log("이미 회원가입된 회원");
                        break;

                    case "403":
                        Debug.Log("차단된 사용자 입니다. 차단 사유 : " + BRO.GetErrorCode());
                        break;

                    default:
                        Debug.Log("서버 공통 에러 발생" + BRO.GetMessage());
                        break;
                }
            }
        }
    }
}
