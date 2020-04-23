using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DialogueSheet : ScriptableObject
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
		public double Index;
		public bool Stop;
		public string NextStep;
		public string Script;
	}
}

