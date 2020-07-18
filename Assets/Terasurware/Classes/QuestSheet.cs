using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestSheet : ScriptableObject
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
		
		public string Name;
		public int Index;
		public int DialogueIndex;
		public string NeedItem;
		public string NeedCount;
		public string RewardItem;
		public string RewardCount;
	}
}

