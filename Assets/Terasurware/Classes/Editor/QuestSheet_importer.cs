using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class QuestSheet_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/02Script/Manager/QuestManager/QuestSheet.xlsx";
	private static readonly string exportPath = "Assets/02Script/Manager/QuestManager/QuestSheet.asset";
	private static readonly string[] sheetNames = { "Sheet1", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			QuestSheet data = (QuestSheet)AssetDatabase.LoadAssetAtPath (exportPath, typeof(QuestSheet));
			if (data == null) {
				data = ScriptableObject.CreateInstance<QuestSheet> ();
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

					QuestSheet.Sheet s = new QuestSheet.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						QuestSheet.Param p = new QuestSheet.Param ();
						
					cell = row.GetCell(0); p.Name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.Index = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.DialogueIndex = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.NeedItem = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.NeedCount = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.RewardItem = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(6); p.RewardCount = (cell == null ? "" : cell.StringCellValue);
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
