using System;
using Match3.Game.KeyShop;
using Match3.Presentation;
using Match3.Presentation.TextAdapting;
using RTLTMPro;
using SeganX;
using UnityEngine;


public class KeyShopItem : MonoBehaviour
{

    [SerializeField] private GameObject tagParentObject = default;
    [SerializeField] private GameObject[] packageIconObjects = default;
    [SerializeField] private TextAdapter titleText = default;
    [SerializeField] private RTLTextMeshPro tagText2 = default;
    [SerializeField] private LocalText keyCountText = default;
    [SerializeField] private LocalText priceText = default;
    
    private KeyShopPackage package;
    
    private Action<KeyShopPackage> onPurchaseButtonClick;

    
    
    
    public KeyShopItem Setup(KeyShopPackage package, Action<KeyShopPackage> onButtonClick)
    {
        onPurchaseButtonClick = onButtonClick;
        this.package = package;

        SetTag(package.Tag());
        SetPackageIcon(package.IconIndex());
        
        titleText.SetText(package.Title());
        keyCountText.SetText(package.KeyCount().ToString());
        priceText.SetText(package.Price().FormatMoneyToString());
        
        return this;
    }

    private void SetTag(string tag)
    {
//        tag = tag.Replace("\\n", "\n");
        tagParentObject.SetActive(false);
        
        if (string.IsNullOrEmpty(tag)) return;
        tagParentObject.SetActive(true);
        tagText2.text = tag;
//        tagText.SetText(tag);
    }

    private void SetPackageIcon(int iconIndex)
    {
        packageIconObjects[Mathf.Min(iconIndex, packageIconObjects.Length - 1)].SetActive(true);
    }
    
    public void PurchaseButtonClick()
    {
        onPurchaseButtonClick?.Invoke(package);
    }
    

}
