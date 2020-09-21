using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public Text title;

    public Button signUpBtn;
    public Button loginBtn;
    public InputField idField;
    public InputField pwField;

    List<PlayerData> playerDatas = new List<PlayerData>();

    string gameName = "Proto";
    string singUpAcc = "SingUp";
    string loginText = "Login";
    string cancelText = "Cancel";
    private void Awake()
    {
        signUpBtn.onClick.AddListener(CreatAccount);
        loginBtn.onClick.AddListener(CheckAccount);
    
        
    }

    public void CheckAccount()
    {
        string id = idField.textComponent.text;
        string pw = pwField.textComponent.text;


    }
    public void CreatAccount()
    {
        title.text = singUpAcc;
        signUpBtn.onClick.AddListener(ConfirmCreatingAccount);

        loginBtn.transform.GetChild(0).GetComponent<Text>().text = cancelText;
        loginBtn.onClick.AddListener(CancelCreatingAccount);
        
    }
    public void ConfirmCreatingAccount()
    {
        string id = idField.textComponent.text;
        string pw = pwField.textComponent.text;

        print(PlayerData.path);
        PlayerData nowData = new PlayerData(id, "jk");
        //File.WriteAllText(PlayerData.path + id + ".json", JsonUtility.ToJson(data));
        print(JsonUtility.ToJson(nowData, true));
        print(nowData.ID);
    }

    public void CancelCreatingAccount()
    {
        title.text = gameName;
        loginBtn.transform.GetChild(0).GetComponent<Text>().text = loginText;
        loginBtn.onClick.AddListener(CheckAccount);
    }

    [SerializeField]
    public class PlayerData
    {
        public string id;
        public string pw;
        public string NickName;

        public static string path = Application.dataPath + "/Resources/Managers/SaveData/";
        public PlayerData(string id, string nickName)
        {
            ID = id;
            NickName = nickName;
        }

        public void CreateJsonFile()
        {
            var fileStream = new FileStream(string.Format("{0}/{1}.json", Application.dataPath, ID), FileMode.Create);
        }
    }
}
