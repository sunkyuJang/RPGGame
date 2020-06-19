using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using System.Security.Permissions;

public class ItemSheet : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		public enum ItemTypeEnum { Active, Key, Equipment }
		public ItemTypeEnum GetItemType 
		{ 
			get 
			{
                switch (ItemType)
                {
					case "Active": return ItemTypeEnum.Active;
					case "Key": return ItemTypeEnum.Key;
                }
				return ItemTypeEnum.Equipment;
			} 
		}
		public int Index;
		public string Name;
		public string Name_eng;
		public string ItemType;
		public string Description;
		public int Limit;
		public int Buy;
		public int Sell;
		public int EffecterIndex;
	}
}

