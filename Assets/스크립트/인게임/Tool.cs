using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tool : MonoBehaviour
{
    public int[] materialList = new int[0];
    float collTime = 0;
    int finishFoodId = 0;
    int childNumber = 0;

    List<int> materialSetList = new List<int>();
    Image foreGuage = null;
    IEnumerator toolCoroutine;
    IEnumerator shakeCoroutine;

    public AudioSource putlnAudioSource;
    public AudioSource putSetAudioSource;
    public GameObject smokePuff;

    public void SetTool(int childNumber,int toolId,int[] materialList, float collTime, int finishFoodId)
    {
        this.materialList = materialList;
        this.finishFoodId = finishFoodId;
        this.childNumber = childNumber;

        float value = 0;
        StatType[] statType = ToolChart.instance.GetToolChartInfo(toolId).Stat;
        float[] statValue = ToolChart.instance.GetToolChartInfo(toolId).StatValue;
        float characterValue = 0;
        for (int i = 0; i < statType.Length; i++)
        {
            switch (statType[i])
            {
                case StatType.Inteligence:
                    characterValue = CharacterChart.instance.GetCharacterInfo(UserInfo.instance.GetUserCharacterInfo().eqipCharacter, UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)UserInfo.instance.GetUserCharacterInfo().eqipCharacter]).Inteligence;
                    break;
                case StatType.Knoledge:
                    characterValue = CharacterChart.instance.GetCharacterInfo(UserInfo.instance.GetUserCharacterInfo().eqipCharacter, UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)UserInfo.instance.GetUserCharacterInfo().eqipCharacter]).Knowledge;
                    break;
                case StatType.Art:
                    characterValue = CharacterChart.instance.GetCharacterInfo(UserInfo.instance.GetUserCharacterInfo().eqipCharacter, UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)UserInfo.instance.GetUserCharacterInfo().eqipCharacter]).Art;
                    break;
                case StatType.Passion:
                    characterValue = CharacterChart.instance.GetCharacterInfo(UserInfo.instance.GetUserCharacterInfo().eqipCharacter, UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)UserInfo.instance.GetUserCharacterInfo().eqipCharacter]).Passion;
                    break;
                case StatType.Technology:
                    characterValue = CharacterChart.instance.GetCharacterInfo(UserInfo.instance.GetUserCharacterInfo().eqipCharacter, UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)UserInfo.instance.GetUserCharacterInfo().eqipCharacter]).Technology;
                    break;
                case StatType.Taste:
                    characterValue = CharacterChart.instance.GetCharacterInfo(UserInfo.instance.GetUserCharacterInfo().eqipCharacter, UserInfo.instance.GetUserCharacterInfo().characterGrade[(int)UserInfo.instance.GetUserCharacterInfo().eqipCharacter]).Taste;
                    break;
            }

            value += characterValue * statValue[i]; // xx%형식
        }

        this.collTime = collTime - (collTime * value / 100);
        if (this.collTime < 0) this.collTime = 0;

        int len = materialList.Length;
        if (len > this.transform.Find("상단").childCount) len = this.transform.Find("상단").childCount;

        for (int i = 0; i < this.transform.Find("상단").childCount; i++)
        {
            if (i < len)
            {
                this.transform.Find("상단").GetChild(i).gameObject.SetActive(true);
                this.transform.Find("상단").GetChild(i).GetChild(0).GetComponent<Image>().sprite = FoodChart.instance.GetFoodChartInfo(materialList[i]).Image;
                this.transform.Find("상단").GetChild(i).GetChild(0).GetComponent<Image>().SetNativeSize();
            }
            else
            {
                this.transform.Find("상단").GetChild(i).gameObject.SetActive(false);
            }
        }

        foreGuage = this.transform.Find("하단").Find("foreGauge").GetComponent<Image>();
        foreGuage.fillAmount = 0;

        materialSetList.Clear();
    }

    public bool InputMaterial(int materialId)
    {
        if (foreGuage.fillAmount != 0) return false; // 음식이 동작중이면 false
 
        bool returnFlag = false;
        for (int i = 0; i < materialList.Length; i++)
        {
            if (materialList[i] == materialId)
            {
                if (!materialSetList.Contains(materialId))
                {
                    returnFlag = true;
                    materialSetList.Add(materialId);
                }
            }
        }

        // 음식을 넣는데 성공한 경우 
        if (materialSetList.Count == materialList.Length) // 음식을 넣은것을 확인해보니 다 담겨진 경우 
        {
            SoundManager.instance.SfxPlay(putSetAudioSource);
            this.GetComponent<Animator>().SetBool("isPlay", true);
        }
        else 
        {
            SoundManager.instance.SfxPlay(putlnAudioSource);
        }

        if (returnFlag) // 음식을 넣는데 성공한 경우 
        {
            for (int i = 0; i < this.transform.Find("상단").childCount; i++)
            {
                if (this.transform.Find("상단").GetChild(i).GetChild(0).GetComponent<Image>().sprite == FoodChart.instance.GetFoodChartInfo(materialId).Image)
                {
                    this.transform.Find("상단").GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;
                    this.transform.Find("상단").GetChild(i).GetComponent<Image>().color = Color.green;
                }
            }
            
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            this.transform.Find("Pannel").Find("ToolImg").rotation = Quaternion.Euler(0, 0, 0);

            shakeCoroutine = ShakeCoroutine();
            StartCoroutine(shakeCoroutine);

            GameObject particle = Instantiate(smokePuff, this.transform.position, Quaternion.identity);
            particle.GetComponent<ParticleSystem>().Play();
        }

        if (materialSetList.Count == materialList.Length) // 음식을 넣은것을 확인해보니 다 담겨진 경우 
        {
            toolCoroutine = ToolCoroutine();
            StartCoroutine(toolCoroutine);
        }

        return returnFlag;
    }

    IEnumerator ToolCoroutine()
    {
        float currentTime = TimeManager.instance.readTime;
        float nextTime = TimeManager.instance.readTime;

        while (foreGuage.fillAmount < 1)
        {

            if (foreGuage.fillAmount < 0.5f) 
            {
                foreGuage.color = Color.yellow;
            }
            else 
            {
                foreGuage.color = Color.green;
            }

            yield return TimeManager.instance.waitTime;
            nextTime = TimeManager.instance.readTime;
            foreGuage.fillAmount = (nextTime - currentTime) / collTime;
        }

        foreGuage.fillAmount = 1;

        // 도구 초기화 
        InitTool();

        /*/ 음식 놓기 
        SetMetarial(childNumber, finishFoodId); */
    }

    IEnumerator ShakeCoroutine()
    {
        Transform transform = this.transform.Find("Pannel").Find("ToolImg");
        float z = 0;

        float p = 6f;
        WaitForSeconds waitTime = new WaitForSeconds(0.01f);

        while (z > -30)
        {
            z -= p;
            this.transform.Find("Pannel").Find("ToolImg").rotation = Quaternion.Euler(0, 0, z);
            yield return waitTime;
        }
        while (z < 30)
        {
            z += p;
            this.transform.Find("Pannel").Find("ToolImg").rotation = Quaternion.Euler(0, 0, z);
            yield return waitTime;
        }

        while (z > 0)
        {
            z -= p;
            this.transform.Find("Pannel").Find("ToolImg").rotation = Quaternion.Euler(0, 0, z);
            yield return waitTime;
        }

        this.transform.Find("Pannel").Find("ToolImg").rotation = Quaternion.Euler(0, 0, 0);
    }

    public void SoundStop()
    {
        putlnAudioSource.Stop();
        putSetAudioSource.Stop();
    }

    // 도구로 음식을 만드는데 성공
    void SetMetarial(int childNumber, int foodId)
    {
        // 준재료에 넣기 
        if (SceneManager.GetActiveScene().name == "인게임")
        {
            Material02Manager.instance.MaterialSet(childNumber, foodId);
        }

        if (SceneManager.GetActiveScene().name == "인게임튜토리얼")
        {
            TutorialMaterial02Manager.instance.MaterialSet(childNumber, foodId);
            TutorialManager.instance.NextStep();
        }
    }
    // 도구 초기화 
    void InitTool()
    {
        if (toolCoroutine != null) StopCoroutine(toolCoroutine);

        // 초기화 
        foreGuage.fillAmount = 0;
        materialSetList.Clear();
        for (int i = 0; i < this.transform.Find("상단").childCount; i++)
        {
            if (this.transform.Find("상단").GetChild(i).GetChild(0).GetComponent<Image>().color == Color.white)
            {
                this.transform.Find("상단").GetChild(i).GetComponent<Image>().color = Color.white;
            }
        }

        // 음향 , 애니 
        putSetAudioSource.Stop();
        this.GetComponent<Animator>().SetBool("isPlay", false);
    }

    public void TouchTool()
    {
        if (foreGuage.fillAmount == 0 || foreGuage.fillAmount == 1) return;

        if (foreGuage.fillAmount < 0.5f) // 실패 
        {
            InitTool();
        }
        else // 성공 
        {
            SetMetarial(childNumber, finishFoodId);
            InitTool();
        }
    }
}
