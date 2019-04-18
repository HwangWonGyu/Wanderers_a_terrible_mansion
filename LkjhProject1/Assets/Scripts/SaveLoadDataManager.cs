using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SaveLoadDataManager : Singleton<SaveLoadDataManager> {
	const string str_settingData = "SettingDatas1";
	public const string str_adsData = "AdsDataTest";
	public SettingData data;
	public AdsData adsData;
    public bool isLoadingComplete;
	// Use this for initialization
	void Awake () {
        if (sInstance == null)
        {
            sInstance = this;
            isLoadingComplete = false;
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(gameObject);
        }
      
	}
    private void Start()
    {
        LoadSettingData();
		//LoadAdsData();
    }

    public void SaveSettingData()
	{
		SaveData(str_settingData, data);
	}
	public void LoadSettingData()
	{
		if (!PlayerPrefs.HasKey(str_settingData))
		{
			ResetSettingData();
			
		}
		data = (SettingData)LoadData(str_settingData);
        isLoadingComplete = true;
    }
	public void ResetSettingData()
	{
		data = new SettingData();
		data.HorizontalRotateSensitive = 3;
		data.VerticalRotateThreshold = 0.1f;
        data.EffectVolume = 1f;
        data.BgmVolume = 1f;
        data.IsExtremeValue = false;
        SaveSettingData();
    }
	//public void SaveAdsData()
	//{
	//	SaveData(str_adsData, adsData);
	//}
	//public void LoadAdsData()
	//{
	//	//if (!PlayerPrefs.HasKey(str_adsData))
	//	//{
	//	//	ResetAdsData();
	//	//}
	//	adsData = (AdsData)LoadData(str_adsData);
	//}
	
	//public void ResetAdsData(bool value)
	//{
	//	print("ResetAdsData");
	//	adsData = new AdsData();
	//	adsData.IsAdsSkipPurchase = value;
	//	SaveAdsData();
	//}
	public void SaveData(string dataName, object data)
	{
		BinaryFormatter binary = new BinaryFormatter();
		MemoryStream memory = new MemoryStream();
		binary.Serialize(memory, data);
		PlayerPrefs.SetString(dataName, Convert.ToBase64String(memory.GetBuffer()));
	}
	public object LoadData(string dataName)
	{
		string data = PlayerPrefs.GetString(dataName);
		if (!string.IsNullOrEmpty(data))
		{
			BinaryFormatter binary = new BinaryFormatter();
			MemoryStream memory = new MemoryStream(Convert.FromBase64String(data));
			return binary.Deserialize(memory);
		}
		return null;
	}

	[Serializable]
	public class SettingData
	{
		int horizontalRotateSensitive; //PlayerJoystick.cs의 horizontalRotateSensitive(1 ~ 5)에 쓰임
		public int HorizontalRotateSensitive
		{
			get	{ return horizontalRotateSensitive;	}
			set
			{
				if (value > 15)
					horizontalRotateSensitive = 15;
				else if (value < 1)
					horizontalRotateSensitive = 1;
				else
					horizontalRotateSensitive = value;
			}
		}

		float verticalRotateThreshold; //PlayerJoystick.cs의 verticalRotateThreshold(0.01 ~ 0.9)에 쓰임
		public float VerticalRotateThreshold
		{
			get { return verticalRotateThreshold; }
			set
			{
				if (value > 0.9f)
					verticalRotateThreshold = 0.9f;
				else if (value < 0.01f)
					verticalRotateThreshold = 0.01f;
				else
					verticalRotateThreshold = value;
			}
		}
        float effectVolume=1f;
        public float EffectVolume
        {
            get
            {
                return effectVolume;
            }

            set
            {
                if (value > 1.0f)
                    value = 1.0f;
                else if (value < 0)
                    value = 0;
                effectVolume = value;
            }
        }
        float bgmVolume=1f;
        public float BgmVolume
        {
            get
            {
                return bgmVolume;
            }

            set
            {
                if (value > 1.0f)
                    value = 1.0f;
                else if (value < 0)
                    value = 0;
                bgmVolume = value;
            }
        }

        public bool IsExtremeValue
        {
            get
            {
                return IsExtreme;
            }

            set
            {
                IsExtreme = value;
            }
        }

        private bool IsExtreme;
	}
	[Serializable]
	public class AdsData
	{
		private bool isAdsSkipPurchase;

		public bool IsAdsSkipPurchase
		{
			get
			{
				return isAdsSkipPurchase;
			}

			set
			{
				isAdsSkipPurchase = value;
			}
		}
	}

}
