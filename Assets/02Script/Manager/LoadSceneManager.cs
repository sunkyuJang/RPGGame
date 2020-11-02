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

    public static bool loadingComplete{private set; get;} = true;
    public void Awake()
    {
        if(instance = null)
            instance = this;
    }
    public void Start()
    {
        StartCoroutine(LoadScene());    
    }
    public static void LoadScene(string sceneName)
    {
        loadingComplete = false;
        loadSecenName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSecenName);
        op.allowSceneActivation = false;

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
                    yield break;
                }
            }
        }

        loadingComplete = true;
    }
}
