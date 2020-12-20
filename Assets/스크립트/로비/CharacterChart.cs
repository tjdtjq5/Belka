using BackEnd;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CharacterChart : MonoBehaviour
{
    [SerializeField] string field;
    public static CharacterChart instance;
    public SpriteAtlas characterSpriteAtlas;
    public SpriteAtlas statSpriteAtlas;
    public SpriteAtlas guideCharacterSpriteAtlas;
    public SpriteAtlas rankingCharacterImage;
    private void Awake() { instance = this; }
    public CharacterChartInfo[] characterChartInfos;

    public CharacterChartInfo GetCharacterInfo(CharacterType characterType, GradeType gradeType)
    {
        for (int i = 0; i < characterChartInfos.Length; i++)
        {
            if (characterChartInfos[i].CharacterGroupId == characterType && characterChartInfos[i].Grade == gradeType)
            {
                return characterChartInfos[i];
            }
        }
        return null;
    }

    [System.Obsolete]
    public void LoadChart(System.Action loadAction)
    {
        BackendAsyncClass.BackendAsync(Backend.Chart.GetChartContents, field, (backendCallback) => {
            JsonData jsonData = backendCallback.GetReturnValuetoJSON()["rows"];
            characterChartInfos = new CharacterChartInfo[jsonData.Count];

            for (int i = 0; i < jsonData.Count; i++)
            {
                characterChartInfos[i] = new CharacterChartInfo();
                JsonData rowData = jsonData[i];
                if (rowData["CharacterId"]["S"].ToString() != "null") characterChartInfos[i].CharacterId = int.Parse(rowData["CharacterId"]["S"].ToString());
                if (rowData["CharacterGroupId"]["S"].ToString() != "null") characterChartInfos[i].CharacterGroupId = (CharacterType)int.Parse(rowData["CharacterGroupId"]["S"].ToString());
                if (rowData["Grade"]["S"].ToString() != "null") characterChartInfos[i].Grade = (GradeType)int.Parse(rowData["Grade"]["S"].ToString());
                if (rowData["Image"]["S"].ToString() != "null") characterChartInfos[i].Image = characterSpriteAtlas.GetSprite(rowData["Image"]["S"].ToString());
                if (rowData["StatImage"]["S"].ToString() != "null") characterChartInfos[i].StatImage = statSpriteAtlas.GetSprite(rowData["StatImage"]["S"].ToString());
                if (rowData["Text"]["S"].ToString() != "null") characterChartInfos[i].Text = rowData["Text"]["S"].ToString();
                if (rowData["Inteligence"]["S"].ToString() != "null") characterChartInfos[i].Inteligence = int.Parse(rowData["Inteligence"]["S"].ToString());
                if (rowData["Knowledge"]["S"].ToString() != "null") characterChartInfos[i].Knowledge = int.Parse(rowData["Knowledge"]["S"].ToString());
                if (rowData["Art"]["S"].ToString() != "null") characterChartInfos[i].Art = int.Parse(rowData["Art"]["S"].ToString());
                if (rowData["Passion"]["S"].ToString() != "null") characterChartInfos[i].Passion = int.Parse(rowData["Passion"]["S"].ToString());
                if (rowData["Technology"]["S"].ToString() != "null") characterChartInfos[i].Technology = int.Parse(rowData["Technology"]["S"].ToString());
                if (rowData["Taste"]["S"].ToString() != "null") characterChartInfos[i].Taste = int.Parse(rowData["Taste"]["S"].ToString());
                if (rowData["GuideCharacterImage"]["S"].ToString() != "null") characterChartInfos[i].GuideCharacterImage = guideCharacterSpriteAtlas.GetSprite(rowData["GuideCharacterImage"]["S"].ToString());
                if (rowData["RankingCharacterImage"]["S"].ToString() != "null") characterChartInfos[i].RankingCharacterImage = rankingCharacterImage.GetSprite(rowData["RankingCharacterImage"]["S"].ToString());
            }
            if (loadAction != null) loadAction();
        });
    }
}
[System.Serializable]
public class CharacterChartInfo
{
    public int CharacterId;
    public CharacterType CharacterGroupId;
    public GradeType Grade;
    public Sprite Image;
    public Sprite StatImage;
    public string Text;
    public int Inteligence;
    public int Knowledge;
    public int Art;
    public int Passion;
    public int Technology;
    public int Taste;
    public Sprite GuideCharacterImage;
    public Sprite RankingCharacterImage;
}