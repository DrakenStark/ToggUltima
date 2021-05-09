using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark

//Special Thanks to Zephyxus

//Version 2.5

//Known issue: Spammed buttons will not sync 100% of the activations. This may be resolved by creating a buffer. This is a "to do" for a future revision.

public class ToggUltimaSync : UdonSharpBehaviour
{
	
	[Header("Synchronized Action Scripts")]
	[Tooltip("Toggle Action Scripts:\n- Drag ToggUltima Action Scripts here to automatically have them synchronized between current players and late joiners.\n- Avoid using more than 30 Action Scripts. If more than 30 Action Scripts need to be synced between players, create another Object for an additional ToggUltimaSync Script and repeat as needed.")]
	[SerializeField] private ToggUltimaAction[] toggleActionScripts;
	[UdonSynced] private int binaryStatesSync = 1;
	private int binaryStatesLocal = 1;
	//For now needSync is only used during initialization, but it could be also used in future improvements to the script.
	private bool needSync = true;
	[HideInInspector] public bool firstRun = true;
	[Header("Optional Features")]
	[Tooltip("Cooldown Period:\n- Prevent the script from running immediately after it has just been run.\n- This decreases the chances of losing synchronization because of players spamming buttons. If there are too many actions to run, clients may begin to ignore some of them.")]
	public float cooldownPeriod = 0.5f;
	//This is to prevent players from producing a lot of network spam.
	[HideInInspector] public bool coolingDown = false;
	[HideInInspector] public float timeOfLastCooldown;
	[Tooltip("Deserialization Delay:\n- This is how often late joiners will check if they've received the variable synchronization needed to let them catch up to current players.\n- Generally doesn't need to be changed, but for more intense worlds, this number may be increased if needed.")]
	[SerializeField] private float deserializationDelay = 2f;
	private float deserializationFilterTime = 0f;
	private bool deserializationFilterBool = false;
	[Tooltip("Persistent Deserialization:\n- Notice: This is an experimental feature!\n- If synchronization is a must at all costs, this may be used as a last resort if players are somehow getting out of sync. This will force players to double check their current toggles with the last updated value they have received.")]
	[SerializeField] private bool persistentDeserialization = false;
	[Header("Optional Multiplayer Debug Output")]
	[Tooltip("Debug Sync Display:\n- Drag a UI Text object here and it will be updated with variable status info from this script.")]
	[SerializeField] private Text debugSyncDisplay;
	[Tooltip("Debug Text Output:\n- Drag a UI Text object here and it will be updated with function status info from this script.")]
	[SerializeField] private Text debugTextOutput;

	
	public void OnEnable()
	{
		_toggUltimaDebugLog("Sync Script Enabled.");
		
		timeOfLastCooldown = Time.time - cooldownPeriod;
		deserializationFilterTime = Time.time - deserializationDelay;
		
		binaryStatesLocal = 1;
		needSync = true;
		firstRun = true;
		for(int arrayIndex = 0; arrayIndex < toggleActionScripts.GetLength(0); arrayIndex++)
		{
			if(toggleActionScripts[arrayIndex] != null)
			{
				if(toggleActionScripts[arrayIndex].toggleIsActive)
				{
					binaryStatesLocal = (1<<(arrayIndex + 1)) + binaryStatesLocal;
				}
				
				if(toggleActionScripts[arrayIndex].objectWithSync != this)
				{
					toggleActionScripts[arrayIndex].objectWithSync = this;
				}
			}
			
			_updateDebugSyncText();
		}
		
		//Set the synced integer if the Object Owner is running this script.
		if(Networking.IsOwner(Networking.LocalPlayer, gameObject))
		{
			_toggUltimaOwnerEnableSync();
		} else {
			//Wherever needSync is set, this function should be run.
			_toggUltimaDeserialization();
		}
	}
	
	public void _cooldownFunctionHot()
	{
		coolingDown = true;
		timeOfLastCooldown = Time.time;
		SendCustomEventDelayedSeconds("toggUltimaActionFilter", cooldownPeriod);
	}
	public void _cooldownFunctionCooled()
	{
		if(Time.time >= timeOfLastCooldown + cooldownPeriod)
		{
			coolingDown = false;
		}
	}
	
	private void _toggUltimaOwnerEnableSync()
	{
		if((binaryStatesLocal & 1) != 0)
		{
			binaryStatesLocal = binaryStatesLocal - 1;
		}
		needSync = false;
		firstRun = false;
		_toggUltimaUpdateLocalVariable();
		binaryStatesSync = binaryStatesLocal;
		_toggUltimaDebugLog("Sync variable initialized.");
		_updateDebugSyncText();
	}
	
	
	public void _toggUltimaUpdateLocalVariable()
	{
		_toggUltimaDebugLog("SyncScript Local Variable Updated.");
		for(int arrayIndex = 0; arrayIndex < toggleActionScripts.GetLength(0); arrayIndex++)
		{
			if(toggleActionScripts[arrayIndex] != null && toggleActionScripts[arrayIndex].toggleIsActive && (binaryStatesLocal & (1<<(arrayIndex + 1))) == 0)
			{
				binaryStatesLocal = (1<<(arrayIndex + 1)) + binaryStatesLocal;
			} else if(toggleActionScripts[arrayIndex] != null && !toggleActionScripts[arrayIndex].toggleIsActive && ((binaryStatesLocal & (1<<(arrayIndex + 1))) != 0)) {
				binaryStatesLocal = binaryStatesLocal - (1<<(arrayIndex + 1));
			}
		}
		_updateDebugSyncText();
	}
	
	
	public void _toggUltimaUpdateSyncVariable()
	{
		_toggUltimaDebugLog("SyncScript Sync Variable Updated.");
		for(int arrayIndex = 0; arrayIndex < toggleActionScripts.GetLength(0); arrayIndex++)
		{
			if(toggleActionScripts[arrayIndex] != null && toggleActionScripts[arrayIndex].toggleIsActive && (binaryStatesSync & (1<<(arrayIndex + 1))) == 0)
			{
				binaryStatesSync = (1<<(arrayIndex + 1)) + binaryStatesSync;
			} else if(toggleActionScripts[arrayIndex] != null && !toggleActionScripts[arrayIndex].toggleIsActive && ((binaryStatesSync & (1<<(arrayIndex + 1))) != 0)) {
				binaryStatesSync = binaryStatesSync - (1<<(arrayIndex + 1));
			}
		}
		_updateDebugSyncText();
	}
	
	
	public void _toggUltimaDeserialization()
	{
		if(Time.time >= (deserializationFilterTime + deserializationDelay))
		{
			deserializationFilterBool = false;
		}
		
		if(needSync && !deserializationFilterBool)
		{
			
			//Synced variable is not initialized correctly in VRChat. Should VRChat fix this, then this firstRun section and variable may be removed.
			if(firstRun)
			{
				_toggUltimaDebugLog("Waiting for synchronization... ( <.<)\n" + _toggUltimaBinaryStatus());
				//This line used to work around an issue with VRChat not initializing the synced integer, so manually got it to update via this. (Before I added an underscore to stop network calls to it.)
				//SendCustomNetworkEvent(NetworkEventTarget.Owner, "toggUltimaOwnerEnableSync()");
			}
			
			if(binaryStatesSync != binaryStatesLocal)
			{
				if(!firstRun)
				{
					needSync = false;
				}
				
				//_toggUltimaDebugLog("Deserialization Function run.");
				bool binaryMatch = false;
				for(int arrayIndex = 0; arrayIndex < toggleActionScripts.GetLength(0); arrayIndex++)
				{
					if(toggleActionScripts[arrayIndex] != null && (binaryStatesSync & (1<<(arrayIndex + 1))) != (binaryStatesLocal & (1<<(arrayIndex + 1))))
					{
							if((binaryStatesSync & (1<<(arrayIndex + 1))) != 0)
							{
								binaryStatesLocal = binaryStatesLocal + (1<<(arrayIndex + 1));
								toggleActionScripts[arrayIndex].toggleIsActive = false;
							} else {
								binaryStatesLocal = binaryStatesLocal - (1<<(arrayIndex + 1));
								
								//One Way Toggles must always be false, otherwise this will double check to ensure the proper toggle mode is activated.
								if(toggleActionScripts[arrayIndex].oneWayActivation)
								{
									toggleActionScripts[arrayIndex].toggleIsActive = false;
								} else {
									toggleActionScripts[arrayIndex].toggleIsActive = true;
								}
							}
							toggleActionScripts[arrayIndex].toggUltimaActivate();
					}
				}
				binaryStatesLocal = binaryStatesSync;
				
				if(firstRun)
				{
					needSync = false;
					firstRun = false;
					_toggUltimaDebugLog("Synchronized! Yey! \\(^.^)/\n" + _toggUltimaBinaryStatus());
				}
				
				_updateDebugSyncText();
			}
			if(needSync || persistentDeserialization)
			{
				needSync = true;
				deserializationFilterBool = true;
				deserializationFilterTime = Time.time;
				SendCustomEventDelayedSeconds("_toggUltimaDeserialization", deserializationDelay);
			}
		}
	}
	
	
	public void _updateDebugSyncText()
	{
		if(debugSyncDisplay != null)
		{
			debugSyncDisplay.text = ("NeedSync: " + needSync + " | FirstRun: " + firstRun + "\nTime of last update: " + Time.realtimeSinceStartup);
			debugSyncDisplay.text = (debugSyncDisplay.text + "\n" + _toggUltimaBinaryStatus());
		}
	}
	
	
	public string _toggUltimaBinaryStatus()
	{
		string debuglocal = "";
		string debugsync = "";
		for(int arrayIndex = 0; arrayIndex < (toggleActionScripts.GetLength(0) + 1); arrayIndex++)
		{
			if((binaryStatesSync & (1<<arrayIndex)) != 0)
			{
				debugsync = ("1" + debugsync);
			} else {
				debugsync = ("0" + debugsync);
			}

			if((binaryStatesLocal & (1<<arrayIndex)) != 0)
			{
				debuglocal = ("1" + debuglocal);
			} else {
				debuglocal = ("0" + debuglocal);
			}
		}
		return (debugsync + " - " + binaryStatesSync + " = Sync | " + debuglocal + " - " + binaryStatesLocal + " = Local");
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