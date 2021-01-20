using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Puzzle02 : MonoBehaviour
{
    public int childNumber = 0;
    public int foodId = 0;
    public Camera theCam;

    Vector3 originLocalPos;
    private void Start()
    {
        originLocalPos = this.transform.GetChild(0).transform.localPosition;

        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
        entry_PointerDown.eventID = EventTriggerType.PointerDown;
        entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerDown);

        EventTrigger.Entry entry_PointerUp = new EventTrigger.Entry();
        entry_PointerUp.eventID = EventTriggerType.PointerUp;
        entry_PointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerUp);
    }
    public void SetFood(int foodId, int childNumber)
    {
        this.foodId = foodId;
        this.childNumber = childNumber;
        this.transform.GetChild(0).DOScale(1, 0);
        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(0).DOScale(1.3f, .5f);
        this.transform.GetChild(0).GetComponent<Image>().sprite = FoodChart.instance.GetFoodChartInfo(foodId).Image;
        this.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
    }

    public void DeleteFood()
    {
        foodId = 0;
        this.transform.GetChild(0).gameObject.SetActive(false);
    }

    bool pointDownFlag = false;
    GameObject prepab;

    void OnPointerDown(PointerEventData data)
    {
        if (this.transform.GetChild(0).gameObject.activeSelf)
        {
            if (prepab != null)
            {
                Destroy(prepab);
                prepab = null;
            }
            this.transform.GetChild(0).gameObject.SetActive(false);
            pointDownFlag = true;
            prepab = Resources.Load<GameObject>("프리팹/이미지프리팹") as GameObject;
            prepab = Instantiate(prepab, Vector3.zero, Quaternion.identity);
            prepab.transform.GetChild(0).GetComponent<Image>().sprite = this.transform.GetChild(0).GetComponent<Image>().sprite;
            prepab.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
        }
    }
    void OnPointerUp(PointerEventData data)
    {
        if (!this.transform.GetChild(0).gameObject.activeSelf && foodId != 0)
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
            pointDownFlag = false;
            GameObject tempPrepab = prepab;
            prepab = null;

            Vector2 mousePosition = theCam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (!hit)
            {
                Destroy(tempPrepab);
            }
            if (hit && !hit.transform.GetComponent<Tool>())
            {
                Destroy(tempPrepab);
            }

            if (hit && hit.transform.GetComponent<Tool>())
            {
                if (hit.transform.GetComponent<Tool>().InputMaterial(foodId))
                {
                    //음식넣기 성공
                    DeleteFood();
                    Vector2 hitPosition = theCam.WorldToScreenPoint(hit.transform.position);
                    tempPrepab.transform.GetChild(0).DOMove(new Vector2(hitPosition.x, hitPosition.y - 25), .8f).SetEase(Ease.InQuad);
                    tempPrepab.transform.GetChild(0).DOScale(.4f, .8f);
                    tempPrepab.transform.GetChild(0).GetComponent<Image>().DOFade(.4f, 0.8f).OnComplete(() => {
                        Destroy(tempPrepab); 
                    });
                }
                else
                {
                    //음식넣기 실패
                    Destroy(tempPrepab);
                }
            }

            if (hit && hit.transform.GetComponent<FoodManager>())
            {
                if (hit.transform.GetComponent<FoodManager>().SetFood(foodId))
                {
                    //음식넣기 성공
                    DeleteFood();
                }
                else
                {
                    //음식넣기 실패
                }
            }
        }
    }

    private void Update()
    {
        if (pointDownFlag)
        {
            prepab.transform.GetChild(0).position = Input.mousePosition;
        }
    }
}
