using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NicknameChange : MonoBehaviour
{
    public InputField nicknameInput;

    // 한글, 영어, 숫자만 입력 가능하게
    private bool CheckNickname()
    {
        return Regex.IsMatch(nicknameInput.text, "^[0-9a-zA-Z가-힣]*$");
    }
    // 길이가 2~10으로 입력
    bool CheckLength()
    {
        int len = nicknameInput.text.Length;
        if (len >= 2 && len <= 10)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void OK()
    {
        if (!CheckLength())
        {
            return;
        }
        if (!CheckNickname())
        {
            return;
        }
        if (UserInfo.instance.GetUserCrystal() < 10)
        {
            return;
        }

      

        BackendReturnObject bro = Backend.BMember.CreateNickname(nicknameInput.text);

        if (bro.IsSuccess())
        {
            this.gameObject.SetActive(false);
            UserInfo.instance.nickname = nicknameInput.text;

            UserInfo.instance.PullUserCrystal(10);
            UserInfo.instance.SaveUserQuest(() => {
                UserInfo.instance.SaveUserCrystal(() => { });
            });
            UpperInfo.instance.CrystalSet();
        }
     
        nicknameInput.text = "";
    }
}
