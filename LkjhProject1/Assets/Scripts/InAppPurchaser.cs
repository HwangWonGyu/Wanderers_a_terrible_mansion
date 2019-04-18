using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
public class InAppPurchaser : Singleton<InAppPurchaser>,IStoreListener {

	private static IStoreController storeController;
	private static IExtensionProvider extensionProvider;
	
	#region 상품ID
	public const string productId= "adsskip";
	#endregion

	private void Awake()
	{
		if (sInstance == null)
		{
			sInstance = this;
			
			DontDestroyOnLoad(gameObject);

		}
		else
		{
			Destroy(gameObject);
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		InitializePurchasing();

		//StartCoroutine(LoadAdsdatas());

	}
	//IEnumerator LoadAdsdatas()
	//{
	//	while (!IsInitialized())
	//		yield return null;
	//	if (!PlayerPrefs.HasKey(SaveLoadDataManager.str_adsData))
	//	{
	//		SaveLoadDataManager.GetInstance.ResetAdsData(HasAdsSkip());
	//	}
	//	else
	//		SaveLoadDataManager.GetInstance.LoadAdsData();
	//}
	public bool HasAdsSkip()
	{
		Product product = storeController.products.WithID(productId);
		if (product != null && product.hasReceipt)
		{
			return true;
		}
		else
			return false;
	}
	
	private bool IsInitialized()
	{
		return (storeController != null && extensionProvider != null);
	}
	public void InitializePurchasing()
	{
		if (IsInitialized())
			return;
		var module = StandardPurchasingModule.Instance();

		ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

		builder.AddProduct(productId, ProductType.NonConsumable, new IDs
		{
			{productId,AppleAppStore.Name },{productId,GooglePlay.Name},
		});

		UnityPurchasing.Initialize(this, builder);

	}
	public void BuyProductID(string productId)
	{
		
		try
		{
			if (IsInitialized())
			{
				Product product = storeController.products.WithID(productId);

				if (product != null && product.availableToPurchase)
				{
					Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));

					storeController.InitiatePurchase(product);
				}
				else
					Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
			}
			else
			{
				Debug.Log("BuyProductID: FAIL.Initialized Failed");
			}
		}
		catch (Exception e)
		{
			Debug.Log("BuyProductID : FAIL. Exception during purchase. " + e);
		}
	}
	public void RestorePurchase()
	{
		if (!IsInitialized())
		{
			Debug.Log("RestorePurchases FAIL. Not initialized.");
			return;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			var apple = extensionProvider.GetExtension<IAppleExtensions>();
			apple.RestoreTransactions
			  (
				  (result) => { Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); }
			  );
		}
	}

	
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		storeController = controller;
		extensionProvider = extensions;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("OnInitializedFailed ");
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		switch (args.purchasedProduct.definition.id)
		{
			case productId:
				//SaveLoadDataManager.GetInstance.adsData.IsAdsSkipPurchase = true;
				//SaveLoadDataManager.GetInstance.SaveAdsData();
				//SaveLoadDataManager.GetInstance.LoadAdsData();
				UIManager.GetInstance.ShopPanelOff();
				break;
		}
		return PurchaseProcessingResult.Complete;
	}
	
}
