using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.U2D;

public class StageManager : MonoBehaviour
{
    StageGroupChartInfo[] stageChartInfos;
    public Transform content;
    public float contextHeightX = 243;
    IEnumerator transformLocalScaleY;
    [Header("리소스")]
    public GameObject stagePripab;
    public Transform stagePripab02Transform;
    public Sprite starOn;
    public Sprite starOff;
    public Sprite conditionSucessSprite;
    public Sprite conditionFailSprite;
    public SpriteAtlas gradeSpriteAtlas;
    public AudioSource playAudio;
    public void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        ContentDelete();
        ContentSet();

        // 스테이지를 깬 후에 그대로 스테이지가 열려있는 상태로 유지 
        Invoke("PreStage02", 0.3f);
    }

    void ContentDelete()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            if (stagePripab02Transform != content.GetChild(i))
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
    }
    void ContentSet()
    {
        stageChartInfos = StageGroupChart.instance.GetStageGroupCharts();
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, contextHeightX * stageChartInfos.Length);
        GameObject pripab;
        for (int i = 0; i < stageChartInfos.Length; i++)
        {
            pripab = Instantiate(stagePripab, Vector3.zero, Quaternion.identity, content);
            pripab.transform.Find("Image").GetComponent<Image>().sprite = stageChartInfos[i].Image;
            pripab.transform.Find("타이틀").GetComponent<Text>().text = TextChart.instance.GetText(stageChartInfos[i].stageName);
            pripab.transform.Find("타이틀").GetComponent<Text>().font = TextChart.instance.GetFont();
            pripab.transform.Find("타이틀").GetComponent<Text>().fontStyle = TextChart.instance.GetFontStyle();
            pripab.transform.Find("설명").GetComponent<Text>().text = TextChart.instance.GetText(stageChartInfos[i].Desc);
            pripab.transform.Find("설명").GetComponent<Text>().font = TextChart.instance.GetFont();
            pripab.transform.Find("설명").GetComponent<Text>().fontStyle = TextChart.instance.GetFontStyle();

           

            int stageGroupInCount = StageChart.instance.StegeGroupInCount(i + 1);
            float maxStarCount = stageGroupInCount * 3;
            float userStarCount = 0;
            List<UserStageInfo> tempUserStageList = UserInfo.instance.UserStageGroupInfo(i+1);
            for (int j = 0; j < tempUserStageList.Count; j++)
            {
                userStarCount += tempUserStageList[j].starCount;
            }
            pripab.transform.Find("fore").GetComponent<Image>().fillAmount = userStarCount / maxStarCount;
            pripab.transform.Find("fore").GetChild(0).GetComponent<Text>().text = (int)(userStarCount / maxStarCount * 100) + "%";

            int siblingIndex = i + 1;
            pripab.GetComponent<Button>().onClick.RemoveAllListeners();

            if (stageChartInfos[i].OpenStageId != 0 && UserInfo.instance.GetUserStageInfo(stageChartInfos[i].OpenStageId) == null)
            {
                pripab.transform.Find("잠금").gameObject.SetActive(true);
            }
            else
            {
                pripab.transform.Find("잠금").gameObject.SetActive(false);

                pripab.GetComponent<Button>().onClick.AddListener(() => {
                    if (Tutorial02Manager.instance.tutorialFlag && Tutorial02Manager.instance.stage01Flag != siblingIndex - 1)
                    {
                        return;
                    }
                    StagePripab02Open(stageGroupInCount, siblingIndex);
                });
            }
        }
        content.GetComponent<VerticalLayoutGroup>().enabled = false;
        content.GetComponent<VerticalLayoutGroup>().enabled = true;
    }

    public void PreStage02()
    {
        if (GameManager.instance.currentStageId != 0)
        {
            int stageID = GameManager.instance.currentStageId;
            GameManager.instance.currentStageId = 0;
            int stageGroupID = StageChart.instance.GetStageChartInfo(stageID).StageGroupId;
            int stageGroupCout = StageChart.instance.StegeGroupInCount(stageGroupID);
            StagePripab02Open(stageGroupCout, stageGroupID,false,()=> {
                Tutorial02Manager.instance.mask01.SetActive(true);
                Tutorial02Manager.instance.mask01.transform.position = Tutorial02Manager.instance.maskTransform[0].position;
            });
     
        }
    }

    void OnClickPlay(int stageId)
    {
        GameManager.instance.StoryGameStart(stageId);
    }

    [HideInInspector] public bool coroutineFlag = false;
    IEnumerator TransformLocalScaleY(Transform transform,float value, bool breakFlag = false, System.Action callback = null)
    {
        if (breakFlag)
        {
            transform.localScale = new Vector2(transform.localScale.x, value);
            content.GetComponent<VerticalLayoutGroup>().enabled = false;
            content.GetComponent<VerticalLayoutGroup>().enabled = true;
            if (callback != null) callback();
            yield break;
        }

        coroutineFlag = true;
        float delta = 0.04f;
        WaitForSeconds wiatTime = new WaitForSeconds(0.01f);
        if (transform.localScale.y < value)
        {
            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y + delta);
        }
        if (transform.localScale.y > value)
        {
            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - delta);
        }
        while (transform.localScale.y <= 1 && transform.localScale.y >= 0)
        {
            if (transform.localScale.y < value)
            {
                transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y + delta);
            }
            else if (transform.localScale.y > value)
            {
                transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y - delta);
            }
            else
            {
                transform.localScale = new Vector2(transform.localScale.x, value);
            }
            content.GetComponent<VerticalLayoutGroup>().enabled = false;
            content.GetComponent<VerticalLayoutGroup>().enabled = true;
            yield return wiatTime;
        }
        transform.localScale = new Vector2(transform.localScale.x, value);
        coroutineFlag = false;

        content.GetComponent<VerticalLayoutGroup>().enabled = false;
        content.GetComponent<VerticalLayoutGroup>().enabled = true;

        if (callback != null) callback();
    }

    public void StagePripab02Open(int stageGroupInCount , int stageGroupID, bool breakFlag = false, System.Action callback = null)
    {
        if (coroutineFlag) return;

        // 스테이지2프리팹의 정보 넣기 
        for (int j = 0; j < stagePripab02Transform.childCount; j++)
        {
            if (j < stageGroupInCount)
            {
                stagePripab02Transform.GetChild(j).gameObject.SetActive(true);
                // 스테이지 번호
                stagePripab02Transform.GetChild(j).Find("스테이지번호").GetComponent<Text>().text = stageGroupID + "-" + (j + 1);
                int stageId = StageChart.instance.GetStageChartInfo(stageGroupID, (j + 1)).StageId;
                // 유저 별 개수 표시
                int userStarCount02 = 0;
                if (UserInfo.instance.GetUserStageInfo(stageId) != null) userStarCount02 = UserInfo.instance.GetUserStageInfo(stageId).starCount;
                for (int k = 0; k < stagePripab02Transform.GetChild(j).Find("별").childCount; k++)
                {
                    if (k < userStarCount02)
                    {
                        stagePripab02Transform.GetChild(j).Find("별").GetChild(k).GetComponent<Image>().sprite = starOn;
                    }
                    else
                    {
                        stagePripab02Transform.GetChild(j).Find("별").GetChild(k).GetComponent<Image>().sprite = starOff;
                    }
                }
                // 잠금 화면 여부
                int prevStageId = StageChart.instance.GetStageChartInfo(stageGroupID, (j + 1)).PrevStageId;
                stagePripab02Transform.GetChild(j).GetComponent<Button>().onClick.RemoveAllListeners();
                stagePripab02Transform.GetChild(j).Find("일반잠금").gameObject.SetActive(false);
                stagePripab02Transform.GetChild(j).Find("특수잠금").gameObject.SetActive(false);
                if (StageChart.instance.GetStageChartInfo(stageGroupID, (j + 1)).ConditionCharacterID == CharacterType.없음) // 일반잠금 
                {
                    stagePripab02Transform.GetChild(j).Find("일반잠금").gameObject.SetActive(true);

                    if (UserInfo.instance.GetUserStageInfo(prevStageId) != null || prevStageId == 0) // 유저가 조건 스테이지를 한번이라도 꺠서 정보가 있다면 
                    {
                        stagePripab02Transform.GetChild(j).Find("일반잠금").gameObject.SetActive(false);
                        stagePripab02Transform.GetChild(j).GetComponent<Button>().onClick.AddListener(() => {
                            PopupManager.instance.StageOpen(stageId); 
                        });
                    }
                }
                else // 특수잠금
                {

                    CharacterType conditionCharacter = StageChart.instance.GetStageChartInfo(stageGroupID, (j + 1)).ConditionCharacterID;
                    GradeType conditionCharacterGrade = StageChart.instance.GetStageChartInfo(stageGroupID, (j + 1)).ConditionCharacterGrade;
                    int conditionStarCount = StageChart.instance.GetStageChartInfo(stageGroupID, (j + 1)).ConditionStarCount;

                    stagePripab02Transform.GetChild(j).Find("특수잠금").gameObject.SetActive(true);

                    //특수잠금 해제
                    if ((int)UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)conditionCharacter] >= (int)conditionCharacterGrade && conditionStarCount <= userStarCount02)
                    {
                        stagePripab02Transform.GetChild(j).Find("특수잠금").gameObject.SetActive(false);
                        stagePripab02Transform.GetChild(j).GetComponent<Button>().onClick.AddListener(() => {
                            PopupManager.instance.StageOpen(stageId);
                        });
                    }
                    else 
                    {
                        // 캐릭터 등급을 갖추었는가
                        if ((int)UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)conditionCharacter] >= (int)conditionCharacterGrade)
                        {
                            stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건1").GetComponent<Image>().sprite = conditionSucessSprite;
                            stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건1").Find("토글체크").gameObject.SetActive(true);
                        }
                        else
                        {
                            stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건1").GetComponent<Image>().sprite = conditionFailSprite;
                            stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건1").Find("토글체크").gameObject.SetActive(false);
                        }
                        // 별 갯수를 갖추었는가 
                        if (conditionStarCount <= userStarCount02)
                        {
                            stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건2").GetComponent<Image>().sprite = conditionSucessSprite;
                            stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건2").Find("토글체크").gameObject.SetActive(true);
                        }
                        else
                        {
                            stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건2").GetComponent<Image>().sprite = conditionFailSprite;
                            stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건2").Find("토글체크").gameObject.SetActive(false);
                        }

                        stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건1").Find("설명").GetComponent<Text>().text = conditionCharacter.ToString();
                        stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건1").Find("등급이미지").GetComponent<Image>().sprite = gradeSpriteAtlas.GetSprite("Grade" + ((int)conditionCharacterGrade-1));
                        stagePripab02Transform.GetChild(j).Find("특수잠금").Find("조건2").Find("설명").GetComponent<Text>().text = "별 " + conditionStarCount + "개";
                    }


                }
            }
            else
            {
                stagePripab02Transform.GetChild(j).gameObject.SetActive(false);
            }
        }

        // 스테이지 프리팹2 위치, 크기조절 
        if (stagePripab02Transform.localScale.y < 0.95f || stagePripab02Transform.GetSiblingIndex() != stageGroupID)
        {
         
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, contextHeightX * stageChartInfos.Length + 900);

            stagePripab02Transform.DOScaleY(0, 0).OnComplete(()=> {
                stagePripab02Transform.SetSiblingIndex(stageGroupID);
                if (transformLocalScaleY != null) StopCoroutine(transformLocalScaleY);
                transformLocalScaleY = TransformLocalScaleY(stagePripab02Transform, 1, breakFlag, callback);
                StartCoroutine(transformLocalScaleY);
            });
        }
        else
        {
            /*
            if (transformLocalScaleY != null) StopCoroutine(transformLocalScaleY);
            transformLocalScaleY = TransformLocalScaleY(stagePripab02Transform, 0);
            StartCoroutine(transformLocalScaleY);*/

            stagePripab02Transform.localScale = new Vector2(stagePripab02Transform.localScale.x, 0);

            content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, contextHeightX * stageChartInfos.Length);

            content.GetComponent<VerticalLayoutGroup>().enabled = false;
            content.GetComponent<VerticalLayoutGroup>().enabled = true;
        }
    }

}
