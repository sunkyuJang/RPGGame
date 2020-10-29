using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance { private set; get; }
    public string PlayerDataPath { private set; get; }

    Controller Controller { set; get; }
    public GameObject frame;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            frame.SetActive(false);
            PlayerDataPath = Application.dataPath + "/Resources/Managers/SaveData/";
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public bool IsDataExist(string ID)
    {
        return File.Exists(PlayerDataPath + ID + ".json");
    }

    public PlayerData GetPlayerData(string id)
    {
        return JsonUtility.FromJson<PlayerData>(File.ReadAllText(PlayerDataPath + id + ".json"));
    }

    public bool IsPWCorrect(PlayerData playerData, string pw)
    {
        return playerData.pw == pw;
    }

    public void CreatPlayerData(string id, string pw, string nickName)
    {
        var newData = new PlayerData(id, pw, nickName);
        File.WriteAllText(newData.GetJsonPathWithAcc, JsonUtility.ToJson(newData, true));
    }

    public void ShowView(Controller controller)
    {
        gameObject.transform.SetAsLastSibling();
        Controller = controller;
        frame.SetActive(true);
    }

    public void SavePlayerDataToJson()
    {
        Controller.playerData.SetPlayerDataFromCharacter(Controller.Character);
        File.WriteAllText(Controller.playerData.GetJsonPathWithAcc, JsonUtility.ToJson(Controller.playerData, true));
    }

    public void PressedReturnMainScreenBtn()
    {
        StartCoroutine(ConfirmToReturnMainScene());
    }

    IEnumerator ConfirmToReturnMainScene()
    {
        ConfimBoxManager.instance.ShowComfirmBox("저장하지 않은 데이터는 소실됩니다.\r\n메인 화면으로 돌아가시겠습니까?");
        yield return new WaitWhile(() => ConfimBoxManager.instance.NowState == ConfimBoxManager.State.Waiting);
        if (ConfimBoxManager.instance.NowState == ConfimBoxManager.State.Yes)
        {
            Controller.Character.gameObject.SetActive(false);
            frame.SetActive(false);
            SceneManager.LoadSceneAsync(GameManager.pathOfScenes + "LoginScene");
            LoginManager.instance.ShowView();
        }
    }

    public void Hide()
    {
        Controller.SetAllActive(true);
        Controller.Character.IntoNormalUI();
        frame.SetActive(false);
    }
}
