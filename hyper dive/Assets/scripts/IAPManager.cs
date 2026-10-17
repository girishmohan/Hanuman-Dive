#if HD_IAP
 using System;
using UnityEngine;
using UnityEngine.Purchasing;


public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager instance;
    public int gemAmount;
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    private string gems7500 = "gems_7500";
    private string gems15000 = "gems_15000";
    private string gems50000 = "gems_50000";
    private string gems75000 = "gems_75000";
    private string gems100000 = "gems_100000";
    private string gems250000 = "gems_250000";
    //Step 1 create your products


    private void start()
    {
        gemAmount = PlayerPrefs.GetInt("GemAmount");
    }
    //************************** Adjust these methods **************************************
    public void InitializePurchasing()
    {
        if (IsInitialized()) { return; }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Step 2 choose if your product is a consumable or non consumable
        builder.AddProduct(gems7500, ProductType.Consumable);
        builder.AddProduct(gems15000, ProductType.Consumable);
        builder.AddProduct(gems50000, ProductType.Consumable);
        builder.AddProduct(gems75000, ProductType.Consumable);
        builder.AddProduct(gems100000, ProductType.Consumable);
        builder.AddProduct(gems250000, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    //Step 3 Create methods

    public void BuyGems7500()
    {
        BuyProductID(gems7500);
    }
    public void BuyGems15000()
    {
        BuyProductID(gems15000);
    }
    public void BuyGems50000()
    {
        BuyProductID(gems50000);
    }
    public void BuyGems75000()
    {
        BuyProductID(gems75000);
    }
    public void BuyGems100000()
    {
        BuyProductID(gems100000);
    }
    public void BuyGems250000()
    {
        BuyProductID(gems250000);
    }


    //Step 4 modify purchasing
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, gems7500, StringComparison.Ordinal))
        {
            gemAmount += 7500;
            PlayerPrefs.SetInt("GemAmount", gemAmount);
            Debug.Log("InApp_Purchase passed");
        }
        else if (String.Equals(args.purchasedProduct.definition.id, gems15000, StringComparison.Ordinal))
        {
            gemAmount += 15000;
            PlayerPrefs.SetInt("GemAmount", gemAmount);
            Debug.Log("InApp_Purchase passed");
        }
        else if (String.Equals(args.purchasedProduct.definition.id, gems50000, StringComparison.Ordinal))
        {
            gemAmount += 50000;
            PlayerPrefs.SetInt("GemAmount", gemAmount);
            Debug.Log("InApp_Purchase passed");
        }
        else if (String.Equals(args.purchasedProduct.definition.id, gems75000, StringComparison.Ordinal))
        {
            gemAmount += 75000;
            PlayerPrefs.SetInt("GemAmount", gemAmount);
            Debug.Log("InApp_Purchase passed");
        }
        else if (String.Equals(args.purchasedProduct.definition.id, gems100000, StringComparison.Ordinal))
        {
            gemAmount += 100000;
            PlayerPrefs.SetInt("GemAmount", gemAmount);
            Debug.Log("InApp_Purchase passed");
        }
        else if (String.Equals(args.purchasedProduct.definition.id, gems250000, StringComparison.Ordinal))
        {
            gemAmount += 250000;
            PlayerPrefs.SetInt("GemAmount", gemAmount);
            Debug.Log("InApp_Purchase passed");
        }
        else
        {
            Debug.Log("Purchase Failed");
        }
        return PurchaseProcessingResult.Complete;
    }










    //**************************** Dont worry about these methods ***********************************
    private void Awake()
    {
        TestSingleton();
    }

    void Start()
    {
        if (m_StoreController == null) { InitializePurchasing(); }
    }

    private void TestSingleton()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) => {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}
#else
using UnityEngine;

// IAP disabled (HD_IAP not defined): stub keeps scene references and button hooks valid.
public class IAPManager : MonoBehaviour
{
    public static IAPManager instance;
    public int gemAmount;

    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializePurchasing() { }
    public void BuyGems7500() { Disabled(); }
    public void BuyGems15000() { Disabled(); }
    public void BuyGems50000() { Disabled(); }
    public void BuyGems75000() { Disabled(); }
    public void BuyGems100000() { Disabled(); }
    public void BuyGems250000() { Disabled(); }
    public void RestorePurchases() { Disabled(); }

    void Disabled() { Debug.Log("IAP disabled in this build"); }
}
#endif
