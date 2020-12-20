using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using System.Numerics;

public class BackendGameInfo : MonoBehaviour
{
    public static BackendGameInfo instance;
    private void Awake() { instance = this;  }
    public List<string> serverDataList = new List<string>();

    BigInteger a = new BigInteger();

    [ContextMenu("세이브")]
    public void TestSave()
    {
        List<string> testData = new List<string>();
        testData.Add("데이타1");
        testData.Add("데이타2");
        testData.Add("데이타3");
        Param test = new Param();
        test.Add("테스트", testData);
        PublicTableUpdate("aaaa", test);
    }
    [ContextMenu("로드")]
    public void TestLoad()
    {
    }


    [System.Obsolete]
    public void GetEqip()
    {
        GetPrivateContents("UserInfo", "EqipCharacter", ()=> {
            // 정보가 이미 있는경우 
            // 유정정보에 serverDataList 로드 
        }, ()=> {
           // 첫 정보 불러오기 
           // 유저정보에 검을 장비 -> Save 
        });
    }
    // 서버 테이블에 정보 넣기 
    [System.Obsolete]
    public void Insert(string table, Param param, System.Action sucess = null)
    {
        BackendAsyncClass.BackendAsync(Backend.GameInfo.Insert, table, param, (callback) => {
            string stateCode = callback.GetStatusCode();
            switch (stateCode)
            {
                case "404": // 존재하지 않는 tableName인 경우 
                    Debug.Log("존재하지 않는 tableName인 경우 ");
                    break;
                case "412": // 비활성화 된 tableName인 경우 
                    Debug.Log("비활성화 된 tableName인 경우 ");
                    break;
                case "413": // 하나의 row( column들의 집합 )이 400KB를 넘는 경우 
                    Debug.Log("하나의 row( column들의 집합 )이 400KB를 넘는 경우 ");
                    break;
                default:
                    break;
            }
            sucess();
        });
    }
    // 개인 프라이빗 테이블 정보 가져오기 
    [System.Obsolete]
    public void GetPrivateContents(string table, string key, System.Action sucess = null, System.Action fail=null)
    {
        serverDataList.Clear();
        BackendAsyncClass.BackendAsync(Backend.GameInfo.GetPrivateContents, table, (callback) => {
         
            string stateCode = callback.GetStatusCode();
            switch (stateCode)
            {
                case "200": // 성공 
                    break;
                case "404": // 존재하지 않는 tableName인 경우 
                    Debug.Log("존재하지 않는 tableName인 경우 ");
                    break;
                case "400": // private table 아닌 tableName을 입력한 경우 또는 limit이 100이상인 경우
                    Debug.Log("private table 아닌 tableName을 입력한 경우 또는 limit이 100이상인 경우");
                    break;
                case "412": // 비활성화 된 tableName인 경우 
                    Debug.Log("비활성화 된 tableName인 경우 ");
                    break;
                default:
                    break;
            }
            if (callback.GetReturnValuetoJSON()[0].Count == 0) // 테이블에 해당 유저의 정보가 아무것도 없는 경우
            {
                Param param = new Param();
                param.Add("N", 0);
                Insert(table, param, ()=> { GetPrivateContents(table, key, sucess, fail); });
            }
            else
            {
                JsonData jsonData = callback.GetReturnValuetoJSON()[0][0];
                if (jsonData.Keys.Contains(key))
                {
                    if (jsonData[key].Keys.Contains("L")) // 리스트형 데이터인지 아닌지 
                    {
                        JsonData keyData = jsonData[key][0];
                        for (int i = 0; i < keyData.Count; i++)
                        {
                            serverDataList.Add(keyData[i][0].ToString());
                        }
                    }
                    else
                    {
                        serverDataList.Add(jsonData[key][0].ToString());
                    }
                    if (sucess != null) { sucess(); }
                }
                else
                {
                    if (fail != null) fail();
                }
            }
        });
    }

    // 개인 테이블 정보 수정 
    [System.Obsolete]
    public void PrivateTableUpdate(string table, Param param, System.Action sucess = null)
    {
        GetPrivateContents(table, "inDate", () =>
        {
            BackendAsyncClass.BackendAsync(Backend.GameInfo.Update, table, serverDataList[0], param, (callback) =>
            {
                string stateCode = callback.GetStatusCode();
                switch (stateCode)
                {
                    case "405": // param에 partition, gamer_id, inDate, updatedAt 네가지 필드가 있는 경우
                        Debug.Log("param에 partition, gamer_id, inDate, updatedAt 네가지 필드가 있는 경우");
                        break;
                    case "403": // 퍼블릭테이블의 타인정보를 수정하고자 하였을 경우
                        Debug.Log("퍼블릭테이블의 타인정보를 수정하고자 하였을 경우");
                        break;
                    case "404": // 존재하지 않는 tableName인 경우
                        Debug.Log("존재하지 않는 tableName인 경우" + table);
                        break;
                    case "412": // 비활성화 된 tableName인 경우 
                        Debug.Log("비활성화 된 tableName인 경우 ");
                        break;
                    case "413": // 하나의 row( column들의 집합 )이 400KB를 넘는 경우
                        Debug.Log("하나의 row( column들의 집합 )이 400KB를 넘는 경우");
                        break;
                    default:
                        break;
                }

                if (sucess != null) sucess();
            });
        });
    }

    // 공용 테이블 정보 가져오기
    [System.Obsolete]
    public void GetPublicContents(string table, string key, System.Action sucess = null)
    {
        serverDataList.Clear();
        BackendAsyncClass.BackendAsync(Backend.GameInfo.GetPublicContents, table, (callback) => {

            switch (callback.GetStatusCode())
            {
                case "200": // 성공 
                    break;
                case "404": // 존재하지 않는 tableName인 경우 
                    Debug.Log("존재하지 않는 tableName인 경우 ");
                    break;
                case "400": // private table 아닌 tableName을 입력한 경우 또는 limit이 100이상인 경우
                    Debug.Log("public table 아닌 tableName 을 입력한 경우 또는 limit이 100이상인 경우");
                    break;
                case "412": // 비활성화 된 tableName인 경우 
                    Debug.Log("비활성화 된 tableName인 경우 ");
                    break;
                default:
                    break;
            }
            if (callback.GetReturnValuetoJSON()[0].Count == 0) // 테이블에 해당 유저의 정보가 아무것도 없는 경우
            {
                if (sucess != null) { sucess(); }
            }
            else
            {
                for (int i = 0; i < callback.GetReturnValuetoJSON()[0].Count; i++)
                {
                    JsonData jsonData = callback.GetReturnValuetoJSON()[0][i];
                    if (jsonData.Keys.Contains(key))
                    {
                        if (jsonData[key].Keys.Contains("L")) // 리스트형 데이터인지 아닌지 
                        {
                            JsonData keyData = jsonData[key][0];
                            for (int j = 0; j < keyData.Count; j++)
                            {
                                serverDataList.Add(keyData[j][0].ToString());
                            }
                        }
                        else
                        {
                            serverDataList.Add(jsonData[key][0].ToString());
                        }
                    }
                }
                if (sucess != null) { sucess(); }
            }
        });
    }
    // 공용 자신의 테이블 정보 가져오기 
    [System.Obsolete]
    public void GetMyPublicContents(string table, string key, System.Action sucess = null)
    {
        serverDataList.Clear();
        BackendAsyncClass.BackendAsync(Backend.GameInfo.GetMyPublicContents, table, (callback) => {

            switch (callback.GetStatusCode())
            {
                case "200": // 성공 
                    break;
                case "404": // 존재하지 않는 tableName인 경우 
                    Debug.Log("존재하지 않는 tableName인 경우 ");
                    break;
                case "400": // private table 아닌 tableName을 입력한 경우 또는 limit이 100이상인 경우
                    Debug.Log("public table 아닌 tableName 을 입력한 경우 또는 limit이 100이상인 경우");
                    break;
                case "412": // 비활성화 된 tableName인 경우 
                    Debug.Log("비활성화 된 tableName인 경우 ");
                    break;
                default:
                    break;
            }
            if (callback.GetReturnValuetoJSON()[0].Count == 0) // 테이블에 해당 유저의 정보가 아무것도 없는 경우
            {
                Param param = new Param();
                param.Add("N", 0);
                Insert(table, param, () => { GetMyPublicContents(table, key, sucess); });
            }
            else
            {
                for (int i = 0; i < callback.GetReturnValuetoJSON()[0].Count; i++)
                {
                    JsonData jsonData = callback.GetReturnValuetoJSON()[0][i];
                    if (jsonData.Keys.Contains(key))
                    {
                        if (jsonData[key].Keys.Contains("L")) // 리스트형 데이터인지 아닌지 
                        {
                            JsonData keyData = jsonData[key][0];
                            for (int j = 0; j < keyData.Count; j++)
                            {
                                serverDataList.Add(keyData[j][0].ToString());
                            }
                        }
                        else
                        {
                            serverDataList.Add(jsonData[key][0].ToString());
                        }
                    }
                }
                if (sucess != null) { sucess(); }
            }
        });
    }
    // 공용 테이블 수정 
    [System.Obsolete]
    public void PublicTableUpdate(string table, Param param, System.Action sucess = null)
    {
        GetMyPublicContents(table, "inDate", () =>
        {
            BackendAsyncClass.BackendAsync(Backend.GameInfo.Update, table, serverDataList[0], param, (callback) =>
            {
                string stateCode = callback.GetStatusCode();
                switch (stateCode)
                {
                    case "405": // param에 partition, gamer_id, inDate, updatedAt 네가지 필드가 있는 경우
                        Debug.Log("param에 partition, gamer_id, inDate, updatedAt 네가지 필드가 있는 경우");
                        break;
                    case "403": // 퍼블릭테이블의 타인정보를 수정하고자 하였을 경우
                        Debug.Log("퍼블릭테이블의 타인정보를 수정하고자 하였을 경우");
                        break;
                    case "404": // 존재하지 않는 tableName인 경우
                        Debug.Log("존재하지 않는 tableName인 경우");
                        break;
                    case "412": // 비활성화 된 tableName인 경우 
                        Debug.Log("비활성화 된 tableName인 경우 ");
                        break;
                    case "413": // 하나의 row( column들의 집합 )이 400KB를 넘는 경우
                        Debug.Log("하나의 row( column들의 집합 )이 400KB를 넘는 경우");
                        break;
                    default:
                        break;
                }
                if (sucess != null) sucess();
            });
        });
    }

    [System.Obsolete]
    public void GetPublicUserInfo(string nickname, string table, string key, System.Action callback = null)
    {
        BackendAsyncClass.BackendAsync(Backend.Social.GetGamerIndateByNickname, nickname, (indateByNicknameCallback) => {
            string indate = indateByNicknameCallback.GetReturnValuetoJSON()[0][0]["inDate"][0].ToString();
            Debug.Log("인데이트 값 확인" + indate);
            BackendAsyncClass.BackendAsync(Backend.GameInfo.GetPublicContentsByGamerIndate, table, indate, (getCallback) => {
                serverDataList.Clear();
                switch (getCallback.GetStatusCode())
                {
                    case "200": // 성공 
                        break;
                    case "404": // 존재하지 않는 tableName인 경우 
                        Debug.Log("존재하지 않는 tableName인 경우 ");
                        break;
                    case "400": // private table 아닌 tableName을 입력한 경우 또는 limit이 100이상인 경우
                        Debug.Log("public table 아닌 tableName 을 입력한 경우 또는 limit이 100이상인 경우");
                        break;
                    case "412": // 비활성화 된 tableName인 경우 
                        Debug.Log("비활성화 된 tableName인 경우 ");
                        break;
                    default:
                        break;
                }
                if (getCallback.GetReturnValuetoJSON()[0].Count == 0) // 테이블에 해당 유저의 정보가 아무것도 없는 경우
                {
                    Param param = new Param();
                    param.Add("N", 0);
                    Insert(table, param, () => { GetPublicUserInfo(nickname, table, key, callback); });
                }
                else
                {
                    for (int i = 0; i < getCallback.GetReturnValuetoJSON()[0].Count; i++)
                    {
                        JsonData jsonData = getCallback.GetReturnValuetoJSON()[0][i];
                        if (jsonData.Keys.Contains(key))
                        {
                            if (jsonData[key].Keys.Contains("L")) // 리스트형 데이터인지 아닌지 
                            {
                                JsonData keyData = jsonData[key][0];
                                for (int j = 0; j < keyData.Count; j++)
                                {
                                    serverDataList.Add(keyData[j][0].ToString());
                                }
                            }
                            else
                            {
                                serverDataList.Add(jsonData[key][0].ToString());
                            }
                        }
                    }
                    if (callback != null) { callback(); }
                }
            });
        });
       
    }
}
