namespace Match3.Foundation.Base.MoneyHandling
{
    public static class MoneyUtilities
    {
        private static readonly Money INVALID_MONEY = new Money(0, "NA");

        public static bool IsValid(this Money money)
        {
            return INVALID_MONEY.Equals(money) == false;
        }

        public static Money InvalidMoney()
        {
            return INVALID_MONEY;
        }    
    }
}