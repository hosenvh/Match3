using I2.Loc;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Match3.EditorTools.Editor.Menus.Localization
{
    public partial class I2LanguageSourceAssetUtility
    {
        private class ScenarioTermDataPackage
        {
	        private enum PrePostType
	        {
		        Pre,
		        Post
	        }

	        public TermData OriginalData { get; }
	        public long Index { get; }

	        public ScenarioTermDataPackage(TermData originalData)
	        {
		        OriginalData = originalData;
		        try
		        {
			        // Original Data Scenario/Day00_DialogueBubble_post01_S123
			        // we convert this to a corresponding number (called index) like this
			        // first we remove the "Scenario/" from start of this strings
			        string termText = originalData.Term.Split('/')[1];

			        // Then we split it into needed Parts
			        List<string> parted = termText.Split('_').ToList();

			        int dayNumber = int.Parse(parted[0].Remove(0,3)); // Removing "Day"
			        PrePostType prePostType = parted[2].Contains("pre") ? PrePostType.Pre : PrePostType.Post;

			        int prePostNumber = 0;
			        switch (prePostType)
			        {
				        case PrePostType.Pre:
					        prePostNumber = int.Parse(parted[2].Remove(0,3)); // Removing "Pre"
					        break;
				        case PrePostType.Post:
					        prePostNumber = int.Parse(parted[2].Remove(0,4)); // Removing "Post"
					        break;
			        }

			        int sNumber = int.Parse(parted[3].Remove(0,2)); // Removing "SI"
			        int dNumber = 0;
			        if (parted.Count >= 5)                           // some datas do not have last dNumber we assume theirs to be 0
				        dNumber = int.Parse(parted[4].Remove(0, 1)); // Removing "d"

			        Index = CalculateIndex(dayNumber, prePostType , prePostNumber,  sNumber, dNumber);
		        }
		        catch (Exception e)
		        {
			        Debug.LogError("Logically this was not excepted to happen. it seems we have some terms breaking the decided naming rule. The process of sorting is not broken now, however we will add this items to end of the list. Unity Exception: " + e);
			        Index = Int64.MaxValue;
		        }
	        }

	        private long CalculateIndex(int day, PrePostType prePostType , int prePostNumber, int sNumber, int dNumber)
	        {
		        // Original Data Day00_DialogueBubble_post01_S123
		        // we convert this to a corresponding number (called index) like this
		        // nDigit for day - nDigit for PrePostNumber - 1digit for PrePost - nDigit for sNumber - nDigit for dNumber
		        // n depends on the biggest day,sOrD number and PrePost Number, if the biggest Possible is 99999 we assume n to be 5
		        // Note that, calculating a wrong 'n' can cause the sort algorithm work wrong.
		        int digitCount = 3; // this is the 'n' mentioned in above comment

		        long result = 0;
		        result += dNumber;
		        result += (int) (sNumber * Mathf.Pow(10, digitCount));
		        result += (long) ((int) prePostType * Mathf.Pow(10, 2 * digitCount));
		        result += (long) (prePostNumber * Mathf.Pow(10, (2 * digitCount)+1));

		        result += (long) (day * Mathf.Pow(10, (3 * digitCount) + 1));

		        return result;
	        }
        }
	}
}
