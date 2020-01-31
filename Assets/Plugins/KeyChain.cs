using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Purchasing;
using System.Collections.Generic;

public class KeyChain {
	
	#if UNITY_IPHONE || UNITY_STANDALONE_OSX
	
	[DllImport("__Internal")]
	private static extern string getKeyChainUser();
	
	public static string BindGetKeyChainUser()
	{
		var jsonString = getKeyChainUser();
		var userInfo = JsonUtility.FromJson<UserInfo>(jsonString);
		return userInfo.uuid;
	}
	
	[DllImport("__Internal")]
	private static extern void setKeyChainUser(string userId, string uuid);
	
	public static void BindSetKeyChainUser(string userId, string uuid)
	{
		setKeyChainUser(userId, uuid);
	}
	
	[DllImport("__Internal")]
	private static extern void deleteKeyChainUser();
	
	public static void BindDeleteKeyChainUser()
	{
		deleteKeyChainUser();
	}

	public struct UserInfo
	{
		public string userId;
		public string uuid;
	}
#endif
}
