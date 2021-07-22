using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark
//Discord Server: https://discord.gg/ZC4zd3hN5v

//Version 3.4

//Notice: This is not a end all be all to world interaction security and will only keep the players who are not using mods in check. This script is just a deterrent and will only be developed as such.

public class ToggUltimaBouncer : UdonSharpBehaviour
{
	[Header("When this Bouncer Script is linked to another ToggUltima Action, List, or Sync Script: The associated Action Scripts will only allow users that fit the Enabled options here. No enabled options means that feature will be disabled for all legitimate players.")]
	[Tooltip("Only Object Owner Allowed:\n- Check this box to allow the Object Owner to interact with this Toggle. (Usually the Object Owner is the same as the Instance Host.)")]
	[SerializeField] private string[] allowedUsernames = null;
	[Tooltip("Only Object Owner Allowed:\n- Check this box to allow the Object Owner to interact with this Toggle. (Usually the Object Owner is the same as the Instance Host.)")]
	[SerializeField] private string[] blockedUsernames = null;
	[Tooltip("Only Object Owner Allowed:\n- Check this box to always allow the Object Owner to interact with this Toggle. Usually an Object Owner is the same as the Instance Host.")]
	[SerializeField] private bool allowObjectOwnerAlways = true;
	
	[Header("Optional Multiplayer Debug Output")]
	[Tooltip("Debug Text Output:\n- Drag a UI Text object here and it will be updated with function status info from this script.")]
	[SerializeField] private Text debugTextOutput = null;
	
	
	//Wishful thinking for a future revision.
	//[SerializeField] private bool allowInstanceOwner;
	//[Tooltip("Only Object Owner Allowed:\n- Check this box to only allow the Object Owner to interact with this Toggle when there isn't someone present from the Allowed Usernames.\n- Usually an Object Owner is the same as the Instance Host.")]
	//[SerializeField] private bool allowObjectOwnerAsFallback = false;
	//[SerializeField] private bool verifyViaObjectOwner = false;
	//[SerializeField] private string[] verifyViaAllowedPlayer;
	//[SerializeField] private string[] verifyViaInstanceOwner;
	//[SerializeField] private string[] ifOwnerIsBlockedTransferOwnership;

	public bool _toggUltimaCheckList(GameObject checkingObject)
	{
		if(checkingObject != null)
		{
			return _toggUltimaCheckListLocal(checkingObject);
		}
		string bouncerDebugText = "Unable to find player or object. Denied.";
		Debug.LogWarning(bouncerDebugText, gameObject);
		if(debugTextOutput != null)
		{
			_toggUltimaDebugLog(bouncerDebugText);
		}
		return false;
	}
	
	private bool _toggUltimaCheckListLocal(GameObject checkingObject)
	{
		bool playerIsBlocked = false;
		//bool allowedPlayersFound = false;
		string bouncerDebugText = "";
		if(blockedUsernames != null)
		{
			for(int arrayIndex = 0; arrayIndex < blockedUsernames.GetLength(0); arrayIndex++)
			{
				if(blockedUsernames[arrayIndex] != null)
				{
					if(Networking.LocalPlayer != null)
					{
						if(Networking.LocalPlayer.displayName == blockedUsernames[arrayIndex])
						{
							playerIsBlocked = true;
						}
					} else {
						bouncerDebugText = "Local Player not found.";
						Debug.LogWarning(bouncerDebugText, gameObject);
						if(debugTextOutput != null)
						{
							_toggUltimaDebugLog(bouncerDebugText);
						}
					}
				}
			}
		}
		
		if(allowObjectOwnerAlways && Networking.IsOwner(Networking.LocalPlayer, checkingObject))
		{
			if(playerIsBlocked) //&& ifOwnerIsBlockedTransferOwnership)
			{
				bouncerDebugText = "User Denied.";
				Debug.LogWarning(bouncerDebugText, gameObject);
				if(debugTextOutput != null)
				{
					_toggUltimaDebugLog(bouncerDebugText);
				}
				//In a future revision, if ifOwnerIsBlockedTransferOwnership is enabled transfer ownership to someone else since this player isn't trusted.
				return false;
			} else {
				bouncerDebugText = "Object Owner Permitted.";
				Debug.LogWarning(bouncerDebugText, gameObject);
				if(debugTextOutput != null)
				{
					_toggUltimaDebugLog(bouncerDebugText);
				}
				return true;
			}
		}
		
		
		if(!playerIsBlocked && allowedUsernames != null)
		{
			for(int arrayIndex = 0; arrayIndex < allowedUsernames.GetLength(0); arrayIndex++)
			{
				if(allowedUsernames[arrayIndex] != null)
				{
					if(Networking.LocalPlayer != null)
					{
						if(Networking.LocalPlayer.displayName == allowedUsernames[arrayIndex])
						{
							bouncerDebugText = Networking.LocalPlayer.displayName + " Permitted.";
							Debug.LogWarning(bouncerDebugText, gameObject);
							if(debugTextOutput != null)
							{
								_toggUltimaDebugLog(bouncerDebugText);
							}
							return true;
						}
					} else {
						bouncerDebugText = "Local Player not found.";
						Debug.LogWarning(bouncerDebugText, gameObject);
						if(debugTextOutput != null)
						{
							_toggUltimaDebugLog(bouncerDebugText);
						}
					}
				}
			}
		}
		
		//Too expensive of a feature to implement at this time. Perhaps will be implemented in a future version with some better concept planning.
		/*if(allowObjectOwnerAsFallback && !allowedPlayersFound && Networking.IsOwner(Networking.LocalPlayer, checkingObject))
		{
			
		}*/
		
		bouncerDebugText = "User Denied.";
		Debug.LogWarning(bouncerDebugText, gameObject);
		if(debugTextOutput != null)
		{
			_toggUltimaDebugLog(bouncerDebugText);
		}
		return false;
	}
	
	
	public void _toggUltimaDebugLog(string debugText)
	{
		if(debugTextOutput != null)
		{
			debugTextOutput.text = Time.realtimeSinceStartup + ": " + debugText + "\n" + debugTextOutput.text;
			string[] debugTextLog = debugTextOutput.text.Split('\n');
			debugTextOutput.text = "";
			for(int arrayIndex = 0; arrayIndex < 15; arrayIndex++)
			{
				if(debugTextLog != null && arrayIndex < debugTextLog.GetLength(0))
				{
					debugTextOutput.text = debugTextOutput.text + debugTextLog[arrayIndex] + "\n";
				}
			}
		}
	}
}