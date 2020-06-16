using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity_Sheet1 : ScriptableObject
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
		
		public double Index;
		public string RequestType;
		public double RequestIndex;
		public double nowHp;
		public double nowMp;
		public double HP;
		public double MP;
		public double ATK_point;
		public double DEF_point;
		public double SPD_point;
		public double ATK_percent;
		public double DEF_percent;
		public double SPD_percent;
		public bool Super;
		public double During;
	}
}

