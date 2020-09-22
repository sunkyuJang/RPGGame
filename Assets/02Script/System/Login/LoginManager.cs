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

    List<PlayerData> playerDatas = new List<PlayerData>();

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
            string id = idField.textComponent.text;
            string pw = pwField.textComponent.text;

            if (id.Length > 5 && pw.Length > 5)
                if (GetNickNameFieldGameObj.activeSelf)
                {
                    string nn = nickNameField.textComponent.text;
                    if (nn.Length > 5)
                        return true;
                }
                else
                    return true;

            return false;
        }
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
                var newData = new PlayerData(idField.textComponent.text, pwField.textComponent.text, nickNameField.textComponent.text);
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
        nickNameField.text = "";

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
            if (data.pw == pwField.textComponent.text)
                idField.textComponent.text = "존재";
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
        idField.textComponent.text = "";
        pwField.textComponent.text = "";
        nickNameField.textComponent.text = "";
    }

    public class PlayerData
    {
        public string id;
        public string pw;
        public string NickName;

        public static string path = Application.dataPath + "/Resources/Managers/SaveData/";
        public string GetJsonPathWithAcc { get { return PlayerData.path + id + ".json"; } }

        public bool isFirstStart;
        public Vector3 LastPosition;
        Dictionary<int, int> inventoryItem = new Dictionary<int, int>();
        public PlayerData(string id, string pw, string nickName)
        {
            this.id = id;
            this.pw = pw;
            NickName = nickName;
            isFirstStart = true;
            LastPosition = Vector3.zero;
        }
    }
}
