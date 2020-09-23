using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerData playerData;
    public static Transform mainCanvas;
    public Controller controller;

    public static string pathOfScenes { get { return "01Scene/"; } }
    private void Awake()
    {
        instance = this;
        mainCanvas = transform;

        print(pathOfScenes);
    }

    public void SetPlayerData(PlayerData playerData)
    {
        this.playerData = playerData;
        SceneManager.LoadSceneAsync(playerData.LastScene, LoadSceneMode.Single);
    }
}
