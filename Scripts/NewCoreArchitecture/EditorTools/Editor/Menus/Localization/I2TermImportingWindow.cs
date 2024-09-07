using I2.Loc;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using RTLTMPro;
using System.Linq;

namespace Match3.EditorTools.Editor.Menus.Localization
{
	// TODO: Move some other utilities except Transltation Importing to I2LanguageSourceAssetUtility
	public partial class I2TermImportingWindow : EditorWindow
    {

		const string KeyColumnName = "Key";
		const string TypeColumnName = "Type";
		const string DesColumnName = "Des";
		const string PersianColumnName = "Persian";



		[MenuItem("Golmorad/Localization/I2 Term Importing Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(I2TermImportingWindow));
        }

		Dictionary<string, int> columnTypeToIndexMap = new Dictionary<string, int>();

		LanguageSourceAsset languageSource;

        string filePath;

		string prefixToImport = "";
		string prefixToCheckForNoTermError = "";
		bool hideErrorForNoTermError;

		bool showAditionalSettings = false;

        void OnGUI()
        {
            languageSource = (LanguageSourceAsset)EditorGUILayout.ObjectField("Source", languageSource, typeof(LanguageSourceAsset), allowSceneObjects: false);


			showAditionalSettings = EditorGUILayout.Foldout(showAditionalSettings, "Additional Settings");

			if (showAditionalSettings)
			{

				prefixToImport = EditorGUILayout.TextField("Prefix to import", prefixToImport);
				hideErrorForNoTermError = EditorGUILayout.Toggle("Hide <<new term error>>", hideErrorForNoTermError);
				if (hideErrorForNoTermError == false)
					prefixToCheckForNoTermError = EditorGUILayout.TextField("Prefix to check <<new term error>>", prefixToCheckForNoTermError);
			}

			if (GUILayout.Button("Select Tranlation File"))
                filePath = EditorUtility.OpenFilePanel("Select Translation File", "", "csv,txt");

            if (GUILayout.Button("Import"))
                Import(filePath, languageSource);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            var style = GUI.skin.GetStyle("Label");
            style.alignment = TextAnchor.UpperCenter;
        }

        void Import(string fileName, LanguageSourceAsset languageSource)
        {
			Debug.Log("----------------------- Importing Started ---------------------");
			var encoding = System.Text.Encoding.UTF8;
            string fileText = LocalizationReader.ReadCSVfile(fileName, encoding);
			List<string[]> rows = LocalizationReader.ReadCSV(fileText, DetermineFileSeparator(fileText));
			ImportRows(rows, languageSource);

			Debug.Log("----------------------- Importing Finished ---------------------");
		}

		private char DetermineFileSeparator(string text)
        {
			var firstLine = text.Substring(0, text.IndexOf("\n"));

			if (firstLine.Contains(','))
				return ',';
			else if (firstLine.Contains(';'))
				return ';';
			else if (firstLine.Contains('\t'))
				return '\t';

			throw new Exception("Can't determine the files seperator");
		}


		public void ImportRows(List<string[]> cvsRows, LanguageSourceAsset sourceAsset)
		{
			SetupColumnMap(cvsRows, sourceAsset.mSource);

			for (int i = 1; i < cvsRows.Count; ++i)
			{
				try
				{
					ImportRow(cvsRows[i], sourceAsset.mSource, i);
				}
				catch(Exception e)
                {
					Debug.LogError($"Exception in importing row {i}\n{e.Message}\n{e.StackTrace}");
                }
			}

			EditorUtility.SetDirty(sourceAsset);
		}

        private void ImportRow(string[] row, LanguageSourceData sourceData, int rowNumber)
        {
			var key = KeyOf(row);
			var importedFarsiTranslate = FarsiTranslateOf(row);

			var termData = sourceData.GetTermData(key);

			if ((termData != null && termData.TermType != eTermType.Text) ||
				key.StartsWith(prefixToImport) == false)
				return;

			if (termData == null)
			{
				if(hideErrorForNoTermError == false && key.StartsWith(prefixToCheckForNoTermError))
					Debug.LogError($"There is no term define in LanguageSource for {key} in row {rowNumber + 1}");
			}
			else
			{
				var originalFarsiTranslate = termData.GetTranslation(GetFarsiLanguageIndex()).Replace("\r\n", "\n");

				if (originalFarsiTranslate.Equals(importedFarsiTranslate))
				{
					if (termData.Languages.Length < sourceData.GetLanguages().Count)
						Array.Resize(ref termData.Languages, sourceData.GetLanguages().Count);

					foreach (var language in sourceData.GetLanguages())
						termData.SetTranslation(GetLanguageIndex(language), TranslationOf(language, row));
				}
				else
					Debug.LogError($"Farsi translate does not mach for term {key} in row {rowNumber + 1}\n\nO: {new RTLSupport().FixRTL(originalFarsiTranslate)}\nI:{new RTLSupport().FixRTL(importedFarsiTranslate)}");
			}
		}

        private string FarsiTranslateOf(string[] row)
        {
			return row[columnTypeToIndexMap[PersianColumnName]];
        }

		private string TranslationOf(string language, string[] row)
        {
			return row[columnTypeToIndexMap[language]];
		}

        private string KeyOf(string[] row)
        {
			return row[columnTypeToIndexMap[KeyColumnName]];
		}

        private void SetupColumnMap(List<string[]> cvsRows, LanguageSourceData languageSource)
        {
			columnTypeToIndexMap = new Dictionary<string, int>();

			columnTypeToIndexMap[KeyColumnName] = Array.FindIndex(cvsRows[0], (column) => column.Equals(KeyColumnName));
			columnTypeToIndexMap[TypeColumnName] = Array.FindIndex(cvsRows[0], (column) => column.Equals(TypeColumnName));
			columnTypeToIndexMap[DesColumnName] = Array.FindIndex(cvsRows[0], (column) => column.Equals(DesColumnName));

			columnTypeToIndexMap[PersianColumnName] = Array.FindIndex(cvsRows[0], (column) => column.Equals(PersianColumnName));
			if(columnTypeToIndexMap[PersianColumnName] < 0)
				columnTypeToIndexMap[PersianColumnName] = Array.FindIndex(cvsRows[0], (column) => column.Equals("$"+PersianColumnName));

			foreach (var language in languageSource.GetLanguages())
			{
				columnTypeToIndexMap[language] = Array.FindIndex(cvsRows[0], (column) => column.Equals(language));
				if (columnTypeToIndexMap[language] < 0)
					columnTypeToIndexMap[language] = Array.FindIndex(cvsRows[0], (column) => column.Equals("$" + language));
			}
		}

        private int GetLanguageIndex(string language)
        {
			return languageSource.mSource.GetLanguageIndex(language, SkipDisabled: false);
		}

        private int GetFarsiLanguageIndex()
		{
			return languageSource.mSource.GetLanguageIndex("Persian", SkipDisabled: false);
		}
	}
}
