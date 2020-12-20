using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChart : MonoBehaviour
{
    [SerializeField] string field;
    public static TextChart instance;

    public Font koFont;
    public Font enFont;

    private void Awake() { instance = this; }
    public TextChartInfo[] textChartInfos;


    public NationType nationType = NationType.Ko;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("NationType"))
        {
            if (Application.systemLanguage == SystemLanguage.Korean)
            {
                nationType = NationType.Ko;
            }
            else
            {
                nationType = NationType.En;
            }
        }
        else
        {
            if (PlayerPrefs.GetString("NationType") == "Ko")
            {
                nationType = NationType.Ko;
            }
            else
            {
                nationType = NationType.En;
            }
        }
    }

    public string GetText(string textId)
    {
        for (int i = 0; i < textChartInfos.Length; i++)
        {
            if (textChartInfos[i].TextId == textId)
            {
                if (nationType == NationType.Ko)
                {
                    string text = textChartInfos[i].Ko.Replace('*', '\n');
                    return text;
                }
                else
                {
                    string text = textChartInfos[i].En.Replace('*', '\n');
                    return text;
                }
            }
        }
        return "텍스트가 없습니다.";
    }
    public Font GetFont()
    {
        if (nationType == NationType.Ko)
        {
            return koFont;
        }
        else
        {
            return enFont;
        }
    }
    public FontStyle GetFontStyle()
    {
        if (nationType == NationType.Ko)
        {
            return FontStyle.Normal;
        }
        else
        {
            return FontStyle.Bold;
        }
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            textChartInfos = new TextChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                textChartInfos[i] = new TextChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["TextId"]["S"].ToString() != "null") textChartInfos[i].TextId = rowData["TextId"]["S"].ToString();
                if (rowData["Ko"]["S"].ToString() != "null") textChartInfos[i].Ko = rowData["Ko"]["S"].ToString();
                if (rowData["En"]["S"].ToString() != "null") textChartInfos[i].En = rowData["En"]["S"].ToString();
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class TextChartInfo
{
    public string TextId;
    public string Ko;
    public string En;
}