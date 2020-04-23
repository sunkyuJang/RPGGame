using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class Item_ActiveSheet_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/02Script/Manager/ItemManager/Sheet/Item_ActiveSheet.xlsx";
	private static readonly string exportPath = "Assets/02Script/Manager/ItemManager/Sheet/Item_ActiveSheet.asset";
	private static readonly string[] sheetNames = { "ActiveItemSheet", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			ActiveItemSheet data = (ActiveItemSheet)AssetDatabase.LoadAssetAtPath (exportPath, typeof(ActiveItemSheet));
			if (data == null) {
				data = ScriptableObject.CreateInstance<ActiveItemSheet> ();
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

					ActiveItemSheet.Sheet s = new ActiveItemSheet.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						ActiveItemSheet.Param p = new ActiveItemSheet.Param ();
						
					cell = row.GetCell(0); p.ImageIndex = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Title = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.Description = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.Limit = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Buy = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.Sell = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.Effects = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(7); p.Increase = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(8); p.During = (float)(cell == null ? 0 : cell.NumericCellValue);
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
