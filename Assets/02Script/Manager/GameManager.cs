﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject CharacterPrefab;
    
    public static GameManager instance;
    public PlayerData playerData;
    public static Transform mainCanvas;
    public Controller controller;

    public static string pathOfScenes { get { return "01Scene/"; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            mainCanvas = transform;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        print(pathOfScenes);
    }

    public void SetGameStart(PlayerData playerData)
    {
        this.playerData = playerData;
        SceneManager.LoadSceneAsync(playerData.LastScene, LoadSceneMode.Single);
        var character = Instantiate(CharacterPrefab, playerData.LastPosition, Quaternion.identity).GetComponent<Character>();
        character.SetCharacterWithPlayerData(playerData);
        controller.Character = character;
    }
}
