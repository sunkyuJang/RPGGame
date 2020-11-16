using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateEffecterSheet : ScriptableObject
{
    public List<Sheet> sheets = new List<Sheet>();

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
        public string RequestType;
        public int RequestIndex;
        public double nowHp;
        public double nowMp;
        public int HP;
        public int MP;
        public int ATK_point;
        public int DEF_point;
        public int SPD_point;
        public double ATK_percent;
        public double DEF_percent;
        public double SPD_percent;
        public bool Super;
        public double During;
    }
}

