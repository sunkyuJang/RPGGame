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
		public int ParentIndex;
		public string InfluencedBy;
		public int Damage_Percentage;
		public int HitCount;
		public float Duration;
		public bool IsSingleTarget;
		public string TargetTo;
		public string StartFrom;
		public float Length;
		public string AttackType;
		public bool IsDetectCollision;
		public string FXStartPoint;
		public int Cooldown;
		public int SkillTier;
		public int StateEffecterIndex;
	}
}

