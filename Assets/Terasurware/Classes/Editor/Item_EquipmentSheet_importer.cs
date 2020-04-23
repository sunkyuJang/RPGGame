using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class Item_EquipmentSheet_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/02Script/Manager/ItemManager/Sheet/Item_EquipmentSheet.xlsx";
	private static readonly string exportPath = "Assets/02Script/Manager/ItemManager/Sheet/Item_EquipmentSheet.asset";
	private static readonly string[] sheetNames = { "EquipmentItem", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			EquipmentItemSheet data = (EquipmentItemSheet)AssetDatabase.LoadAssetAtPath (exportPath, typeof(EquipmentItemSheet));
			if (data == null) {
				data = ScriptableObject.CreateInstance<EquipmentItemSheet> ();
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

					EquipmentItemSheet.Sheet s = new EquipmentItemSheet.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						EquipmentItemSheet.Param p = new EquipmentItemSheet.Param ();
						
					cell = row.GetCell(0); p.ImageIndex = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.Title = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.Description = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.Limit = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.Buy = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.Sell = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.Effects = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(7); p.Increase = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(8); p.Type = (cell == null ? "" : cell.StringCellValue);
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
