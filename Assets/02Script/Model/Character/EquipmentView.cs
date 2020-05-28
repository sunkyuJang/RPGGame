using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class EquipmentView : MonoBehaviour
{
    Character Character { set; get; }
    List<ItemManager.EquipmentItem> EquipmentItems { set; get; } = new List<ItemManager.EquipmentItem>();
    RectTransform Transform { set; get; }
    Transform WeaponTrans { set; get; }
    Transform ArmorTrans { set; get; }
    Image WeaponImage { set; get; }
    Image ArmorImage { set; get; }

    public static Rect WeaponArea { private set; get; }
    public static Rect ArmorArea { private set; get; }
    Text CharacterName { set; get; }
    Text HP { set; get; }
    Text MP { set; get; }
    Text ATK{ set; get; }
    Text DEF{ set; get; }
    Text SPD { set; get; }
    public void HideObject() { gameObject.SetActive(false); Character.IntoNomalUI(); }

    public IEnumerator OnTouched(int index)
    {
        print(index);
        if (index >= 0)
        {
            ComfimBox comfimBox = StaticManager.GetComfimBox;
            comfimBox.ShowComfirmBox("장착된 아이템을 해제 하시겠습니까?");
            while (comfimBox.NowState == ComfimBox.State.Waiting) { yield return new WaitForFixedUpdate(); }
            if (comfimBox.NowState == ComfimBox.State.Yes)
            {
                List<string> increase = new List<string>();
                ItemManager.EquipmentItem equipmentItem = EquipmentItems[index];
                foreach (string n in equipmentItem.GetIncrease)
                {
                    string newIncrease = "";
                    switch (n[0])
                    {
                        case '-': newIncrease += "+"; break;
                        case '+': newIncrease += "-"; break;
                        case '/': newIncrease += "*"; break;
                        case '*': newIncrease += "/"; break;
                    }
                    newIncrease += n.Substring(1);
                    increase.Add(newIncrease);
                }
                StartCoroutine(Character.IncreaseEffect(equipmentItem.Effects, increase, null));
                Character.Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.EquipmentItemList, equipmentItem.Index, 1));
                EquipmentItems.RemoveAt(index);
                if(equipmentItem.Type == "Weapon") { WeaponImage.sprite = null; Destroy(WeaponTrans.GetChild(0).gameObject); }
                else { ArmorImage.sprite = null; Destroy(ArmorTrans.gameObject); }
                LoadCharacterState();
            }
        }
    }

    public void TouchedInWeapon() { StartCoroutine(OnTouched(FindIndex("Weapon"))); }
    public void TouchedInArmor() { StartCoroutine(OnTouched(FindIndex("Armor"))); }
    int FindIndex(string type)
    {
        for(int i = 0; i < EquipmentItems.Count; i++)
        {
            if(EquipmentItems[i].Type == type) { return i; }
        }
        return -1;
    }

    public void SetEquipmetItem(ItemManager.EquipmentItem equipmentItem)
    {
        if(equipmentItem.Type == "Weapon")
        {
            WeaponImage.sprite = equipmentItem.Image;
            Instantiate(Resources.Load<GameObject>("Character/Weapon/Dagger"), WeaponTrans);
        }
        else
        {
            ArmorImage.sprite = equipmentItem.Image;
            ArmorTrans = Instantiate(Resources.Load<GameObject>("Character/Armor/WarriorMale"), Character.transform).transform;
        }
        EquipmentItems.Add(equipmentItem);
    }
    public static EquipmentView GetNew(Character character)
    {
        EquipmentView NewEquipmentView = Create.GetNewInCanvas<EquipmentView>();
        NewEquipmentView.SetConstructor(character);
        NewEquipmentView.LoadCharacterState();
        return NewEquipmentView;
    }
    void SetConstructor(Character character)
    {
        Character = character;

        WeaponTrans = Character.Transform.Find("CharArmature/Root/Pelvis/Spine/Chest/Shoulder.L/UpperArm.L/LowerArm.L/Hand.L/HoldWeapon");

        Transform = gameObject.GetComponent<RectTransform>();
        Transform WeaponTransform = Transform.Find("Weapon");
        WeaponImage = WeaponTransform.GetChild(1).GetComponent<Image>();
        WeaponArea = GMath.GetRect(WeaponTransform.GetComponent<RectTransform>());

        Transform ArmorTransform = Transform.Find("Armor");
        ArmorImage = ArmorTransform.GetChild(1).GetComponent<Image>();
        ArmorArea = GMath.GetRect(ArmorTransform.GetComponent<RectTransform>());

        Transform ChildTransform = Transform.Find("CharacterStat");
        CharacterName = ChildTransform.GetChild(0).GetChild(0).GetComponent<Text>();
        HP = ChildTransform.GetChild(1).GetChild(0).GetComponent<Text>();
        MP = ChildTransform.GetChild(2).GetChild(0).GetComponent<Text>();
        ATK = ChildTransform.GetChild(3).GetChild(0).GetComponent<Text>();
        DEF = ChildTransform.GetChild(4).GetChild(0).GetComponent<Text>();
        SPD = ChildTransform.GetChild(5).GetChild(0).GetComponent<Text>();

        gameObject.SetActive(false);
    }
    public void LoadCharacterState()
    {
        this.CharacterName.text = Character.CharacterName;
        this.HP.text = Character.HP.ToString();
        this.MP.text = Character.MP.ToString();
        this.ATK.text = Character.ATK.ToString();
        this.DEF.text = Character.DEF.ToString();
        this.SPD.text = Character.SPD.ToString();
    }
}
