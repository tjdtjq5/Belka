using BackEnd;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;
    private void Awake() { instance = this; }
    public GameObject blackPannel;
    [SerializeField] GameObject shopPerchase;
    [SerializeField] GameObject result;
    [SerializeField] GameObject pause;
    [SerializeField] GameObject timeOver;
    [SerializeField] GameObject setting;

    [SerializeField] GameObject privacy;
    [SerializeField] GameObject nicknameChange;

    [SerializeField] GameObject bag;
    [SerializeField] GameObject piece;
    [SerializeField] GameObject pieceAssosiation;
    [SerializeField] GameObject audit;
    [SerializeField] GameObject auditEnhance;
    [SerializeField] GameObject characterUpgarde;
    [SerializeField] GameObject rankingHelp;
    [SerializeField] GameObject rankingGuideRecipe;
    [SerializeField] GameObject rankingReward;
    public GameObject dialogue;
    [SerializeField] GameObject rankingInGameResult;
    [SerializeField] GameObject characterUpgradeInGameResult;
    [SerializeField] GameObject rankingScore;
    [SerializeField] GameObject purchaseComplete;
    [SerializeField] GameObject combineComplete;
    [SerializeField] GameObject enhanceSucess;
    [SerializeField] GameObject enhanceFail;
    [SerializeField] GameObject gachaResult;
    [SerializeField] GameObject gachaBoxOpen;
    [SerializeField] GameObject stageOpen;
    [SerializeField] GameObject btnAlram;
    [SerializeField] GameObject alram;
    public GameObject tutorialBlackText;
    public GameObject tutorialBlack;
    public GameObject tutorialSpeech;

    public AudioSource sucessAudioSource;
    public GameObject firework;
    [ContextMenu("aa")]
    public void test()
    {
        tutorialBlack.SetActive(true);
        tutorialBlack.transform.position = result.transform.Find("HomeBtn").position;
    }
    public void ShopPerchase(int shopId ,System.Action callback)
    {
        blackPannel.SetActive(true);
        shopPerchase.SetActive(true);
        shopPerchase.transform.Find("Yes").GetComponent<Button>().onClick.RemoveAllListeners();
        shopPerchase.transform.Find("No").GetComponent<Button>().onClick.RemoveAllListeners();

       
        shopPerchase.transform.Find("No").GetComponent<Button>().onClick.AddListener(() => { shopPerchase.SetActive(false); blackPannel.SetActive(false); });

        shopPerchase.transform.Find("상품이미지").GetComponent<Image>().sprite = ShopChart.instance.GetShopChartInfo(shopId).Image;
        shopPerchase.transform.Find("상품이미지").GetChild(0).GetComponent<Text>().text = ShopChart.instance.GetShopChartInfo(shopId).RewardCount.ToString();

        shopPerchase.transform.Find("상품명").GetComponent<Text>().text = TextChart.instance.GetText(ShopChart.instance.GetShopChartInfo(shopId).Text);
        shopPerchase.transform.Find("상품명").GetComponent<Text>().font = TextChart.instance.GetFont();

        RewardType rewardType = ShopChart.instance.GetShopChartInfo(shopId).RewardType;
        int rewardItem = ShopChart.instance.GetShopChartInfo(shopId).RewardId;
        int rewradCount = ShopChart.instance.GetShopChartInfo(shopId).RewardCount;

        switch (ShopChart.instance.GetShopChartInfo(shopId).BuyType)
        {
            case BuyType.dia:
                shopPerchase.transform.Find("다이아").gameObject.SetActive(true);
                shopPerchase.transform.Find("가격").gameObject.SetActive(false);
                shopPerchase.transform.Find("다이아").GetChild(0).GetComponent<Text>().text = TextChart.instance.GetText(ShopChart.instance.GetShopChartInfo(shopId).BuyText);

                shopPerchase.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(() => {
                    // 보상 및 구매 
                    int needDia = ShopChart.instance.GetShopChartInfo(shopId).BuyCount;

                    if (UserInfo.instance.GetUserCrystal() >= needDia)
                    {
                        SoundManager.instance.SfxPlay(sucessAudioSource);
                        callback(); shopPerchase.SetActive(false); blackPannel.SetActive(false);

                        UserInfo.instance.PullUserCrystal(needDia);
                        UpperInfo.instance.CrystalSet();
                        UserInfo.instance.SaveUserCrystal(() => {
                            GameManager.instance.UserItemReward(rewardType, rewardItem, rewradCount);
                        });
                        PurchaseComplete(rewardItem, () => { });
                    }
                    else
                    {
                        Alram("Error_02");
                    }
                });
                break;
            case BuyType.Cash:
                shopPerchase.transform.Find("다이아").gameObject.SetActive(false);
                shopPerchase.transform.Find("가격").gameObject.SetActive(true);
                shopPerchase.transform.Find("가격").GetComponent<Text>().text = TextChart.instance.GetText(ShopChart.instance.GetShopChartInfo(shopId).BuyText);
                shopPerchase.transform.Find("가격").GetComponent<Text>().font = TextChart.instance.GetFont();

                shopPerchase.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(() => {
                    SoundManager.instance.SfxPlay(sucessAudioSource);
                    callback(); shopPerchase.SetActive(false); blackPannel.SetActive(false);
                    // 보상 및 구매 
                });
                break;
            case BuyType.Ads:
                shopPerchase.transform.Find("다이아").gameObject.SetActive(false);
                shopPerchase.transform.Find("가격").gameObject.SetActive(true);
                shopPerchase.transform.Find("가격").GetComponent<Text>().text = TextChart.instance.GetText(ShopChart.instance.GetShopChartInfo(shopId).BuyText);
                shopPerchase.transform.Find("가격").GetComponent<Text>().font = TextChart.instance.GetFont();

                shopPerchase.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(() => {
                    SoundManager.instance.SfxPlay(sucessAudioSource);
                    callback(); shopPerchase.SetActive(false); blackPannel.SetActive(false);
                    // 보상 및 구매 
                    GoogleAdsManager.instance.SetRewardItem(rewardType, rewardItem, rewradCount);
                    GoogleAdsManager.instance.UserOptToWatchAd();

                    PurchaseComplete(rewardItem, () => { });
                });
                break;
        }
    }

    [System.Obsolete]
    public void StoryInGameResult(float scoreTime , int stageId)
    {
        UserStageInfo userStageInfo = UserInfo.instance.GetUserStageInfo(stageId);
        float bestScoreTime = scoreTime;
        int userStarCount = 0;
        bool isFirst = true;
        if (userStageInfo != null)
        {
            isFirst = false;
            userStarCount = userStageInfo.starCount;
            if (userStageInfo.clearTime > scoreTime) bestScoreTime = userStageInfo.clearTime;
        }

        // Time
        int min = (int)scoreTime / 60;
        int second = (int)scoreTime % 60;
        result.transform.Find("Flag").Find("Time").GetChild(0).GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}", min, second);

        // Condition
        string[] starCondition = StageChart.instance.GetStageChartInfo(stageId).StarCondition;
        string clearMsg = "";
        int starCount = 0;
        for (int i = 0; i < starCondition.Length; i++)
        {
            if (starCondition[i] == "clear")
            {
                clearMsg = "JUST CLEAR";
                starCount++;
            }
            else
            {
                clearMsg = "CLEAR IN ";
                int conditionTime = int.Parse(starCondition[i]);
                int conditionMin = conditionTime / 60;
                int conditionSecond = conditionTime % 60;
                clearMsg += string.Format("{0:D2}:{1:D2}", conditionMin, conditionSecond);

                if(scoreTime > conditionTime) starCount++;
            }
            result.transform.Find("Flag").Find("Mission").GetChild(i).Find("Text").GetComponent<Text>().text = clearMsg;
        }

        // 별 
        for (int i = 0; i < result.transform.Find("Star").childCount; i++)
        {
            result.transform.Find("Star").GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
        for (int i = 0; i < starCount; i++)
        {
            result.transform.Find("Star").GetChild(i).GetChild(0).gameObject.SetActive(true);
        }

        for (int i = 0; i < result.transform.Find("Reward").childCount; i++)
        {
            result.transform.Find("Reward").GetChild(i).gameObject.SetActive(false);
        }

        RewardType[] rewardTypes = StageChart.instance.GetStageChartInfo(stageId).StarRewardType;
        int[] rewardIds = StageChart.instance.GetStageChartInfo(stageId).RewardId;
        int[] rewardCount = StageChart.instance.GetStageChartInfo(stageId).RewardCount;

        int len = starCount;
        Debug.Log(len);
        if (!isFirst)
        {
            len = 0;
        }
        for (int i = 0; i < len; i++)
        {
            result.transform.Find("Reward").GetChild(i).gameObject.SetActive(true);
            switch (rewardTypes[i])
            {
                case RewardType.Goods:
                    result.transform.Find("Reward").GetChild(i).Find("Image").GetComponent<Image>().sprite = GoodsChart.instance.GetGoodsChartInfo(rewardIds[i]).Image;
                    switch (rewardIds[i])
                    {
                        case 1: // 하트
                            UserInfo.instance.PutUserHeart(rewardCount[i]);
                            break;
                        case 2: // 다이아
                            UserInfo.instance.PutUserCrystal(rewardCount[i]);
                            break;
                    }
                    break;
                case RewardType.Item:
                    result.transform.Find("Reward").GetChild(i).Find("Image").GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(rewardIds[i]).Image;
                    UserInfo.instance.PutUserItem(rewardIds[i], rewardCount[i]);
                    break;
            }
            result.transform.Find("Reward").GetChild(i).Find("Image").GetComponent<Image>().SetNativeSize();
            result.transform.Find("Reward").GetChild(i).Find("Text").GetComponent<Text>().text = rewardCount[i].ToString();
        }

        // clearReward
        RewardType[] clearRewardType = StageChart.instance.GetStageChartInfo(stageId).ClearRewardType;
        int[] clearRewardId = StageChart.instance.GetStageChartInfo(stageId).ClearRewardId;
        int[] clearRewardCount = StageChart.instance.GetStageChartInfo(stageId).ClearRewardCount;

        int len02 = len + clearRewardType.Length;
        if (len02 > result.transform.Find("Reward").childCount) len02 = result.transform.Find("Reward").childCount;
        for (int i = len; i < len02; i++)
        {
            result.transform.Find("Reward").GetChild(i).gameObject.SetActive(true);
            switch (clearRewardType[i - len])
            {
                case RewardType.Goods:
                    result.transform.Find("Reward").GetChild(i).Find("Image").GetComponent<Image>().sprite = GoodsChart.instance.GetGoodsChartInfo(clearRewardId[i - len]).Image;
                    switch (clearRewardId[i - len])
                    {
                        case 1: // 하트
                            UserInfo.instance.PutUserHeart(clearRewardCount[i - len]);
                            break;
                        case 2: // 다이아
                            UserInfo.instance.PutUserCrystal(clearRewardCount[i - len]);
                            break;
                    }
                    break;
                case RewardType.Item:
                    result.transform.Find("Reward").GetChild(i).Find("Image").GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(clearRewardId[i - len]).Image;
                    UserInfo.instance.PutUserItem(clearRewardId[i - len], clearRewardCount[i - len]);
                    break;
            }
            result.transform.Find("Reward").GetChild(i).Find("Image").GetComponent<Image>().SetNativeSize();
            result.transform.Find("Reward").GetChild(i).Find("Text").GetComponent<Text>().text = clearRewardCount[i - len].ToString();
        }

        //Button
        result.transform.Find("HomeBtn").GetComponent<Button>().onClick.RemoveAllListeners();
        result.transform.Find("HomeBtn").GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("메인화면"); });
        result.transform.Find("ReplayBtn").GetComponent<Button>().onClick.RemoveAllListeners();
        result.transform.Find("ReplayBtn").GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("인게임"); });

        //UserStageInfo
        if (userStageInfo != null)
        {
            UserInfo.instance.GetUserStageInfo(stageId).clearTime = bestScoreTime;
            UserInfo.instance.GetUserStageInfo(stageId).starCount = starCount;
        }
        else
        {
            UserInfo.instance.PutUserStage(stageId, bestScoreTime, starCount);
        }

        //Save
        UserInfo.instance.SaveUserCrystal(() => {
            UserInfo.instance.SaveUserHeartInfo(() => {
                UserInfo.instance.SaveUserItemInfo(() => {
                    UserInfo.instance.SaveUserStageInfo(() => {
                        if (stageId == 1)
                        {
                            BackendGameInfo.instance.GetPrivateContents("UserInfo", "Tutorial02", () => {
                            },()=> {
                                GradeType gradeType = UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)CharacterType.한별];
                                if (gradeType == GradeType.E)
                                {
                                    // 튜토리얼 시작 
                                    result.transform.Find("ReplayBtn").GetComponent<Button>().onClick.RemoveAllListeners();
                                    Param param = new Param();
                                    param.Add("Tutorial02", true);
                                    BackendGameInfo.instance.PrivateTableUpdate("UserInfo", param);
                                    TutorialBlackText("Tutorial_17");
                                    tutorialBlack.SetActive(true);
                                    tutorialBlack.transform.position = result.transform.Find("HomeBtn").position;
                                }
                            });
                        }
                        blackPannel.SetActive(true);
                        result.SetActive(true);
                        SoundManager.instance.SfxPlay(result.GetComponent<AudioSource>());
                    });
                });
            });
        });
       
    }
    public void Pause()
    {
        blackPannel.SetActive(true);
        pause.SetActive(true);

        pause.transform.Find("Bg").Find("Continue").GetComponent<Button>().onClick.RemoveAllListeners();
        pause.transform.Find("Bg").Find("Continue").GetComponent<Button>().onClick.AddListener(()=> { blackPannel.SetActive(false); pause.SetActive(false); });
        pause.transform.Find("Bg").Find("Restart").GetComponent<Button>().onClick.RemoveAllListeners();
        pause.transform.Find("Bg").Find("Restart").GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("인게임"); });
        pause.transform.Find("Bg").Find("Home").GetComponent<Button>().onClick.RemoveAllListeners();
        pause.transform.Find("Bg").Find("Home").GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("메인화면"); });
    }
    public void TimeOver()
    {
        blackPannel.SetActive(true);
        timeOver.SetActive(true);

        timeOver.transform.Find("Restart").GetComponent<Button>().onClick.RemoveAllListeners();
        timeOver.transform.Find("Restart").GetComponent<Button>().onClick.AddListener(()=> { SceneManager.LoadScene("인게임"); });
        timeOver.transform.Find("Home").GetComponent<Button>().onClick.RemoveAllListeners();
        timeOver.transform.Find("Home").GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("메인화면"); });
    }

    public void Setting()
    {
        if (Tutorial02Manager.instance.tutorialFlag)
        {
            return;
        }

        blackPannel.SetActive(true);
        setting.SetActive(true);

        setting.transform.Find("bg").GetChild(0).GetComponent<Text>().text = TextChart.instance.GetText("Setting_Title");
        setting.transform.Find("Sound").GetChild(1).GetComponent<Text>().text = TextChart.instance.GetText("Setting_Sound");
        setting.transform.Find("Music").GetChild(1).GetComponent<Text>().text = TextChart.instance.GetText("Setting_Music");
        setting.transform.Find("Vibration").GetChild(1).GetComponent<Text>().text = TextChart.instance.GetText("Setting_Vibration");
        setting.transform.Find("Language").GetChild(1).GetComponent<Text>().text = TextChart.instance.GetText("Setting_Language");

        setting.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        setting.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            blackPannel.SetActive(false);
            setting.SetActive(false);
        });
    }

    public void Privacy()
    {
        privacy.SetActive(true);
        privacy.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        privacy.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            privacy.SetActive(false);
        });
    }
    public void NicknameChange()
    {
        nicknameChange.SetActive(true);
        nicknameChange.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        nicknameChange.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            nicknameChange.SetActive(false);
        });
    }


    public void Bag()
    {
        if (Tutorial02Manager.instance.tutorialFlag)
        {
            return;
        }

        blackPannel.SetActive(true);
        bag.SetActive(true);
        bag.GetComponent<Bag>().Open();

        bag.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        bag.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            blackPannel.SetActive(false);
            bag.SetActive(false);
        });
    }
    public void Piece(int itemId)
    {
        blackPannel.SetActive(true);
        piece.SetActive(true);
        piece.GetComponent<Piece>().Open(itemId);

        piece.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        piece.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            piece.SetActive(false);
        });
    }
    public void PieceAssosiation(int itemId)
    {
        int beforeItemCount = UserInfo.instance.GetUserItemInfo(itemId).numberOfItem;
        int count = CombinationChart.instance.GetCombinationChartInfo(itemId).Count;
        if (beforeItemCount / count <= 0)
        {
            return;
        }

        blackPannel.SetActive(true);
        pieceAssosiation.SetActive(true);
        pieceAssosiation.GetComponent<PieceAssosiation>().Open(itemId);

        pieceAssosiation.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        pieceAssosiation.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            pieceAssosiation.SetActive(false);
        });
        pieceAssosiation.transform.Find("취소").GetComponent<Button>().onClick.RemoveAllListeners();
        pieceAssosiation.transform.Find("취소").GetComponent<Button>().onClick.AddListener(() => {
            pieceAssosiation.SetActive(false);
        }); 
    }
    public void Audit(int itemId)
    {
        blackPannel.SetActive(true);
        audit.SetActive(true);
        audit.GetComponent<Audit>().Open(itemId);

        audit.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        audit.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            audit.SetActive(false);
        });
    }

    public void AuditEnhance(int itemId)
    {
        blackPannel.SetActive(true);
        auditEnhance.SetActive(true);
        auditEnhance.GetComponent<AuditEnhance>().Open(itemId);

        auditEnhance.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        auditEnhance.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            auditEnhance.SetActive(false);
        });
        auditEnhance.transform.Find("취소").GetComponent<Button>().onClick.RemoveAllListeners();
        auditEnhance.transform.Find("취소").GetComponent<Button>().onClick.AddListener(() => {
            auditEnhance.SetActive(false);
        });
    }
    public void CharacterUpgrade(CharacterType characterType, GradeType gradeType)
    {
        blackPannel.SetActive(true);
        characterUpgarde.SetActive(true);
        characterUpgarde.GetComponent<CharacterUpgarde>().Open(characterType, gradeType);

        characterUpgarde.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        characterUpgarde.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            blackPannel.SetActive(false);
            characterUpgarde.SetActive(false);
        });
    }

    public void RankingHelp()
    {
        blackPannel.SetActive(true);
        rankingHelp.SetActive(true);

        rankingHelp.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        rankingHelp.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            blackPannel.SetActive(false);
            rankingHelp.SetActive(false);
        });
    }

    public void RankingGuideRecipe()
    {
        blackPannel.SetActive(true);
        rankingGuideRecipe.SetActive(true);

        rankingGuideRecipe.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        rankingGuideRecipe.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            blackPannel.SetActive(false);
            rankingGuideRecipe.SetActive(false);
        });
        BackendReturnObject bro = Backend.Utils.GetServerTime();
        string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime currentTime = DateTime.Parse(time);
        int rankingId = RankingManager.instance.GetRankingID(currentTime.Year, currentTime.Month, currentTime.Day);
        int recipeID = RankingChart.instance.GetRankingChartChartInfo(rankingId).RecipeID;

        Transform recipeTransform = rankingGuideRecipe.transform.Find("레시피");

        int toolCount = 0;
        List<int> finishFoodList = new List<int>();
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).FirstToolId != 0) toolCount++;
        int[] firstMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).FirstMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).FirstFinishFoodId);
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).SecondToolId != 0) toolCount++;
        int[] secondMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).SecondMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).SecondFinishFoodId);
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).ThirdToolId != 0) toolCount++;
        int[] thirdMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).ThirdMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).ThirdFinishFoodId);
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).FourthToolId != 0) toolCount++;
        int[] fourthMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).FourthMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).FourthFinishFoodId);
        float height = 110;

        recipeTransform.Find("Square").GetComponent<RectTransform>().sizeDelta = new Vector2(recipeTransform.Find("Square").GetComponent<RectTransform>().sizeDelta.x, height * toolCount);
        // 해당 레시피의 도구 수만큼 레시피 조리방법 활성화 
        for (int i = 0; i < recipeTransform.Find("Frame").childCount; i++)
        {
            if (i < toolCount)
            {
                recipeTransform.Find("Frame").GetChild(i).gameObject.SetActive(true);
                Transform frameTransform = recipeTransform.Find("Frame").GetChild(i);
                frameTransform.GetChild(0).GetComponent<Image>().sprite = FoodChart.instance.GetFoodChartInfo(finishFoodList[i]).Image;
                for (int j = 1; j < frameTransform.childCount; j++)
                {
                    int[] materialID = new int[0];
                    switch (i)
                    {
                        case 0:
                            materialID = firstMaterialID;
                            break;
                        case 1:
                            materialID = secondMaterialID;
                            break;
                        case 2:
                            materialID = thirdMaterialID;
                            break;
                        case 3:
                            materialID = fourthMaterialID;
                            break;
                    }
                    if (j < materialID.Length + 1)
                    {
                        frameTransform.GetChild(j).GetComponent<Image>().sprite = FoodChart.instance.GetFoodChartInfo(materialID[j - 1]).Image;
                        frameTransform.GetChild(j).GetComponent<Image>().SetNativeSize();
                    }
                    else
                    {
                        frameTransform.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                recipeTransform.Find("Frame").GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    [Header("랭킹보상")]
    public Sprite firstRankingSprite;
    public Sprite secondRankingSprite;
    public Sprite thirdRankingSprite;
    public Sprite heartSprite;
    public Sprite diaSprite;

    public void RankingReward()
    {
        blackPannel.SetActive(true);
        rankingReward.SetActive(true);

        rankingReward.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        rankingReward.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            blackPannel.SetActive(false);
            rankingReward.SetActive(false);
        });

        Transform prepabTransform = rankingReward.transform.Find("프리팹");
        GameObject originPrepab = Resources.Load<GameObject>("프리팹/랭킹보상프리팹") as GameObject;
        int len = RankingRewardChart.instance.rankingRewardChartInfos.Length;

        for (int i = 0; i < prepabTransform.childCount; i++)
        {
            Destroy(prepabTransform.GetChild(i).gameObject);
        }

        for (int i = 0; i < len; i++)
        {
            GameObject prepab = Instantiate(originPrepab, Vector3.zero, Quaternion.identity, prepabTransform);
            RankingRewardChartInfo rankingRewardChartInfo = RankingRewardChart.instance.rankingRewardChartInfos[i];
            int[] ranking = rankingRewardChartInfo.Ranking;
            RewardType[] rewardTypes = rankingRewardChartInfo.RewardType;
            int[] rewardID = rankingRewardChartInfo.RewardId;
            int[] rewardCount = rankingRewardChartInfo.RewardCount;

            prepab.transform.Find("Left").Find("순위이미지").gameObject.SetActive(false);
            prepab.transform.Find("Left").Find("순위텍스트").gameObject.SetActive(false);
            if (ranking.Length == 1)
            {
                switch (ranking[0])
                {
                    case 1:
                        prepab.transform.Find("Left").Find("순위이미지").gameObject.SetActive(true);
                        prepab.transform.Find("Left").Find("순위이미지").GetComponent<Image>().sprite = firstRankingSprite;
                        break;
                    case 2:
                        prepab.transform.Find("Left").Find("순위이미지").gameObject.SetActive(true);
                        prepab.transform.Find("Left").Find("순위이미지").GetComponent<Image>().sprite = secondRankingSprite;
                        break;
                    case 3:
                        prepab.transform.Find("Left").Find("순위이미지").gameObject.SetActive(true);
                        prepab.transform.Find("Left").Find("순위이미지").GetComponent<Image>().sprite = thirdRankingSprite;
                        break;
                    default:
                        prepab.transform.Find("Left").Find("순위텍스트").gameObject.SetActive(true);
                        prepab.transform.Find("Left").Find("순위텍스트").GetComponent<Text>().text = ranking[0] + "위";
                        break;
                }
            }
            if(ranking.Length == 2)
            {
                prepab.transform.Find("Left").Find("순위텍스트").gameObject.SetActive(true);
                prepab.transform.Find("Left").Find("순위텍스트").GetComponent<Text>().text = ranking[0] + "위~" + ranking[1] + "위";
            }

            int count = rewardTypes.Length;
            if (count > prepab.transform.Find("Right").childCount) count = prepab.transform.Find("Right").childCount;

            for (int j = 0; j < prepab.transform.Find("Right").childCount; j++)
            {
                prepab.transform.Find("Right").GetChild(j).gameObject.SetActive(false);
            }

            for (int j = 0; j < count; j++)
            {
                prepab.transform.Find("Right").GetChild(j).gameObject.SetActive(true);

                Sprite tempItemSprite = null;
                int tempItemCount = rewardCount[j];
                switch (rewardTypes[j])
                {
                    case RewardType.Goods:
                        switch (rewardID[j])
                        {
                            case 1: // 하트
                                tempItemSprite = heartSprite;
                                break;
                            case 2: // 다이아
                                tempItemSprite = diaSprite;
                                break;
                        }
                        break;
                    case RewardType.Item:
                        tempItemSprite = ItemChart.instance.GetItemChartChartInfo(rewardID[j]).Image;
                        break;
                }
                prepab.transform.Find("Right").GetChild(j).GetComponent<Image>().sprite = tempItemSprite;
                prepab.transform.Find("Right").GetChild(j).GetComponent<Image>().SetNativeSize();
                prepab.transform.Find("Right").GetChild(j).GetChild(0).GetComponent<Text>().text = tempItemCount.ToString();
            }
        }
    }
    public void Dialogue(int dialogueID , System.Action callback)
    {
        dialogue.SetActive(true);
    //    Debug.Log(dialogueID);
        dialogue.GetComponent<DialogueManager>().Open(dialogueID, ()=> {  callback(); });
    }

    [Obsolete]
    public void RankingInGameResult(float scoreTime)
    {
        rankingInGameResult.transform.Find("Home").GetComponent<Button>().onClick.RemoveAllListeners();
        rankingInGameResult.transform.Find("Home").GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("메인화면"); });

        rankingInGameResult.transform.Find("ReStart").GetComponent<Button>().onClick.RemoveAllListeners();
        rankingInGameResult.transform.Find("ReStart").GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("인게임"); });

        int min = (int)scoreTime / 60;
        int second = (int)scoreTime % 60;
        rankingInGameResult.transform.Find("SCORE").GetChild(0).GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}", min, second);

        BackendReturnObject bro = Backend.Utils.GetServerTime();
        string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
        DateTime currentTime = DateTime.Parse(time);
        int rankingId = RankingManager.instance.GetRankingID(currentTime.Year, currentTime.Month, currentTime.Day);
        string key = "RankingInGameBestScore" + rankingId;
        BackendGameInfo.instance.GetPrivateContents("UserInfo", key, () => {
            float bestScore = float.Parse(BackendGameInfo.instance.serverDataList[0]);
            Debug.Log("내 개인 최고점수 가져오기 성공 : " + bestScore);

            min = (int)bestScore / 60;
            second = (int)bestScore % 60;
            rankingInGameResult.transform.Find("BestScore").GetChild(0).GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}", min, second);

            RankingManager.instance.SetScore(scoreTime, () => {

                Debug.Log("셋 스코어 : " + scoreTime);

                RankingManager.instance.GetMyRankingToday(() => {
                    if (RankingManager.instance.rankingUserInfos.Count > 0)
                    {

                        int rank = RankingManager.instance.rankingUserInfos[0].rank;
                        Debug.Log("내 랭킹 가져오기 성공 : " + rank);
                        switch (rank)
                        {
                            case 1:
                                rankingInGameResult.transform.Find("Ranking").GetChild(0).GetComponent<Text>().text = rank + "st";
                                break;
                            case 2:
                                rankingInGameResult.transform.Find("Ranking").GetChild(0).GetComponent<Text>().text = rank + "nd";
                                break;
                            case 3:
                                rankingInGameResult.transform.Find("Ranking").GetChild(0).GetComponent<Text>().text = rank + "rd";
                                break;
                            default:
                                rankingInGameResult.transform.Find("Ranking").GetChild(0).GetComponent<Text>().text = rank + "TH";
                                break;
                        }

                        blackPannel.SetActive(true);
                        rankingInGameResult.SetActive(true);
                        SoundManager.instance.SfxPlay(rankingInGameResult.GetComponent<AudioSource>());
                    }
                    else
                    {
                        rankingInGameResult.transform.Find("Ranking").GetChild(0).GetComponent<Text>().text = "--TH";

                        blackPannel.SetActive(true);
                        rankingInGameResult.SetActive(true);
                        SoundManager.instance.SfxPlay(rankingInGameResult.GetComponent<AudioSource>());
                    }
                });
            });
        }, () => {
            rankingInGameResult.transform.Find("BestScore").GetChild(0).GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}", min, second);

            Debug.Log("내 개인 최고점수 가져오기 실패 : ");

            RankingManager.instance.SetScore(scoreTime, () => {

                Debug.Log("셋 스코어 : " + scoreTime);

                RankingManager.instance.GetMyRankingToday(() => {
                if (RankingManager.instance.rankingUserInfos.Count > 0)
                {
                    int rank = RankingManager.instance.rankingUserInfos[0].rank;
                    Debug.Log("내 랭킹 가져오기 성공 : " + rank);
                    rankingInGameResult.transform.Find("Ranking").GetChild(0).GetComponent<Text>().text = rank + "TH";

                    blackPannel.SetActive(true);
                    rankingInGameResult.SetActive(true);
                    SoundManager.instance.SfxPlay(rankingInGameResult.GetComponent<AudioSource>());
                }
                else
                {
                    rankingInGameResult.transform.Find("Ranking").GetChild(0).GetComponent<Text>().text = "--TH";

                    blackPannel.SetActive(true);
                    rankingInGameResult.SetActive(true);
                    SoundManager.instance.SfxPlay(rankingInGameResult.GetComponent<AudioSource>());

                }
                }); });
        });

    }
    public void CharacterUpgradeInGameResult()
    {
        blackPannel.SetActive(true);
        characterUpgradeInGameResult.SetActive(true);

        SoundManager.instance.SfxPlay(characterUpgradeInGameResult.GetComponent<AudioSource>());
        characterUpgradeInGameResult.transform.Find("Home").GetComponent<Button>().onClick.RemoveAllListeners();

        CharacterType characterType = UserInfo.instance.GetUserCharacterInfo().eqipCharacter;
        CharacterUpgradeChartInfo characterUpgradeChartInfo = CharacterUpgradeChart.instance.GetCharacterUpgradeChartInfo(characterType, UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)characterType]);
        int myNum = 0;
        myNum = UserInfo.instance.GetUserItemInfo(characterUpgradeChartInfo.UpgradeItemId).numberOfItem;
        int needNum = characterUpgradeChartInfo.UpgradeItemCount;
        UserInfo.instance.SetUserItemInfo(characterUpgradeChartInfo.UpgradeItemId, myNum - needNum);
        UserInfo.instance.SaveUserItemInfo(() => {
            UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)characterType] = (GradeType)((int)UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)characterType] + 1);
            UserInfo.instance.SaveUserCharacterInfo(() => {
                characterUpgradeInGameResult.transform.Find("Home").GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("메인화면"); });
            });
        });
    }

    public void RankingScore()
    {
        blackPannel.SetActive(true);
        rankingScore.SetActive(true);

        rankingScore.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        rankingScore.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            blackPannel.SetActive(false);
            rankingScore.SetActive(false);
        });

        rankingScore.GetComponent<RankingScore>().Open();
    }
    public void PurchaseComplete(int itemId, System.Action callback)
    {
        if (ItemChart.instance.GetItemChartChartInfo(itemId) == null)
        {
            Debug.Log("해당 아이템 아이디가 테이블에 없습니다. 아이템 아이디 : " + itemId);
            return;
        }

        blackPannel.SetActive(true);
        purchaseComplete.SetActive(true);

        GameObject particle = Instantiate(firework, purchaseComplete.transform.Find("구매완료이미지").position, Quaternion.identity);
        particle.GetComponent<ParticleSystem>().Play();

        purchaseComplete.transform.Find("아이템이미지").GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(itemId).Image;
        purchaseComplete.transform.Find("아이템이름").GetComponent<Text>().text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(itemId).Text);
        purchaseComplete.transform.Find("아이템이름").GetComponent<Text>().font = TextChart.instance.GetFont();

        purchaseComplete.transform.Find("확인버튼").GetComponent<Button>().onClick.RemoveAllListeners();
        purchaseComplete.transform.Find("확인버튼").GetComponent<Button>().onClick.AddListener(() => {
            blackPannel.SetActive(false);
            purchaseComplete.SetActive(false);
            callback();
        });
    }
    public void CombineComplete(int itemId, System.Action callback)
    {
        if (ItemChart.instance.GetItemChartChartInfo(itemId) == null)
        {
            Debug.Log("해당 아이템 아이디가 테이블에 없습니다. 아이템 아이디 : " + itemId);
            return;
        }

        blackPannel.SetActive(true);
        combineComplete.SetActive(true);

        GameObject particle = Instantiate(firework, combineComplete.transform.Find("조합성공이미지").position, Quaternion.identity);
        particle.GetComponent<ParticleSystem>().Play();

        combineComplete.transform.Find("아이템이미지").GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(itemId).Image;
        combineComplete.transform.Find("아이템이름").GetComponent<Text>().text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(itemId).Text);
        combineComplete.transform.Find("아이템이름").GetComponent<Text>().font = TextChart.instance.GetFont();

        combineComplete.transform.Find("확인버튼").GetComponent<Button>().onClick.RemoveAllListeners();
        combineComplete.transform.Find("확인버튼").GetComponent<Button>().onClick.AddListener(() => {
            combineComplete.SetActive(false);
            callback();
        });
    }
    public void EnhanceSucess(int itemId, System.Action callback)
    {
        if (ItemChart.instance.GetItemChartChartInfo(itemId) == null)
        {
            Debug.Log("해당 아이템 아이디가 테이블에 없습니다. 아이템 아이디 : " + itemId);
            return;
        }

        blackPannel.SetActive(true);
        enhanceSucess.SetActive(true);
        GameObject particle = Instantiate(firework, enhanceSucess.transform.Find("강화성공이미지").position, Quaternion.identity);
        particle.GetComponent<ParticleSystem>().Play();

        enhanceSucess.transform.Find("아이템이미지").GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(itemId).Image;
        enhanceSucess.transform.Find("아이템이름").GetComponent<Text>().text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(itemId).Text);
        enhanceSucess.transform.Find("아이템이름").GetComponent<Text>().font = TextChart.instance.GetFont();

        enhanceSucess.transform.Find("확인버튼").GetComponent<Button>().onClick.RemoveAllListeners();
        enhanceSucess.transform.Find("확인버튼").GetComponent<Button>().onClick.AddListener(() => {
            enhanceSucess.SetActive(false);
            Bag();
            callback();
        });
    }
    public void EnhanceFail(int itemId, System.Action callback)
    {
        if (ItemChart.instance.GetItemChartChartInfo(itemId) == null)
        {
            Debug.Log("해당 아이템 아이디가 테이블에 없습니다. 아이템 아이디 : " + itemId);
            return;
        }

        blackPannel.SetActive(true);
        enhanceFail.SetActive(true);

        enhanceFail.transform.Find("아이템이미지").GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(itemId).Image;
        enhanceFail.transform.Find("아이템이름").GetComponent<Text>().text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(itemId).Text);
        enhanceFail.transform.Find("아이템이름").GetComponent<Text>().font = TextChart.instance.GetFont();

        enhanceFail.transform.Find("확인버튼").GetComponent<Button>().onClick.RemoveAllListeners();
        enhanceFail.transform.Find("확인버튼").GetComponent<Button>().onClick.AddListener(() => {
            enhanceFail.SetActive(false);
            Bag();
            callback();
        });
    }
    public void GachaResult(int len, int[] itemId, int[] itemCount, System.Action callback)
    {
        blackPannel.SetActive(true);
        gachaResult.SetActive(true);

        GameObject particle = Instantiate(firework, gachaResult.transform.Find("가챠결과이미지").position, Quaternion.identity);
        particle.GetComponent<ParticleSystem>().Play();

        Transform content = gachaResult.transform.Find("Scroll View").GetChild(0).GetChild(0);
        for (int i = 0; i < content.childCount; i++)
        {
            content.GetChild(i).gameObject.SetActive(false);
        }

        float origin = 1037;
        float mx = 301.8f;
        if (len > 100) len = 100;
        content.GetComponent<RectTransform>().offsetMax = new Vector2(-(origin - (len * mx)), content.GetComponent<RectTransform>().offsetMax.y);
        for (int i = 0; i < len; i++)
        {
            content.GetChild(i).gameObject.SetActive(true);
            content.GetChild(i).GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(itemId[i]).Image;
            content.GetChild(i).GetChild(0).GetComponent<Text>().text = TextChart.instance.GetText(ItemChart.instance.GetItemChartChartInfo(itemId[i]).Text);
            content.GetChild(i).GetChild(0).GetComponent<Text>().font = TextChart.instance.GetFont();
            content.GetChild(i).GetChild(1).GetComponent<Text>().text = itemCount[i].ToString();
        }

        gachaResult.transform.Find("확인버튼").GetComponent<Button>().onClick.RemoveAllListeners();
        gachaResult.transform.Find("확인버튼").GetComponent<Button>().onClick.AddListener(() => {
            blackPannel.SetActive(false);
            gachaResult.SetActive(false);
            callback();
        });
    }
    public void GachaBoxOpen(int itemId)
    {
        blackPannel.SetActive(true);
        gachaBoxOpen.SetActive(true);
        bag.SetActive(false);

        gachaBoxOpen.GetComponent<GachaBoxOpen>().Open(itemId);

        gachaBoxOpen.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        gachaBoxOpen.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            gachaBoxOpen.SetActive(false);
            blackPannel.SetActive(false);
        });
    }
 
    public void StageOpen(int stageId)
    {
        blackPannel.SetActive(true);
        stageOpen.SetActive(true);

        stageOpen.transform.Find("Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        stageOpen.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() => {
            stageOpen.SetActive(false);
            blackPannel.SetActive(false);
        });

        //오늘의 요리
        Transform transform = stageOpen.transform;
        int recipeID = StageChart.instance.GetStageChartInfo(stageId).RecipeId;
        int numberFood = StageChart.instance.GetStageChartInfo(stageId).NumberOfFood;
        int lastFood = RecipeChart.instance.GetRecipeChartInfo(recipeID).LastFood;
        transform.Find("오늘의 요리").Find("Food").GetComponent<Image>().sprite = FoodChart.instance.GetFoodChartInfo(lastFood).Image;
        transform.Find("오늘의 요리").Find("Food").GetComponent<Image>().SetNativeSize();
        transform.Find("오늘의 요리").Find("Number").GetComponent<Text>().text = "x" + numberFood;

        transform.Find("popupRibbon").GetChild(0).GetComponent<Text>().text = "STAGE " + StageChart.instance.GetStageChartInfo(stageId).StageGroupId + "-" + StageChart.instance.GetStageChartInfo(stageId).StageGroupInId;


        // 레시피
        int toolCount = 0;
        List<int> finishFoodList = new List<int>();
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).FirstToolId != 0) toolCount++;
        int[] firstMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).FirstMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).FirstFinishFoodId);
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).SecondToolId != 0) toolCount++;
        int[] secondMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).SecondMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).SecondFinishFoodId);
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).ThirdToolId != 0) toolCount++;
        int[] thirdMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).ThirdMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).ThirdFinishFoodId);
        if (RecipeChart.instance.GetRecipeChartInfo(recipeID).FourthToolId != 0) toolCount++;
        int[] fourthMaterialID = RecipeChart.instance.GetRecipeChartInfo(recipeID).FourthMaterialId;
        finishFoodList.Add(RecipeChart.instance.GetRecipeChartInfo(recipeID).FourthFinishFoodId);
        float height = 110;
        transform.Find("레시피").Find("Square").GetComponent<RectTransform>().sizeDelta = new Vector2(transform.Find("레시피").Find("Square").GetComponent<RectTransform>().sizeDelta.x, height * toolCount);
        // 해당 레시피의 도구 수만큼 레시피 조리방법 활성화 
        for (int i = 0; i < transform.Find("레시피").Find("Frame").childCount; i++)
        {
            if (i < toolCount)
            {
                transform.Find("레시피").Find("Frame").GetChild(i).gameObject.SetActive(true);
                Transform frameTransform = transform.Find("레시피").Find("Frame").GetChild(i);
                frameTransform.GetChild(0).GetComponent<Image>().sprite = FoodChart.instance.GetFoodChartInfo(finishFoodList[i]).Image;
                for (int j = 1; j < frameTransform.childCount; j++)
                {
                    int[] materialID = new int[0];
                    switch (i)
                    {
                        case 0:
                            materialID = firstMaterialID;
                            break;
                        case 1:
                            materialID = secondMaterialID;
                            break;
                        case 2:
                            materialID = thirdMaterialID;
                            break;
                        case 3:
                            materialID = fourthMaterialID;
                            break;
                    }
                    if (j < materialID.Length+1)
                    {
                        frameTransform.GetChild(j).GetComponent<Image>().sprite = FoodChart.instance.GetFoodChartInfo(materialID[j - 1]).Image;
                        frameTransform.GetChild(j).GetComponent<Image>().SetNativeSize();
                    }
                    else
                    {
                        frameTransform.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                transform.Find("레시피").Find("Frame").GetChild(i).gameObject.SetActive(false);
            }
        }
       
        // STAR
        RewardType[] rewardType = StageChart.instance.GetStageChartInfo(stageId).StarRewardType;
        int[] rewardID = StageChart.instance.GetStageChartInfo(stageId).RewardId;
        for (int i = 0; i < rewardType.Length; i++)
        {
            Transform rewardTransform = transform.Find("STAR").Find("보상");
            if (rewardType[i] == RewardType.Item)
            {
                rewardTransform.GetChild(i).Find("아이템").GetChild(0).GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(rewardID[i]).Image;
            }
            else
            {
                rewardTransform.GetChild(i).Find("아이템").GetChild(0).GetComponent<Image>().sprite = GoodsChart.instance.GetGoodsChartInfo(rewardID[i]).Image;
            }
        }

        // Clear Reward 
        Transform clearRewardTransform = transform.Find("클리어보상").Find("Reward");
        rewardType = StageChart.instance.GetStageChartInfo(stageId).ClearRewardType;
        rewardID = StageChart.instance.GetStageChartInfo(stageId).ClearRewardId;
        int len = rewardType.Length; if (len > clearRewardTransform.childCount) len = clearRewardTransform.childCount;
        for (int i = 0; i < clearRewardTransform.childCount; i++)
        {
            if (i < len)
            {
                clearRewardTransform.GetChild(i).gameObject.SetActive(true);
                if (rewardType[i] == RewardType.Item)
                {
                    clearRewardTransform.GetChild(i).GetComponent<Image>().sprite = ItemChart.instance.GetItemChartChartInfo(rewardID[i]).Image;
                }
                else
                {
                    clearRewardTransform.GetChild(i).GetComponent<Image>().sprite = GoodsChart.instance.GetGoodsChartInfo(rewardID[i]).Image;
                }
            }
            else
            {
                clearRewardTransform.GetChild(i).gameObject.SetActive(false);
            }
        }

        //게임시작
        transform.Find("StartBtn").GetComponent<Button>().onClick.RemoveAllListeners();
        transform.Find("StartBtn").GetComponent<Button>().onClick.AddListener(() => {
            if (UserInfo.instance.GetUserHeartInfo().numberOfHeart < 1)
            {
                Alram("Error_01");
                return;
            }
            UserInfo.instance.GetUserHeartInfo().numberOfHeart--;
            UserInfo.instance.SaveUserHeartInfo(() => {
                stageOpen.SetActive(false);
                blackPannel.SetActive(false);
                GameManager.instance.StoryGameStart(stageId);
            });
        });
    }

    public void TutorialBlackText(string text = "")
    {
        tutorialBlackText.SetActive(true);
        tutorialBlackText.GetComponent<Text>().text = TextChart.instance.GetText(text);
    }
    public void TutorialSpeech(string text, System.Action callback = null)
    {
        tutorialSpeech.SetActive(true);
        speechCoroutine = SpeechCoroutine(text, callback);
        StartCoroutine(speechCoroutine);
    }
    IEnumerator speechCoroutine;
    IEnumerator SpeechCoroutine(string textID, System.Action callback = null)
    {
        Text characterScript = tutorialSpeech.transform.Find("말풍선").GetChild(0).GetComponent<Text>();
        GameObject touchScreen = tutorialSpeech;
        characterScript.text = "";
        string script = TextChart.instance.GetText(textID);
        characterScript.font = TextChart.instance.GetFont();

        touchScreen.GetComponent<Button>().onClick.RemoveAllListeners();
        touchScreen.GetComponent<Button>().onClick.AddListener(() => {
            Speech(script, callback);
        });

        WaitForSeconds waitTime = new WaitForSeconds(0.03f);

        for (int i = 0; i < script.Length; i++)
        {
            characterScript.text += script[i];
            yield return waitTime;
        }

        Speech(script, callback);
    }
    void Speech(string script, System.Action callback = null)
    {
        StopCoroutine(speechCoroutine);

        Text characterScript = tutorialSpeech.transform.Find("말풍선").GetChild(0).GetComponent<Text>();
        GameObject touchScreen = tutorialSpeech;

        characterScript.text = script;

        touchScreen.GetComponent<Button>().onClick.RemoveAllListeners();
        touchScreen.GetComponent<Button>().onClick.AddListener(() => {
            if (callback != null) callback();
            touchScreen.GetComponent<Button>().onClick.RemoveAllListeners();
        });
    }
    public void BtnAlram(System.Action callback, string titleTextID ,string textID)
    {
        btnAlram.gameObject.SetActive(true);

        btnAlram.transform.Find("BG").GetChild(0).GetComponent<Text>().text = TextChart.instance.GetText(titleTextID);
        btnAlram.transform.Find("Text").GetComponent<Text>().text = TextChart.instance.GetText(textID);

        btnAlram.transform.Find("취소").GetChild(0).GetComponent<Text>().text = TextChart.instance.GetText("Button_Cancle");
        btnAlram.transform.Find("취소").GetComponent<Button>().onClick.RemoveAllListeners();
        btnAlram.transform.Find("취소").GetComponent<Button>().onClick.AddListener(() => {
            btnAlram.gameObject.SetActive(false);
        });

        btnAlram.transform.Find("확인").GetChild(0).GetComponent<Text>().text = TextChart.instance.GetText("Button_OK");
        btnAlram.transform.Find("확인").GetComponent<Button>().onClick.RemoveAllListeners();
        btnAlram.transform.Find("확인").GetComponent<Button>().onClick.AddListener(() => {
            callback();
            btnAlram.gameObject.SetActive(false);
        });
    }

    IEnumerator alramCoroutine;
    public void Alram(string textID)
    {
        if (alramCoroutine != null)
        {
            StopCoroutine(alramCoroutine);
        }
        alramCoroutine = AlramCoroutine(textID);
        StartCoroutine(alramCoroutine);
    }
    IEnumerator AlramCoroutine(string textID)
    {
        WaitForSeconds waitTime = new WaitForSeconds(0.02f);

        float fadeSpeed = 0.1f;
        float a = 0;

        alram.transform.Find("Text").GetComponent<Text>().text = TextChart.instance.GetText(textID);
        alram.transform.Find("Text").GetComponent<Text>().color = new Color(1, 1, 1, a);
        alram.transform.Find("BG").GetComponent<Image>().color = new Color(1, 1, 1, a);

        alram.gameObject.SetActive(true);

        while (a < 1)
        {
            a += fadeSpeed;
            alram.transform.Find("Text").GetComponent<Text>().color = new Color(1, 1, 1, a);
            alram.transform.Find("BG").GetComponent<Image>().color = new Color(1, 1, 1, a);
            yield return waitTime;
        }

        yield return new WaitForSeconds(1);

        while (a > 0.05)
        {
            a -= fadeSpeed;
            alram.transform.Find("Text").GetComponent<Text>().color = new Color(1, 1, 1, a);
            alram.transform.Find("BG").GetComponent<Image>().color = new Color(1, 1, 1, a);
            yield return waitTime;
        }
    }
}
