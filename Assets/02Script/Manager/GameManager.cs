using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using GLip;

public class GameManager : MonoBehaviour
{
    public GameObject CharacterPrefab;
    public Character Character { set; get; }
    public GameObject SkillDataGroup;
    public GameObject managerGroup;
    
    public static GameManager instance;
    public static RectTransform mainCanvas;
    public Controller controller;

    public List<GameObject> DestroyAfterIntoLoginScene { set; get; } = new List<GameObject>();
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
        GPosition.GetRectTransformWithReset(transform.GetComponent<RectTransform>(), PlayerDataManager.instance.transform.GetComponent<RectTransform>());
        StartCoroutine(ProgressSetStartGame(playerData));
    }
    public IEnumerator ProgressSetStartGame(PlayerData playerData)
    {
        yield return null;

        managerGroup.SetActive(true);

        if (Character == null)
        {
            Character = Instantiate(CharacterPrefab, playerData.LastPosition, Quaternion.identity).GetComponent<Character>();
            DontDestroyOnLoad(Character.gameObject);
        }
        else if (!Character.gameObject.activeSelf)
            Character.gameObject.SetActive(true);
        
        StartCoroutine(Character.SetCharacterWithPlayerData(playerData));
        Character.Rigidbody.useGravity = false;

        controller.gameObject.SetActive(true);
        controller.SetPlayerData(playerData);
        Character.controller = controller;
        controller.Character = Character;

        yield return new WaitUntil(() => Character.isCharacterReady);

        LoadSceneManager.LoadScene(pathOfScenes + playerData.LastScene);
        while (!SceneManager.GetActiveScene().name.Equals(playerData.LastScene))
            yield return new WaitForFixedUpdate();

        Character.transform.position = playerData.LastPosition;
        Character.Rigidbody.useGravity = true;
    }

    public IEnumerator SetSceneChange(string sceneName, Character character, Vector3 poisition)
    {
        character.gameObject.SetActive(false);
        LoadSceneManager.LoadScene(pathOfScenes + sceneName);

        yield return new WaitUntil(() => LoadSceneManager.loadingComplete);
        character.isReviver = false;
        character.transform.position = poisition;
        character.gameObject.SetActive(true);
    }
}
