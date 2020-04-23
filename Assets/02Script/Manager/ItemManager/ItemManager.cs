using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public partial class ItemManager : MonoBehaviour
{
    public enum Kinds { Null, activeItemList, EquipmentItemList, keyItemList }
    public class Item
    {
        public string Name { private set; get; }
        public string Description { private set; get; }
        public int Limit { private set; get; }
        public Sprite Image { private set; get; }
        public int Index { private set; get; }
        public Item(string url, double imagetIndex, string name, string description, int limit, int index)
        {
            Name = name;
            Description = description;
            Limit = limit;
            Image = Resources.Load<Sprite>("image/" + url + "/" + imagetIndex.ToString());
            Index = index;
        }
    }
    public class ItemIndexer
    {
        public Kinds Kinds { private set; get; }
        public int ItemIndex { private set; get; }
        public ItemIndexer(Kinds _kinds, int _itemIndex)
        {
            Kinds = _kinds;
            ItemIndex = _itemIndex;
        }
        public ItemIndexer(ItemIndexer indexer)
        {
            Kinds = indexer.Kinds;
            ItemIndex = indexer.ItemIndex;
        }
        public bool IsSame(ItemIndexer _target) { return (Kinds == _target.Kinds) && (ItemIndex == _target.ItemIndex); }
    }
    public class ItemCounter : ItemIndexer
    {
        public ItemIndexer Indexer { get { return this as ItemIndexer; } }
        public int Count { set; get; }
        public ItemCounter(ItemIndexer indexer, int _count) : base(indexer) => Count = _count;
        public ItemCounter(Kinds kinds, int index, int count) : base(kinds, index) => Count = count;
    }
    public static Item GetItem (ItemIndexer _indexer)
    {
        switch (_indexer.Kinds)
        {
            case Kinds.activeItemList : return ActiveItems[_indexer.ItemIndex];
            case Kinds.keyItemList : return KeyItems[_indexer.ItemIndex];
            case Kinds.EquipmentItemList: return Equipments[_indexer.ItemIndex];
        }
        return null;
    }
    private void Awake()
    {
        SetActiveList();
        SetEquipment();
        SetKeyItems();
    }
}
