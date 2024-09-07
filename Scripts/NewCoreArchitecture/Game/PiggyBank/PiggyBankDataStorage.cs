using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Data.PiggyBank
{
    public class PiggyBankDataStorage
    {
        private const string CurrentCoinsKey = "PIGGY_BANK_CURRENT_SAVED_COINS";
        private const string BankLevelKey = "PIGGY_BANK_LEVEL";
        private const string IsUnlockedKey = "PIGGY_BANK_UNLOCK_STATE";


        public int CurrentCoins
        {
            get => PlayerPrefs.GetInt(CurrentCoinsKey, 0);
            set => PlayerPrefs.SetInt(CurrentCoinsKey, value);
        }

        public int BankLevel
        {
            get => PlayerPrefs.GetInt(BankLevelKey, 1);
            set => PlayerPrefs.SetInt(BankLevelKey, value);
        }

        public bool IsUnlocked
        {
            get => PlayerPrefs.GetInt(IsUnlockedKey, 0) == 1;
            set => PlayerPrefs.SetInt(IsUnlockedKey, value == true ? 1 : 0);
        }
    }
}