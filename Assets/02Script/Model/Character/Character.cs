using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
using System.Linq;
using JetBrains.Annotations;

public partial class Character : Model
{
    public Controller controller { set; get; }
    public int level { private set; get; }
    public int SkillPoint;
    public bool IsinField { set; get; } = true;
    public enum AnimatorState { Idle, Running, Battle, GetHit, Attak, Dead }
    public AnimatorState NowAnimatorState { set; get; } = AnimatorState.Idle;
    bool IsOnEnableFuncPassed { set; get; } = false;
    public float MonsterLocatorDetectTime = 3f;
    public bool IsChacterReadyForUse { set; get; } = false;

    [SerializeField]
    bool isOnTerrian;
    float terrianCheckRadius { set; get; } = 0.1f;
    public PlayerData PlayerData { set; get; }
    //forDialougue
    public Dictionary<string, int> LastTimeTalkingWith { set; get; } = new Dictionary<string, int>();

    public List<QuestManager.QuestTable> ProcessingQuestList { set; get; } = new List<QuestManager.QuestTable>();
    public List<string> PassedTimeLineAssetName { set; get; } = new List<string>();

    public bool isCharacterReady { private set; get; } = false;
    new void Awake()
    {
        base.Awake();
        SetInfo("temp", 100, 100, 10, 10, 10);
        AwakeInUI();
    }

    new private void OnEnable()
    {
        base.OnEnable();
        SetStateView();
        IsOnEnableFuncPassed = true;
        StartCoroutine(DetectMonsterLocator());
    }

    new private void OnDisable()
    {
        base.OnDisable();
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
        FixedUpdateInAction();
        CheckOnterrian();
    }

    void CheckOnterrian()
    {
        var colliders = Physics.OverlapSphere(transform.position, terrianCheckRadius, 1 << LayerMask.NameToLayer("Terrain"));
        if (colliders.Length == 0)
        {
            transform.position += Physics.gravity * Time.fixedDeltaTime;
            isOnTerrian = false;
        }
        else
            isOnTerrian = true;
    }

    public void DoAnimator(AnimatorState action)
    {
        if (NowAnimatorState != action)
        {
            ResetAnimatorState();
            switch (action)
            {
                case AnimatorState.Idle: Animator.SetBool("IsIdle", true); break;
                case AnimatorState.Running: Animator.SetBool("IsRunning", true); break;
                case AnimatorState.Battle: Animator.SetBool("IsBattle", true); break;
                case AnimatorState.GetHit: Animator.SetBool("IsGetHit", true); break;
                case AnimatorState.Attak: Animator.SetBool("IsAttack", true); break;
            }
            NowAnimatorState = action;
        }
    }
    public void ResetAnimatorState()
    {
        Animator.SetBool("IsRunning", false);
        Animator.SetBool("IsIdle", false);
        Animator.SetBool("IsBattle", false);
        Animator.SetBool("IsGetHit", false);
        Animator.SetBool("IsAttack", false);
    }

    public void SetCharacterWithPlayerData(PlayerData data)
    {
        StartCoroutine(ProcessSetCharacterWithPlayerData(data));
    }
    public IEnumerator ProcessSetCharacterWithPlayerData(PlayerData playerData)
    {
        isCharacterReady = false;

        yield return new WaitWhile(() => IsOnEnableFuncPassed == false);
        yield return new WaitUntil(() => Inventory != null);
        yield return new WaitUntil(() => EquipmentView != null);
        yield return new WaitUntil(() => skillListHandler.StartPass);

        PlayerData = playerData;
        CharacterName = playerData.NickName;
        transform.position = playerData.LastPosition;
        level = playerData.level;

        {//for recycle 
            if (EquipmentView.IsWearing)
                for (int i = 0; i < EquipmentView.EquipmentItems.Length; i++)
                    if (EquipmentView.EquipmentItems[i] != null)
                        EquipmentView.ReleaseWearItem(i);

            Inventory.RemoveAllItem();

            foreach (SkillData skillData in skillListHandler.skillDatas)
                skillData.isLearn = false;

            LastTimeTalkingWith.Clear();

            ProcessingQuestList.Clear();

            PassedTimeLineAssetName.Clear();
        }

        for (int i = 0; i < playerData.itemKinds.Count; i++)
        {
            Inventory.AddItem
                (playerData.itemKinds[i],
                playerData.itemCounts[i]);
        }


        if (playerData.WearingItem.Count > 0)
        {
            foreach (int index in playerData.WearingItem)
            {
                var counter = new ItemManager.ItemCounter(ItemManager.Instance.GetitemData(index), 1);
                EquipmentView.SetEquipmetItem(ItemManager.Instance.GetNewItemView(counter));
            }
        }

        SkillPoint = level;
        for (int i = 0; i < skillListHandler.skillDatas.Count; i++)
        {
            var nowSkillData = skillListHandler.skillDatas[i];
            for (int j = 0; j < playerData.skillNames.Count; j++)
            {
                var nowSkillName = playerData.skillNames[j];
                var nowIsLearnd = playerData.isLearnSkill[j];
                if (nowSkillData.skillName_eng.Equals(nowSkillName))
                {
                    nowSkillData.isLearn = nowIsLearnd;
                    if (nowIsLearnd)
                        SkillPoint--;
                }
            }
        }

        if (playerData.namesTalkingwith.Count > 0) // dialogue 
        {
            for (int i = 0; i < playerData.namesTalkingwith.Count; i++)
            {
                LastTimeTalkingWith.Add(playerData.namesTalkingwith[i], playerData.lastTalkingIndex[i]);
            }
        }

        CharacterSkiilViewer.RefreshSkillPointText();
        CharacterSkiilViewer.SetSKillListHandler(skillListHandler);
        EquipmentView.LoadCharacterState();

        if (playerData.QuestIndexList.Count > 0) //processing QuestList
            ProcessingQuestList = QuestManager.LoadAllProgressQuestTable(playerData.QuestIndexList, this);

        if (playerData.TimeLineAssetName.Count > 0)
            foreach (string name in playerData.TimeLineAssetName)
                PassedTimeLineAssetName.Add(name);

        isCharacterReady = true;

        AddItem(5, 1);
        IntoNormalUI();
    }

    IEnumerator DetectMonsterLocator()
    {
        while (true)
        {
            Ray ray = new Ray();
            ray.origin = transform.position + Vector3.up * 0.5f;
            ray.direction = transform.forward;
            var layerMask = 1 << LayerMask.NameToLayer("MonsterLocator");
            var hits = Physics.RaycastAll(ray, 100f, layerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                var locator = hits[i].transform.GetComponent<MonseterLocator>();
                if (locator != null)
                    locator.TurnOnLocator(this);
            }
            yield return new WaitForSeconds(MonsterLocatorDetectTime);
        }
    }

    void OnDrawGizmos()
    {
        //MonsterLocaterDetector
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position + Vector3.up / 2, transform.position + Vector3.up / 2 + transform.forward * 100f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, terrianCheckRadius);
    }
}
