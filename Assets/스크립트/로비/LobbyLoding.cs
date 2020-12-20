using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyLoding : MonoBehaviour
{
    public GameObject[] setOffList;
    public GameObject[] setOnList;
    public GameObject lodingObj;
    AsyncOperation operation;

    public void Loading()
    {
        for (int i = 0; i < setOffList.Length; i++)
        {
            setOffList[i].SetActive(false);
        }
        for (int i = 0; i < setOnList.Length; i++)
        {
            setOnList[i].SetActive(true);
        }
    }

    [System.Obsolete]
    public void OnClickLoding()
    {
        for (int i = 0; i < setOnList.Length; i++)
        {
            setOnList[i].SetActive(false);
        }
        lodingObj.SetActive(true);

        BackendGameInfo.instance.GetPrivateContents("UserInfo", "InGameTutorial", () => {
            if (BackendGameInfo.instance.serverDataList[0] == "true")
            {
                StartCoroutine(LoadSceneCoroutine("메인화면"));
            }
            else
            {
                UserInfo.instance.GetUserCharacterInfo().eqipCharacter = CharacterType.한별;
                StartCoroutine(LoadSceneCoroutine("인게임튜토리얼"));
            }
        }, () => {
            UserInfo.instance.GetUserCharacterInfo().eqipCharacter = CharacterType.한별;
            StartCoroutine(LoadSceneCoroutine("인게임튜토리얼"));
        });
    }

    [System.Obsolete]

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return null;
        operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            yield return null;
            if (operation.progress >= 0.9f)
            {
                ChartManager.instance.LoadChart(() => {
                    UserInfo.instance.AllLoadUserInfo(() => {
                        operation.allowSceneActivation = true;
                    });
                });
                break;
            }
        }
    }

}
