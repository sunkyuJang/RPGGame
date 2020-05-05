using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : Model
{
    public class UsingItem
    {
        public ItemManager.ItemIndexer itemIndex { set; get; }
        public Coroutine coroutine { set; get; }
        public IEnumerator process { set; get; }
        private int originalATK { set; get; }
        private int originalSPD { set; get; }
        private bool originalSuper { set; get; }
        private float during { set; get; }
        public int GetATK { get { return originalATK; } }
        public int GetSPD { get { return originalSPD; } }
        public bool GetIsSuper { get { return originalSuper; } }
        public float GetDuring { get { return during; } }
        public float nowDuring { set; get; }
        public UsingItem(ItemManager.ItemIndexer _itemIndex, int _ATK, int _SPD, bool _isSuper, float _during) { 
            itemIndex = _itemIndex; 
            originalATK = _ATK; 
            originalSPD = _SPD; 
            originalSuper = _isSuper; 
            during = _during; 
            nowDuring = during;
        }
    }
    List<UsingItem> usingItems = new List<UsingItem>();
    private UsingItem FindUsingItem(ItemManager.ItemIndexer _itemIndex) { foreach (UsingItem usingItem in usingItems) { if (usingItem.itemIndex.Equals(_itemIndex)) return usingItem; } return null;  }
    public void UseItem(ItemManager.ItemIndexer itemIndexer)
    {
        UsingItem alreadyUse = FindUsingItem(itemIndexer);
        if (alreadyUse != null)
        {
            if (itemIndexer.Kinds == ItemManager.Kinds.activeItemList)
            { 
                alreadyUse.nowDuring = alreadyUse.GetDuring; 
            }
        }
        else
        {
            ItemManager.UsefulItemData usefulItem = ItemManager.GetItem(itemIndexer) as ItemManager.UsefulItemData;
            alreadyUse = new UsingItem(itemIndexer, ATK, SPD, isSuper, usefulItem.GetDuring);
            alreadyUse.process = IncreaseEffect(usefulItem.GetEffects, usefulItem.GetIncrease, alreadyUse);
            alreadyUse.coroutine = StartCoroutine(alreadyUse.process);
            usingItems.Add(alreadyUse);
            if (usefulItem is ItemManager.EquipmentItem) 
            {
                EquipmentView.SetEquipmetItem(usefulItem as ItemManager.EquipmentItem);
                EquipmentView.LoadCharacterState();
            }
        }
    }
    public IEnumerator IncreaseEffect(List<ItemManager.EffectName> effectName, List<string> _increase, UsingItem _thisUsingItem)
    {
        for (int i = 0; i < effectName.Count; i++)
        {
            switch (effectName[i])
            {
                case ItemManager.EffectName.HP: nowHP = DecodeSymbol(nowHP, _increase[i]); nowHP = nowHP <= HP ? nowHP : HP; break;
                case ItemManager.EffectName.MP: nowMP = DecodeSymbol(nowMP, _increase[i]); nowMP = nowMP <= MP ? nowMP : MP; break;
                case ItemManager.EffectName.OriginHP: HP = DecodeSymbol(HP, _increase[i]); break;
                case ItemManager.EffectName.OriginMP: MP = DecodeSymbol(MP, _increase[i]); break;
                case ItemManager.EffectName.Inventory: Inventory.length = DecodeSymbol(Inventory.length, _increase[i]); ; break;
                case ItemManager.EffectName.QuickSlot: QuickSlot.IsActive = true; QuickSlot.TurnOn(true); break;
                case ItemManager.EffectName.ATK: ATK = DecodeSymbol(ATK, _increase[i]); break;
                case ItemManager.EffectName.DEF: DEF = DecodeSymbol(DEF, _increase[i]); break;
                case ItemManager.EffectName.SPD: SPD = DecodeSymbol(SPD, _increase[i]); break;
                case ItemManager.EffectName.Super: isSuper = true; break;
            }
        }

        if (_thisUsingItem != null)
        {
            while (_thisUsingItem.nowDuring > 0f)
            {
                yield return new WaitForSeconds(1f);
                _thisUsingItem.nowDuring--;
            }
            if (_thisUsingItem.GetDuring != 0f) { EndDuringState(_thisUsingItem); }
            yield return new WaitForFixedUpdate();
            usingItems.Remove(_thisUsingItem);
        }
    }
    private void EndDuringState(UsingItem _usingItem)
    {
        ATK = _usingItem.GetATK;
        SPD = _usingItem.GetSPD;
        isSuper = _usingItem.GetIsSuper;
    }

    private int DecodeSymbol(int _target, string _increase)
    {
        if (_increase.Length > 6 && _increase.Substring(1, 6).Equals("Origin"))
        {
            int originalData = _increase.Substring(1, 8).Equals("originalHP") ? HP : MP;
            
            if(_increase.Length > 8) 
                return DecodeSymbol(_target, _increase.Substring(0,1) + DecodeSymbol(originalData, _increase.Substring(9)).ToString());
            else 
                return originalData; 
        }
        
        switch (_increase[0])
        {
            case '+': _target += int.Parse(_increase.Substring(1)); break;
            case '-': _target -= int.Parse(_increase.Substring(1)); break;
            case '*': _target *= int.Parse(_increase.Substring(1)); break;
            case '/': _target /= int.Parse(_increase.Substring(1)); break;
        }  
        return _target;
    }
}
