using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class ItemSheet_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/02Script/Manager/ItemManager/Sheet/ItemSheet.xlsx";
	private static readonly string exportPath = "Assets/02Script/Manager/ItemManager/Sheet/ItemSheet.asset";
	private static readonly string[] sheetNames = { "Sheet1", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			ItemSheet data = (ItemSheet)AssetDatabase.LoadAssetAtPath (exportPath, typeof(ItemSheet));
			if (data == null) {
				data = ScriptableObject.CreateInstance<ItemSheet> ();
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

					ItemSheet.Sheet s = new ItemSheet.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						ItemSheet.Param p = new ItemSheet.Param ();
						
					cell = row.GetCell(0); p.Index = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.Name_eng = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.ItemType = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.Description = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.Limit = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.Buy = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.Sell = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.EffecterIndex = (int)(cell == null ? 0 : cell.NumericCellValue);
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
