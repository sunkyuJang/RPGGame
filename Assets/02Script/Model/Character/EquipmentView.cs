using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
using TMPro;

public class EquipmentView : MonoBehaviour
{
    public Character Character { set; get; }
    public ItemManager.ItemCounter[] EquipmentItems { private set; get; } = new ItemManager.ItemCounter[2];
    public bool IsWearing { get { foreach (ItemManager.ItemCounter itemCounter in EquipmentItems) if (itemCounter == null) return false; return true; } }
    RectTransform Transform { set; get; }
    Transform WeaponTrans { set; get; }
    Transform ArmorTrans { set; get; }
    public Image WeaponImage;
    public Image ArmorImage;

    public Transform InfoTransform;
    Text CharacterName { set; get; }
    Text HP { set; get; }
    Text MP { set; get; }
    Text ATK{ set; get; }
    Text DEF{ set; get; }
    Text SPD { set; get; }

    public void HideObject() 
    { 
        Character.IntoNormalUI(); 
        gameObject.SetActive(false); 
    }

    public void Awake()
    {
        Transform = gameObject.GetComponent<RectTransform>();

        CharacterName = InfoTransform.GetChild(0).GetChild(0).GetComponent<Text>();
        HP = InfoTransform.GetChild(1).GetChild(0).GetComponent<Text>();
        MP = InfoTransform.GetChild(2).GetChild(0).GetComponent<Text>();
        ATK = InfoTransform.GetChild(3).GetChild(0).GetComponent<Text>();
        DEF = InfoTransform.GetChild(4).GetChild(0).GetComponent<Text>();
        SPD = InfoTransform.GetChild(5).GetChild(0).GetComponent<Text>();

        gameObject.SetActive(false);
    }
    public IEnumerator OnTouched(int index)
    {
        if (index >= 0)
        {
            var confirmBox = ConfimBoxManager.instance;
            confirmBox.ShowComfirmBox("장착된 아이템을 해제 하시겠습니까?");
            while (confirmBox.NowState == ConfimBoxManager.State.Waiting) { yield return new WaitForFixedUpdate(); }
            if (confirmBox.NowState == ConfimBoxManager.State.Yes)
            {
                var equipmentItem = EquipmentItems[index];
                //var View = Character.Inventory.itemViewPooler.GetObj<ItemView>().SetItemCounter(equipmentItem, Character.Inventory);
                StateEffecterManager.EffectToModelByItem(equipmentItem, Character, true);
                Character.Inventory.AddItem(equipmentItem);
                ItemManager.Instance.ReturnItemView(EquipmentItems[index].View);
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
            WeaponImage.sprite = equipmentItem.icon.sprite;
            Instantiate(Resources.Load<GameObject>("Model/Character/Equipment/Dagger/Dagger"), WeaponTrans);
            EquipmentItems[0] = equipmentItem.ItemCounter.CopyThis();
        }
        else
        {
            ArmorImage.sprite = equipmentItem.icon.sprite;
            ArmorTrans = Instantiate(Resources.Load<GameObject>("Model/Character/Equipment/Leather/WarriorMale"), Character.transform).transform;
            EquipmentItems[1] = equipmentItem.ItemCounter.CopyThis();
        }
        LoadCharacterState();
    }


    public void SetCharacter(Character character)
    {
        Character = character;

        WeaponTrans = Character.Transform.Find("CharArmature/Root/Pelvis/Spine/Chest/Shoulder.R/UpperArm.R/LowerArm.R/Hand.R/HoldWeapon");
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
