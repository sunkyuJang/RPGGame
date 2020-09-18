using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class EquipmentView : MonoBehaviour
{
    Character Character { set; get; }
    ItemManager.ItemCounter[] EquipmentItems { set; get; } = new ItemManager.ItemCounter[2];
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
    public void HideObject() { gameObject.SetActive(false); Character.ShowGameUI(true); }

    public IEnumerator OnTouched(int index)
    {
        print(index);
        if (index >= 0)
        {
            var confirmBox = ConfimBoxManager.instance;
            confirmBox.ShowComfirmBox("장착된 아이템을 해제 하시겠습니까?");
            while (confirmBox.NowState == ConfimBoxManager.State.Waiting) { yield return new WaitForFixedUpdate(); }
            if (confirmBox.NowState == ConfimBoxManager.State.Yes)
            {
                var equipmentItem = EquipmentItems[index];
                var View = ItemManager.GetNewItemView(equipmentItem, Character.Inventory);
                StateEffecterManager.EffectToModelByItem(Character as Model, View.ItemCounter, true);
                Character.Inventory.AddItem(equipmentItem);
                Destroy(View.gameObject);
                EquipmentItems[index] = null;
                if(index == 0) { WeaponImage.sprite = null; Destroy(WeaponTrans.GetChild(0).gameObject); }
                else { ArmorImage.sprite = null; Destroy(ArmorTrans.gameObject); }
                LoadCharacterState();
            }
        }
    }

    public void TouchedInWeapon() { StartCoroutine(OnTouched(0)); }
    public void TouchedInArmor() { StartCoroutine(OnTouched(1)); }

    public void SetEquipmetItem(ItemView equipmentItem)
    {
        ItemSheet.Param data = equipmentItem.ItemCounter.Data;
        if (data.ItemType == "EquipmentWeapon")
        {
            WeaponImage.sprite = equipmentItem.Icon.sprite;
            Instantiate(Resources.Load<GameObject>("Model/Character/Equipment/Dagger/Dagger"), WeaponTrans);
            EquipmentItems[0] = equipmentItem.ItemCounter.CopyThis();
        }
        else
        {
            ArmorImage.sprite = equipmentItem.Icon.sprite;
            ArmorTrans = Instantiate(Resources.Load<GameObject>("Model/Character/Equipment/Leather/WarriorMale"), Character.transform).transform;
            EquipmentItems[1] = equipmentItem.ItemCounter.CopyThis();
        }
        LoadCharacterState();
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

        WeaponTrans = Character.Transform.Find("CharArmature/Root/Pelvis/Spine/Chest/Shoulder.R/UpperArm.R/LowerArm.R/Hand.R/HoldWeapon");

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
