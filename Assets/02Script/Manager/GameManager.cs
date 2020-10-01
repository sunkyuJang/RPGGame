using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject CharacterPrefab;
    public GameObject SkillDataGroup;
    
    public static GameManager instance;
    public PlayerData playerData;
    public static RectTransform mainCanvas;
    public Controller controller;

    public static string pathOfScenes { get { return "01Scene/"; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            mainCanvas = transform.GetComponent<RectTransform>();
            DontDestroyOnLoad(this.gameObject);

            Instantiate(SkillDataGroup, transform.root.parent);
        }
        else
            Destroy(this.gameObject);
    }

    public void SetGameStart(PlayerData playerData)
    {
        this.playerData = playerData;
        SceneManager.LoadSceneAsync(playerData.LastScene, LoadSceneMode.Single);
        var character = Instantiate(CharacterPrefab, playerData.LastPosition, Quaternion.identity).GetComponent<Character>();
        DontDestroyOnLoad(character.gameObject);
        StartCoroutine(character.SetCharacterWithPlayerData(playerData));
        character.controller = controller;
        controller.Character = character;
    }
}
