using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DialogChart : MonoBehaviour
{
    [SerializeField] string field;
    public static DialogChart instance;
    private void Awake() { instance = this; }
    public DialogChartInfo[] dialogChartInfos;

    public SpriteAtlas BGSpriteAtlas;
    public SpriteAtlas BG2SpriteAtlas;
    public SpriteAtlas DialogCharacterSpriteAtlas;

    public List<DialogChartInfo> GetDialogChartInfos(int dialogID)
    {
        List<DialogChartInfo> dialogChartInfoList = new List<DialogChartInfo>();
        for (int i = 0; i < dialogChartInfos.Length; i++)
        {
            if (dialogChartInfos[i].DialogId == dialogID)
            {
                dialogChartInfoList.Add(dialogChartInfos[i]);
            }
        }
        return dialogChartInfoList;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            dialogChartInfos = new DialogChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                dialogChartInfos[i] = new DialogChartInfo();

                JsonData rowData = jsonData[i];
                if (rowData["DialogId"]["S"].ToString() != "null") dialogChartInfos[i].DialogId = int.Parse(rowData["DialogId"]["S"].ToString());
                if (rowData["Step"]["S"].ToString() != "null") dialogChartInfos[i].Step = int.Parse(rowData["Step"]["S"].ToString());
                if (rowData["BG"]["S"].ToString() != "null") dialogChartInfos[i].BG = BGSpriteAtlas.GetSprite(rowData["BG"]["S"].ToString());
                if (rowData["BG2"]["S"].ToString() != "null") dialogChartInfos[i].BG2 = BG2SpriteAtlas.GetSprite(rowData["BG2"]["S"].ToString());
                if (rowData["Effect"]["S"].ToString() != "null") dialogChartInfos[i].Effect = (EffectType)int.Parse(rowData["Effect"]["S"].ToString());
                if (rowData["Character1Image"]["S"].ToString() != "null") dialogChartInfos[i].Character1Image = DialogCharacterSpriteAtlas.GetSprite(rowData["Character1Image"]["S"].ToString());
                if (rowData["Character1Position"]["S"].ToString() != "null") dialogChartInfos[i].Character1Position = (PositionType)int.Parse(rowData["Character1Position"]["S"].ToString());
                if (rowData["Character2Image"]["S"].ToString() != "null") dialogChartInfos[i].Character2Image = DialogCharacterSpriteAtlas.GetSprite(rowData["Character2Image"]["S"].ToString());
                if (rowData["Character2Position"]["S"].ToString() != "null") dialogChartInfos[i].Character2Position = (PositionType)int.Parse(rowData["Character2Position"]["S"].ToString());
                if (rowData["Text"]["S"].ToString() != "null") dialogChartInfos[i].Text = rowData["Text"]["S"].ToString();
                if (rowData["TextName"]["S"].ToString() != "null") dialogChartInfos[i].TextName = rowData["TextName"]["S"].ToString();
                if (rowData["ActiveCharacter"]["S"].ToString() != "null") dialogChartInfos[i].ActiveCharacter = int.Parse(rowData["ActiveCharacter"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class DialogChartInfo
{
    public int DialogId;
    public int Step;
    public Sprite BG = null;
    public Sprite BG2 = null;
    public EffectType Effect = EffectType.없음;
    public Sprite Character1Image = null;
    public PositionType Character1Position;
    public Sprite Character2Image = null;
    public PositionType Character2Position;
    public string Text;
    public string TextName;
    public int ActiveCharacter;
}