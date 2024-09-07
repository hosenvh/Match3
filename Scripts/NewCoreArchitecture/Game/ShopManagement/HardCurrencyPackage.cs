using Match3.Foundation.Base.MoneyHandling;
using Match3.Main.Localization;
using Medrick.Foundation.ShopManagement.Core;
using System;
using UnityEngine;

namespace Match3.Game.ShopManagement
{
    [Serializable]
    public class DiscountInfo
    {
        [SerializeField] bool hasDiscount;
        [SerializeField] string beforeDiscountSku;

        Money beforeDiscountPrice = MoneyUtilities.InvalidMoney();

        public void SetBeforeDiscountPrice(Money price)
        {
            this.beforeDiscountPrice = price;
        }

        public Money BeforeDiscountPrice()
        {
            return beforeDiscountPrice;
        }

        public bool HasDiscount()
        {
            return hasDiscount;
        }

        public string BeforeDiscountSku()
        {
            return beforeDiscountSku;
        }
    }

    [Serializable]
    public abstract class HardCurrencyPackage : ShopPackage
    {
        [SerializeField] string identifier = default;
        [SerializeField] string sku = default;
        [SerializeField] DiscountInfo discountInfo = default;
        [SerializeField] LanguageBasedString title = default;
        [SerializeField] LanguageBasedString tag = default;
        [SerializeField] int iconIndex = default;

        Money price = new Money(0, "NA");

        public abstract void Apply();

        public Money Price()
        {
            return price;
        }

        public void SetPrice(Money price)
        {
            this.price = price;
        }

        public string Identifier()
        {
            return identifier;
        }

        public string SKU()
        {
            return sku;
        }

        public int IconIndex()
        {
            return iconIndex;
        }

        public string Tag()
        {
            return tag;
        }

        public string Title()
        {
            return title;
        }

        public DiscountInfo DiscountInfo()
        {
            return discountInfo;
        }

        public bool IsFree()
        {
            return sku.IsNullOrEmpty();
        }

        public void OverrideSKU(string sku)
        {
            this.sku = sku;
        }

        public void OverrideDiscount(DiscountInfo discountInfo)
        {
            this.discountInfo = discountInfo;
        }

        internal void OverrideTag(LanguageBasedString tag)
        {
            this.tag = tag;
        }
    }
}