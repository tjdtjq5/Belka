using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UnderButton : MonoBehaviour
{
    public float tweenSpeed;

    public Transform pannel;
    public Transform pannel2;
    public Transform pannel3;

    float ScreenSizeX = 1080;

    public Transform shopBtn;
    public Transform characterBtn;
    public Transform stageBtn;
    public Transform rankingBtn;

    public Transform shopIcon;
    public Transform characterIcon;
    public Transform stageIcon;
    public Transform rankingIcon;

    static int index = 2;

    private void Start()
    {
        SoundManager.instance.LobbyBgmStart();

        switch (index)
        {
            case 0:
                SizeDeltaTrans(shopBtn);
                pannel.localPosition = new Vector2(ScreenSizeX * 2, pannel.localPosition.y);
                pannel2.localPosition = new Vector2(ScreenSizeX * 2, pannel.localPosition.y);
                pannel3.localPosition = new Vector2(ScreenSizeX * 2, pannel.localPosition.y);
                break;
            case 1:
                SizeDeltaTrans(characterBtn);
                pannel.localPosition = new Vector2(ScreenSizeX, pannel.localPosition.y);
                pannel2.localPosition = new Vector2(ScreenSizeX, pannel.localPosition.y);
                pannel3.localPosition = new Vector2(ScreenSizeX, pannel.localPosition.y);
                break;
            case 2:
                SizeDeltaTrans(stageBtn);
                pannel.localPosition = new Vector2(0, pannel.localPosition.y);
                pannel2.localPosition = new Vector2(0, pannel.localPosition.y);
                pannel3.localPosition = new Vector2(0, pannel.localPosition.y);
                break;
            case 3:
                SizeDeltaTrans(rankingBtn);
                pannel.localPosition = new Vector2(-ScreenSizeX * 2, pannel.localPosition.y);
                pannel2.localPosition = new Vector2(-ScreenSizeX * 2, pannel.localPosition.y);
                pannel3.localPosition = new Vector2(-ScreenSizeX * 2, pannel.localPosition.y);
                break;
        }
    }

    public void OnClickShop()
    {
        if (Tutorial02Manager.instance.tutorialFlag && Tutorial02Manager.instance.underBtnFlag != 0)
        {
            return;
        }
        index = 0;
        SizeDeltaTrans(shopBtn);
        pannel.DOLocalMoveX(ScreenSizeX * 2, tweenSpeed);
        pannel2.DOLocalMoveX(ScreenSizeX * 2, tweenSpeed);
        pannel3.DOLocalMoveX(ScreenSizeX * 2, tweenSpeed);
    }
    public void OnClickCharacter()
    {
        if (Tutorial02Manager.instance.tutorialFlag && Tutorial02Manager.instance.underBtnFlag != 1)
        {
            return;
        }
        if (Tutorial02Manager.instance.tutorialFlag) Tutorial02Manager.instance.NextStep();
        index = 1;
        SizeDeltaTrans(characterBtn);
        pannel.DOLocalMoveX(ScreenSizeX, tweenSpeed);
        pannel2.DOLocalMoveX(ScreenSizeX, tweenSpeed);
        pannel3.DOLocalMoveX(ScreenSizeX, tweenSpeed);
    }
    public void OnClickStage()
    {
        if (Tutorial02Manager.instance.tutorialFlag && Tutorial02Manager.instance.underBtnFlag != 2)
        {
            return;
        }
        index = 2;
        SizeDeltaTrans(stageBtn);
        pannel.DOLocalMoveX(0, tweenSpeed);
        pannel2.DOLocalMoveX(0, tweenSpeed);
        pannel3.DOLocalMoveX(0, tweenSpeed);
    }
    public void OnClickRanking()
    {
        if (Tutorial02Manager.instance.tutorialFlag && Tutorial02Manager.instance.underBtnFlag != 3)
        {
            return;
        }
        index = 3;
        SizeDeltaTrans(rankingBtn);
        pannel.DOLocalMoveX(-ScreenSizeX, tweenSpeed);
        pannel2.DOLocalMoveX(-ScreenSizeX, tweenSpeed);
        pannel3.DOLocalMoveX(-ScreenSizeX, tweenSpeed);
    }


    void LeftDrag()
    {
        index--;
        if (index < 0)
        {
            index = 0;
        }
        switch (index)
        {
            case 0:
                OnClickShop();
                break;
            case 1:
                OnClickCharacter();
                break;
            case 2:
                OnClickStage();
                break;
            case 3:
                OnClickRanking();
                break;
        }
    }
    void RightDrag()
    {
        index++;
        if (index > 3)
        {
            index = 3;
        }
        switch (index)
        {
            case 0:
                OnClickShop();
                break;
            case 1:
                OnClickCharacter();
                break;
            case 2:
                OnClickStage();
                break;
            case 3:
                OnClickRanking();
                break;
        }
    }

    bool mouseFlag = false;
    Vector2 tempMousePos;
    float distanceX = 60; // X축의 이 거리 이상 떨어진 경우 
    float dirDistance = 30; // 각도가 이만큼 떨어진 경우
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!mouseFlag)
            {
                mouseFlag = true;
                tempMousePos = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (mouseFlag)
            {
                mouseFlag = false;

                Vector2 mousePos = Input.mousePosition;
                float x = tempMousePos.x - mousePos.x;

                float dir = CalculateAngle(tempMousePos, mousePos);

                if (dir > 270 - dirDistance && dir < 270 + dirDistance || dir > 90 - dirDistance && dir < 90 + dirDistance)
                {
                    if (x > distanceX)
                    {
                        RightDrag();
                    }
                    else if (Mathf.Abs(x) - tempMousePos.x > distanceX)
                    {
                        LeftDrag();
                    }
                }
            }
        }
        if (PopupManager.instance.blackPannel.activeSelf)
        {
            mouseFlag = false;
        }
    }

    public float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }



    public void SizeDeltaTrans(Transform sizeUpBtn)
    {
        Vector2 originSize = new Vector2(216, 220);
        Vector2 upSize = new Vector2(432, 220);

        Vector2 originScalie = new Vector2(1.6f, 1.6f);
        Vector2 upScalie = new Vector2(2, 2);


        if (sizeUpBtn == shopBtn)
        {
            shopBtn.GetComponent<RectTransform>().DOSizeDelta(upSize, tweenSpeed);
            shopIcon.DOScale(upScalie, tweenSpeed);
            shopIcon.GetComponent<Image>().color = Color.white;
        }
        else
        {
            shopBtn.GetComponent<RectTransform>().DOSizeDelta(originSize, tweenSpeed);
            shopIcon.DOScale(originScalie, tweenSpeed);
            shopIcon.GetComponent<Image>().color = Color.gray;
        }
        if (sizeUpBtn == characterBtn)
        {
            characterBtn.GetComponent<RectTransform>().DOSizeDelta(upSize, tweenSpeed);
            characterIcon.DOScale(upScalie, tweenSpeed);
            characterIcon.GetComponent<Image>().color = Color.white;
        }
        else
        {
            characterBtn.GetComponent<RectTransform>().DOSizeDelta(originSize, tweenSpeed);
            characterIcon.DOScale(originScalie, tweenSpeed);
            characterIcon.GetComponent<Image>().color = Color.gray;
        }
        if (sizeUpBtn == stageBtn)
        {
            stageBtn.GetComponent<RectTransform>().DOSizeDelta(upSize, tweenSpeed);
            stageIcon.DOScale(upScalie, tweenSpeed);
            stageIcon.GetComponent<Image>().color = Color.white;
        }
        else
        {
            stageBtn.GetComponent<RectTransform>().DOSizeDelta(originSize, tweenSpeed);
            stageIcon.DOScale(originScalie, tweenSpeed);
            stageIcon.GetComponent<Image>().color = Color.gray;
        }
        if (sizeUpBtn == rankingBtn)
        {
            rankingBtn.GetComponent<RectTransform>().DOSizeDelta(upSize, tweenSpeed);
            rankingIcon.DOScale(upScalie, tweenSpeed);
            rankingIcon.GetComponent<Image>().color = Color.white;
        }
        else
        {
            rankingBtn.GetComponent<RectTransform>().DOSizeDelta(originSize, tweenSpeed);
            rankingIcon.DOScale(originScalie, tweenSpeed);
            rankingIcon.GetComponent<Image>().color = Color.gray;
        }
    }
}
