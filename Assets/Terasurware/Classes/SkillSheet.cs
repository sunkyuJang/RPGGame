using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillSheet : ScriptableObject
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
		
		public int Index;
		public string Name;
		public string Name_Eng;
		public string Description;
		public bool CanMove;
		public string InfluencedBy;
		public float Damage_Percentage;
		public int HitCount;
		public float During;
		public float ActivateTime_sec;
		public bool IsSingleTarget;
		public string StartFrom;
		public string StartRange_Rad;
		public float EndFrom;
		public string EndRad;
		public string AttackType;
		public int Cooldown;
		public int ParentIndex;
		public int SkillTier;
		public int BuffIndex;
		public int DebuffIndex;
	}
}

