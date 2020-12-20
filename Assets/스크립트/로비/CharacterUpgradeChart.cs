using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CharacterUpgradeChart : MonoBehaviour
{
    [SerializeField] string field;
    public static CharacterUpgradeChart instance;
    private void Awake() { instance = this; }
    public SpriteAtlas characterUpgradeSpriteAtlas;
    public CharacterUpgradeChartInfo[] characterUpgradeChartInfos;

    public CharacterUpgradeChartInfo GetCharacterUpgradeChartInfo(CharacterType characterType, GradeType gradeType)
    {
        for (int i = 0; i < characterUpgradeChartInfos.Length; i++)
        {
            if (characterUpgradeChartInfos[i].CharacterGroupId == characterType && characterUpgradeChartInfos[i].Grade == gradeType)
            {
                return characterUpgradeChartInfos[i];
            }
        }
        return null;
    }
    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            characterUpgradeChartInfos = new CharacterUpgradeChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                characterUpgradeChartInfos[i] = new CharacterUpgradeChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["CharacterUpgradeId"]["S"].ToString() != "null") characterUpgradeChartInfos[i].CharacterUpgradeId = int.Parse(rowData["CharacterUpgradeId"]["S"].ToString());
                if (rowData["CharacterGroupId"]["S"].ToString() != "null") characterUpgradeChartInfos[i].CharacterGroupId = (CharacterType)int.Parse(rowData["CharacterGroupId"]["S"].ToString());
                if (rowData["Grade"]["S"].ToString() != "null") characterUpgradeChartInfos[i].Grade = (GradeType)int.Parse(rowData["Grade"]["S"].ToString());
                if (rowData["UpgradeItemId"]["S"].ToString() != "null") characterUpgradeChartInfos[i].UpgradeItemId = int.Parse(rowData["UpgradeItemId"]["S"].ToString());
                if (rowData["UpgradeItemCount"]["S"].ToString() != "null") characterUpgradeChartInfos[i].UpgradeItemCount = int.Parse(rowData["UpgradeItemCount"]["S"].ToString());
                if (rowData["StageId"]["S"].ToString() != "null") characterUpgradeChartInfos[i].StageId = int.Parse(rowData["StageId"]["S"].ToString());
                if (rowData["HeartCount"]["S"].ToString() != "null") characterUpgradeChartInfos[i].HeartCount = int.Parse(rowData["HeartCount"]["S"].ToString());
                if (rowData["UpgradeImage"]["S"].ToString() != "null") characterUpgradeChartInfos[i].UpgradeImage = characterUpgradeSpriteAtlas.GetSprite(rowData["UpgradeImage"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class CharacterUpgradeChartInfo
{
    public int CharacterUpgradeId;
    public CharacterType CharacterGroupId;
    public GradeType Grade;
    public int UpgradeItemId;
    public int UpgradeItemCount;
    public int StageId;
    public int HeartCount;
    public Sprite UpgradeImage;
}