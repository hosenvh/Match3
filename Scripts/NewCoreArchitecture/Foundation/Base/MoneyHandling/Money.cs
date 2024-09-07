
using System.Collections.Generic;

namespace Match3.Foundation.Base.MoneyHandling
{
    public class Money
    {
        private decimal amount;
        private string currencyCode;

        public Money(decimal amount, string currencyCode)
        {
            this.amount = amount;
            this.currencyCode = currencyCode;
        }

        public decimal Amount()
        {
            return amount;
        }

        public string CurrencyCode()
        {
            return currencyCode;
        }

        public override bool Equals(object otherObj)
        {
            if (otherObj == null || otherObj is Money == false)
            {
                return false;
            }
            else
            {
                var otherMoney = (Money)otherObj;

                return this.amount.Equals(otherMoney.amount)
                    && this.currencyCode.Equals(otherMoney.currencyCode);
            }
        }

        public override int GetHashCode()
        {
            int hashCode = -679235172;
            hashCode = hashCode * -1521134295 + amount.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(currencyCode);
            return hashCode;
        }
    }
}