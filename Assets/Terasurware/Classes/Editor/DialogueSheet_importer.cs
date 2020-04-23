using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class DialogueSheet_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/02Script/Manager/DialogueManager/DialogueSheet.xlsx";
	private static readonly string exportPath = "Assets/02Script/Manager/DialogueManager/DialogueSheet.asset";
	private static readonly string[] sheetNames = { "Sheet1", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			DialogueSheet data = (DialogueSheet)AssetDatabase.LoadAssetAtPath (exportPath, typeof(DialogueSheet));
			if (data == null) {
				data = ScriptableObject.CreateInstance<DialogueSheet> ();
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

					DialogueSheet.Sheet s = new DialogueSheet.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						DialogueSheet.Param p = new DialogueSheet.Param ();
						
					cell = row.GetCell(0); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.Index = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.Stop = (cell == null ? false : cell.BooleanCellValue);
					cell = row.GetCell(3); p.NextStep = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.Script = (cell == null ? "" : cell.StringCellValue);
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
