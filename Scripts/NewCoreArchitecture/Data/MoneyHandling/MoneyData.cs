using Match3.Foundation.Base.MoneyHandling;
using PandasCanPlay.HexaWord.Utility;
using System;

namespace Match3.Data.MoneyHandling
{
    [System.Serializable]
    public class MoneyData
    {
        [UnityEngine.SerializeField] public double amount;

        [Dropdown("IRR","TRY")]
        [UnityEngine.SerializeField] public string currencyCode;


        public Money ToMoney()
        {
            return new Money(Convert.ToDecimal(amount), currencyCode);
        }
    }
}