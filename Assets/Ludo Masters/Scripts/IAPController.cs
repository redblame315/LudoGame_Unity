using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;
using System;

public class IAPController : MonoBehaviour, IStoreListener
{

    public string SKU_50_COINS = "pool_50_coins";
    public string SKU_100_COINS = "pool_100_coins";
    public string SKU_500_COINS = "pool_500_coins";
    public string SKU_1000_COINS = "pool_1000_coins";
    public string SKU_1500_COINS = "pool_1500_coins";

    public IStoreController controller;
    private IExtensionProvider extensions;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        GameManager.Instance.IAPControl = this;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(SKU_50_COINS, ProductType.Consumable);
        builder.AddProduct(SKU_100_COINS, ProductType.Consumable);
        builder.AddProduct(SKU_500_COINS, ProductType.Consumable);
        builder.AddProduct(SKU_1000_COINS, ProductType.Consumable);
        builder.AddProduct(SKU_1500_COINS, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
/*        Debug.Log("IAP Initizalization complete!!");*/
        this.controller = controller;
        this.extensions = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("IAP Initizalization FAILED!! " + error.ToString());
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log("IAP purchase FAILED!! " + p.ToString());
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {

        if (e.purchasedProduct.definition.id == SKU_50_COINS)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(50);
        }
        else if (e.purchasedProduct.definition.id == SKU_100_COINS)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(100);
        }
        else if (e.purchasedProduct.definition.id == SKU_500_COINS)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(500);
        }
        else if (e.purchasedProduct.definition.id == SKU_1000_COINS)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(1000);
        }
        else if (e.purchasedProduct.definition.id == SKU_1500_COINS)
        {
            GameManager.Instance.playfabManager.addCoinsRequest(1500);
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseClicked(int productId)
    {
        if (controller != null)
        {
            if (productId == 1)
            {
                controller.InitiatePurchase(SKU_50_COINS);
            }
            else if (productId == 2)
            {
                controller.InitiatePurchase(SKU_100_COINS);
            }
            else if (productId == 3)
            {
                controller.InitiatePurchase(SKU_500_COINS);
            }
            else if (productId == 4)
            {
                controller.InitiatePurchase(SKU_1000_COINS);
            }
            else if (productId == 5)
            {
                controller.InitiatePurchase(SKU_1500_COINS);
            }
        }
    }


}
