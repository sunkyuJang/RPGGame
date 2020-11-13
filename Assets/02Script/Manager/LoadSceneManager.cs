using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager instance { set; get; }
    public static string loadSecenName { set; get; }
    public Image progressBar;
    static Character Character { set; get; } = null;
    static PlayerData PlayerData { set; get; } = null;
    static Vector3 Position { set; get; }
    public void Awake()
    {
        if (instance = null)
            instance = this;
    }
    public void Start()
    {
        StartCoroutine(LoadScene());
    }
    public static void LoadScene(string sceneName, Character character, Vector3 position)
    {
        loadSecenName = sceneName;
        Character = character;
        Character.Controller.ResetJoystick();
        character.IntoClearUI();
        Position = position;
        SceneManager.LoadScene("LoadingScene");
    }

    public static void LoadingSceneFirstGameStart(PlayerData data)
    {
        loadSecenName = data.LastScene;
        Position = data.LastPosition;
        PlayerData = data;
        SceneManager.LoadScene("LoadingScene");
    }

    public static void LoadScene(string sceneName)
    {
        Character = null;
        loadSecenName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSecenName);
        op.allowSceneActivation = false;

        if (PlayerData != null)
            Character = PlayerDataManager.instance.LoadCharater(PlayerData);

        if (Character != null)
        {
            Character.gameObject.SetActive(true);
            yield return new WaitUntil(() => Character.isCharacterReady);
        }

        float timer = 0.0f;

        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
                if (progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (progressBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    if (Character != null)
                    {
                        // if (!Character.Controller.gameObject.activeSelf)
                        //     Character.Controller.gameObject.SetActive(true);

                        Character.IntoNormalUI();
                        Character.transform.position = Position;
                        GameManager.instance.Character = Character;
                    }
                    Character = null;
                    PlayerData = null;
                    Position = Vector3.zero;

                    yield break;
                }
            }
        }


    }
}
