using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class SkillSheet_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/02Script/Manager/SkillManager/SkillSheet.xlsx";
	private static readonly string exportPath = "Assets/02Script/Manager/SkillManager/SkillSheet.asset";
	private static readonly string[] sheetNames = { "시트1", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			SkillSheet data = (SkillSheet)AssetDatabase.LoadAssetAtPath (exportPath, typeof(SkillSheet));
			if (data == null) {
				data = ScriptableObject.CreateInstance<SkillSheet> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					SkillSheet.Sheet s = new SkillSheet.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						SkillSheet.Param p = new SkillSheet.Param ();
						
					cell = row.GetCell(0); p.Index = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.Name_Eng = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.Description = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.CanMove = (cell == null ? false : cell.BooleanCellValue);
					cell = row.GetCell(5); p.InfluencedBy = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(6); p.Damage_Percentage = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.HitCount = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.During = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.ActivateTime_sec = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.IsSingleTarget = (cell == null ? false : cell.BooleanCellValue);
					cell = row.GetCell(11); p.StartFrom = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(12); p.StartRange_Rad = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(13); p.EndFrom = (float)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(14); p.EndRad = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(15); p.AttackType = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(16); p.Cooldown = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(17); p.ParentIndex = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(18); p.SkillTier = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(19); p.BuffIndex = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(20); p.DebuffIndex = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
