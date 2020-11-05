using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using GLip;

public class GameManager : MonoBehaviour
{
    //public GameObject CharacterPrefab;
    public Character Character { set; get; } = null;
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
        managerGroup.SetActive(true);

        if (Character == null)
        {
            Character = PlayerDataManager.instance.LoadCharater(playerData);
            Character.controller = controller;
            controller.Character = Character;
        }
        else
        {
            Character.gameObject.SetActive(true);
            Character.SetCharacterWithPlayerData(playerData);
        }

        LoadSceneManager.LoadScene(playerData.LastScene, Character, playerData.LastPosition);
    }
    // public IEnumerator ProgressSetStartGame(PlayerData playerData)
    // {
    //     if (Character == null)
    //     {
    //         Character = Instantiate(CharacterPrefab, playerData.LastPosition, Quaternion.identity).GetComponent<Character>();
    //         DontDestroyOnLoad(Character.gameObject);
    //     }
    //     else if (!Character.gameObject.activeSelf)
    //         Character.gameObject.SetActive(true);

    //     StartCoroutine(Character.SetCharacterWithPlayerData(playerData));
    //     Character.Rigidbody.useGravity = false;

    //     controller.gameObject.SetActive(true);
    //     Character.controller = controller;
    //     controller.Character = Character;

    //     yield return new WaitUntil(() => Character.isCharacterReady);

    //     LoadSceneManager.LoadScene(pathOfScenes + playerData.LastScene);
    //     while (!SceneManager.GetActiveScene().name.Equals(playerData.LastScene))
    //         yield return new WaitForFixedUpdate();

    //     Character.transform.position = playerData.LastPosition;
    //     Character.Rigidbody.useGravity = true;
    // }
}
