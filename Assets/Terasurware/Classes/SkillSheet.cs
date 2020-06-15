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
		public int Damage_Percentage;
		public int HitCount;
		public float During;
		public bool IsSingleTarget;
		public string StartFrom;
		public float EndFrom;
		public string AttackType;
		public bool IsDetectCollision;
		public string FXStartPoint;
		public int Cooldown;
		public int ParentIndex;
		public int SkillTier;
		public int BuffIndex;
		public int DebuffIndex;
	}
}

