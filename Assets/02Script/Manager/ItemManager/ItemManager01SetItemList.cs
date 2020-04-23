using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public partial class ItemManager
{
    public ActiveItemSheet activeItemSheet;
    public EquipmentItemSheet equipmentItemSheet;
    public KeyItemSheet keyItemSheet;
    public static List<ActiveItem> ActiveItems { private set; get; } = new List<ActiveItem>();
    public static List<EquipmentItem> Equipments { private set; get; } = new List<EquipmentItem>();
    public static List<KeyItem> KeyItems { private set; get; } = new List<KeyItem>();
    public enum EffectName { HP, MP, OriginHP, OriginMP, ATK, DEF, SPD, Super, Inventory, QuickSlot }
    void SetKeyItems()
    {
        List<KeyItemSheet.Param> keyList = keyItemSheet.sheets[0].list;
        for (int i = 0; i < keyList.Count; i++)
        {
            KeyItemSheet.Param item = keyList[i];
            KeyItems.Add(new KeyItem(item.ImageIndex, item.Title, item.Description, item.Limit, KeyItems.Count));
        }
    }
    void SetActiveList()
    {
        List<ActiveItemSheet.Param> activeList = activeItemSheet.sheets[0].list;
        for (int i = 0; i < activeList.Count; i++)
        {
            ActiveItemSheet.Param item = activeList[i];
            string[] effectList = item.Effects.Split(';');
            string[] increaseList = item.Increase.Split(';');
            List<EffectName> effects = new List<EffectName>();
            List<string> increases = new List<string>();
            GetEffectsIncreaseList(effectList, increaseList, ref effects, ref increases);
            ActiveItems.Add(new ActiveItem(item.ImageIndex, item.Title, item.Description, item.Limit, item.Buy, item.Sell, effects, increases, item.During, ActiveItems.Count)); ;
        }
    }
    void SetEquipment()
    {
        List<EquipmentItemSheet.Param> equipmentList = equipmentItemSheet.sheets[0].list;
        for (int i = 0; i < equipmentList.Count; i++)
        {
            EquipmentItemSheet.Param item = equipmentList[i];
            string[] effectList = item.Effects.Split(';');
            string[] increaseList = item.Increase.Split(';');
            List<EffectName> effects = new List<EffectName>();
            List<string> increases = new List<string>();
            GetEffectsIncreaseList(effectList, increaseList, ref effects, ref increases);
            Equipments.Add(new EquipmentItem(item.ImageIndex, item.Title, item.Description, item.Limit, item.Buy, item.Sell, effects, increases, item.Type, Equipments.Count)); ;
        }
    }

    private void GetEffectsIncreaseList(string[] effectList, string[] increaseList, ref List<EffectName> effects, ref List<string> increases)
    {
        for (int j = 0; j < effectList.Length -1; j++)
        {
            switch (effectList[j])
            {
                case "HP": effects.Add(EffectName.HP); break;
                case "MP": effects.Add(EffectName.MP); break;
                case "OriginHP": effects.Add(EffectName.OriginHP); break;
                case "OriginMP": effects.Add(EffectName.OriginMP); break;
                case "ATK": effects.Add(EffectName.ATK); break;
                case "DEF": effects.Add(EffectName.DEF); break;
                case "SPD": effects.Add(EffectName.SPD); break;
                case "Super": effects.Add(EffectName.Super); break;
                case "Inventory": effects.Add(EffectName.Inventory); break;
                case "QuickSlot": effects.Add(EffectName.QuickSlot); break;
            }
            if (j < increaseList.Length) increases.Add(increaseList[j]);
        }
    }

    public class ActiveItem : Item, UsefulItemData
    {
        public List<EffectName> Effects { private set; get; }
        public List<EffectName> GetEffects { get { return Effects; } }
        public List<string> Increase { private set; get; }
        public List<string> GetIncrease { get { return Increase; } }
        public float During { private set; get; }
        public float GetDuring { get { return During; } }
        public int Buy { private set; get; }
        public int Sell { private set; get; }

        public ActiveItem(double imageIndex, string title, string descripstion, int limit,
            int buy, int sell, List<EffectName> _effects, List<string> _increase, float during, int index)
            : base("ActiveItem", imageIndex, title, descripstion, limit, index)
        {
            Effects = _effects;
            Increase = _increase;
            During = during;
            Buy = buy;
            Sell = sell;
        }
    }
    public class EquipmentItem : Item, UsefulItemData
    {
        public List<EffectName> Effects { private set; get; }
        public List<EffectName> GetEffects { get { return Effects; } }
        public List<string> Increase { private set; get; }
        public List<string> GetIncrease { get { return Increase; } }
        public float GetDuring { get { return 0f; } }
        public int Buy { private set; get; }
        public int Sell { private set; get; }
        public string Type { private set; get; }
        public EquipmentItem(double imageIndex, string title, string descripstion, int limit,
            int buy, int sell, List<EffectName> _effects, List<string> _increase, string type, int index)
            : base("EquipmentItem", imageIndex, title, descripstion, limit, index)
        {
            Effects = _effects;
            Increase = _increase;
            Buy = buy;
            Sell = sell;
            Type = type;
        }
    }
    public class KeyItem : Item
    {
        public KeyItem(double imageIndex, string title, string descripstion, int limit, int index) : base("KeyItem", imageIndex, title, descripstion, limit, index) { }
    }
    public interface UsefulItemData
    {
        List<EffectName> GetEffects { get; }
        List<string> GetIncrease { get; }
        float GetDuring { get; }
    }
}
