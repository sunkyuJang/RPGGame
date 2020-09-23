using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public GameObject GameManagerPrefab;
    public Text title;

    public Button signUpBtn;
    public Button loginBtn;
    public InputField idField;
    public InputField pwField;
    public InputField nickNameField;
    public GameObject GetNickNameFieldGameObj 
    { 
        get 
        { 
            return nickNameField.transform.parent.parent.gameObject; 
        } 
    }

    string loginText = "Login";
    string cancelText = "Cancel";

    private void Awake()
    {
        SetSignUpBtnListener(true);

        SetLoginBtnListener(true);
    }

    void SetSignUpBtnListener(bool isCreatMode)
    {
        if (isCreatMode)
        {
            signUpBtn.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
            signUpBtn.onClick.SetPersistentListenerState(1, UnityEngine.Events.UnityEventCallState.Off);
        }
        else
        {
            signUpBtn.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            signUpBtn.onClick.SetPersistentListenerState(1, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        }
    }

    void SetLoginBtnListener(bool isCheckAccountMode)
    {
        if (isCheckAccountMode)
        {
            loginBtn.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
            loginBtn.onClick.SetPersistentListenerState(1, UnityEngine.Events.UnityEventCallState.Off);
        }
        else
        {
            loginBtn.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            loginBtn.onClick.SetPersistentListenerState(1, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        }
    }

    bool CheckInfomation
    {
        get
        {
            List<string> words = new List<string>();
            words.Add(idField.text);
            words.Add(pwField.text);
            words.Add(nickNameField.text);

            for (int i = 0; i < (GetNickNameFieldGameObj.activeSelf ? 3 : 2); i++)
            {
                if (!IsAllSmallLetter(words[i]))
                    return false;
            }

            if (words[0].Length > 5 && words[1].Length > 5)
                if (GetNickNameFieldGameObj.activeSelf)
                {
                    if (words[2].Length > 5)
                        return true;
                }
                else
                    return true;

            return false;
        }
    }

    bool IsAllSmallLetter(string n)
    {
        for (int i = 0; i < n.Length; i++)
        {
            var nowCharNum = (int)n[i];
            if (97 > nowCharNum || nowCharNum > 122)
                return false;
        }
        return true;
    }

    bool CheckAccountExsit
    {
        get
        {
            string id = idField.textComponent.text;
            string pw = pwField.textComponent.text;

            return File.Exists(PlayerData.path + id + ".json");
        }
    }
    public void CreatAccount()
    {
        title.gameObject.SetActive(false);
        SetSignUpBtnListener(false);

        loginBtn.transform.GetChild(0).GetComponent<Text>().text = cancelText;

        GetNickNameFieldGameObj.SetActive(true);
        SetLoginBtnListener(false);
    }
    public void ConfirmCreatingAccount()
    {
        if (CheckInfomation)
        {
            if (CheckAccountExsit)
            {
                idField.text = "이미 존재하는 계정입니다.";
            }
            else
            {
                var newData = new PlayerData(idField.textComponent.text, pwField.text, nickNameField.textComponent.text);
                File.WriteAllText(newData.GetJsonPathWithAcc, JsonUtility.ToJson(newData, true));
                ClearAllTextField();
                idField.text = "계정이 생성되었습니다.";
                CancelCreatingAccount();
            }
        }
        else
        {
            idField.text = "모든 공란은 6자리 이상으로 채워주세요.";
        }    
    }

    public void CancelCreatingAccount()
    {
        title.gameObject.SetActive(true);

        GetNickNameFieldGameObj.SetActive(false);

        SetSignUpBtnListener(true);
        loginBtn.transform.GetChild(0).GetComponent<Text>().text = loginText;
        SetLoginBtnListener(true);
    }

    public void CheckAccount()
    {
        if (CheckAccountExsit)
        {
            var data = JsonUtility.FromJson<PlayerData>(File.ReadAllText(PlayerData.path + idField.textComponent.text + ".json"));
            if (data.pw == pwField.text)
            {
                var gameManager = Instantiate(GameManagerPrefab).GetComponent<GameManager>();
                gameManager.SetPlayerData(data);
            }
            else
            {
                ClearAllTextField();
                idField.text = "패스워드가 바르지 않습니다.";
            }
        }
        else
        {
            ClearAllTextField();
            idField.text = "존재하지 않는 계정입니다.";
        }
    }

    void ClearAllTextField()
    {
        idField.text = "";
        pwField.text = "";
        nickNameField.text = "";
    }
}
