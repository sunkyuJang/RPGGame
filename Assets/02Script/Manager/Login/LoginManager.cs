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
    public static LoginManager instance { set; get; }
    public GameObject frame;

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

    public string GetNowIDFieldText { get { return idField.textComponent.text; } }
    public string GetNowPWFieldText { get { return pwField.text; } }
    public string GetNowNickNameFieldText { get { return nickNameField.textComponent.text; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SetSignUpBtnListener(true);

            SetLoginBtnListener(true);
        }
        else
        {
            Destroy(gameObject);
        }
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
            if (PlayerDataManager.instance.IsDataExist(GetNowIDFieldText))
            {
                idField.text = "이미 존재하는 계정입니다.";
            }
            else
            {
                PlayerDataManager.instance.CreatPlayerData(GetNowIDFieldText, GetNowPWFieldText, GetNowNickNameFieldText);
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
        if (PlayerDataManager.instance.IsDataExist(GetNowIDFieldText))
        {
            var data = PlayerDataManager.instance.GetPlayerData(GetNowIDFieldText);
            print(data.pw + "//" + GetNowPWFieldText);
            if (PlayerDataManager.instance.IsPWCorrect(data, GetNowPWFieldText))
            {
                /*var gameManager = Instantiate(GameManagerPrefab).GetComponent<GameManager>();
                gameManager.transform.position = Vector3.zero;
                gameManager.SetGameStart(data);*/
                GameManager.instance.SetGameStart(data);
                HideView();
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

    public void ShowView() 
    {
        frame.SetActive(true);
    }

    public void HideView()
    {
        frame.SetActive(false);
    }
}
