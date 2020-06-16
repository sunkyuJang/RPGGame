using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class StateEffecterSheet_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/02Script/Model/StateEffecterSheet.xlsx";
	private static readonly string exportPath = "Assets/02Script/Model/StateEffecterSheet.asset";
	private static readonly string[] sheetNames = { "Sheet1", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			StateEffecterSheet data = (StateEffecterSheet)AssetDatabase.LoadAssetAtPath (exportPath, typeof(StateEffecterSheet));
			if (data == null) {
				data = ScriptableObject.CreateInstance<StateEffecterSheet> ();
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

					StateEffecterSheet.Sheet s = new StateEffecterSheet.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						StateEffecterSheet.Param p = new StateEffecterSheet.Param ();
						
					cell = row.GetCell(0); p.Index = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.RequestType = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.RequestIndex = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.nowHp = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.nowMp = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.HP = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.MP = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.ATK_point = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.DEF_point = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.SPD_point = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.ATK_percent = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(11); p.DEF_percent = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(12); p.SPD_percent = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(13); p.Super = (cell == null ? false : cell.BooleanCellValue);
					cell = row.GetCell(14); p.During = (cell == null ? 0.0 : cell.NumericCellValue);
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
