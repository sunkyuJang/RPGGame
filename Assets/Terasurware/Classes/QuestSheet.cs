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
		
		public string name;
		public int index;
		public string needItem;
		public string needCount;
		public string rewardItem;
		public string rewardCount;
	}
}

