using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark
//Discord Server: https://discord.gg/ZC4zd3hN5v

//Special Thanks to ArtySilvers

//Version 3.4

public class ToggUltimaAction : UdonSharpBehaviour
{
	[Header("List Object")]
	[Tooltip("Object With List:\n- An object that has an ToggUltimaList script may be dragged here to support mutual exclusivity.\n- The ToggUltimaList must be configured to disable all objects you wish to have mutually exclusive.\n-All ToggUltimaAction scripts using the same Object With List will be mutually exclusive with each other.")]
	[SerializeField] private ToggUltimaList objectWithList = null;
	[Tooltip("Auto Add OTE To OTD:\n- Automatically adds the Objects To Enable to the Object With List's Objects To Disable when the object this script is in is loaded or enabled for the first time.")]
	[SerializeField] private bool autoAddOTEToOTD = false;
	private bool toggleListUpdated = false;
	
	//[Header("Multiplayer Sync Object")]
	//[Tooltip("Object With Sync:\n- An object that has an ToggUltimaSync script may be dragged here to support keeping this script synced up with all players.")]
	//This is now set automatically when this script is added to a ToggUltima Sync Script.
	[HideInInspector] public ToggUltimaSync objectWithSync = null;
	
	[Header("Toggled Objects")]
	[Tooltip("Objects To Enable:\n- Drag Objects here to be Enabled while this Toggle is Active via interaction with by a player.\n- To be interacted with, the object with this script must have a collider!")]
	[SerializeField] private GameObject[] objectsToEnable = null;
	[Tooltip("Objects To Enable:\n- Drag Objects here to be Disabled while this Toggle is Active via interaction with by a player.\n- Upon this Toggle being Deactivated via player interaction or another Toggle being Activated, these objects will be Enabled.\n- If you want objects that will stay disabled after this Toggle being Activated, include them into the Disabled Objects section of an Object With List.\n- To be interacted with, the object with this script must have a collider!")]
	[SerializeField] private GameObject[] objectsToReenable = null;
	[HideInInspector] public bool toggleIsActive = false;
	
	[Header("Optional Features")]
	[Tooltip("One Way Activation:\n- Check this box to prevent players from disabling this Toggle via a second interaction.\n- While One Way Activation is enabled, the only way this Toggle may be Deactivated is via another Toggle through an Object With List.")]
	public bool oneWayActivation = false;
	[Tooltip("Activate On Interact:\n- Check this box to allow players to activate this script through direct interaction with the object containing it.")]
	[SerializeField] private bool activateOnInteract = true;
	//This is now automated, no need to include it.
	//[Tooltip("Allow Other Script Interaction:\n- Check this box to enable the ability for players to directly interact with the object containing this script.\n- Only uncheck this box if there is no intention of allowing players to directly interact with this object.")]
	//[SerializeField] private bool allowScriptInteraction = true;
	[Tooltip("Activate On Enable:\n- Check this box to activate this script whenever the object containing it is enabled (upon load or after it had been disabled).")]
	[SerializeField] private bool activateOnEnable = false;
	[Tooltip("Activate On Disable:\n- Check this box to activate this script whenever the object containing it is disabled (will not run without being enabled first).")]
	[SerializeField] private bool activateOnDisable = false;
	[Tooltip("Activation Delay:\n- If set to 0, this feature, Ignore While Waiting, and Reset Timer On Activate will be ignored and the script will always activate immediately when activated.\n- If set to anything above 0, whenever this script is activated, it will wait that many seconds before toggling objects.")]
	[SerializeField] private float activationDelay = 0f;
	[Tooltip("Ignore While Waiting:\n- This will be ignored if Activation Delay is set to 0.\n- Cannot be used simultaniously with Reset Timer On Activate.\n- Once activated and while waiting, any activations will be ignored until after the time is up and objects are toggled.")]
	[SerializeField] private bool ignoreWhileWaiting = true;
	private bool waitingToActivate = false;
	[Tooltip("Reset Timer On Activate:\n- This will be ignored if Activation Delay is set to 0.\n- Cannot be used simultaniously with Ignore While Waiting.\n- Once activated and while waiting, any activation will reset the delay for when the objects will be toggled.")]
	[SerializeField] private bool resetTimerOnActivate = false;
	private float timeOfLastActivation;
	[HideInInspector] public bool timerSyncFirstRunFilter = true;
	private bool timerPartThreeActivationFilter = false;
	[Tooltip("Object With Bouncer:\n- Use an object with a ToggUltima Bouncer script here to restrict legitimate use of a toggle to specific users.\n- Notice: This is not a end all be all to world interaction security and will only keep the players who are not using mods in check. The Bouncer script and its implementation is just a deterrent and will only be developed as such.")]
	[SerializeField] private ToggUltimaBouncer objectWithBouncer = null;
	
	
	//This is where arrays of GameObjects are checked to be enabled or disabled accordingly.
	private void _toggUltimaModifyGameObjectArray(GameObject[] gOArray, bool objectState, string sourceOfCall)
	{
		if(gOArray != null)
		{
			if(gOArray.GetLength(0) > 0)
			{
				foreach(GameObject modifyMe in gOArray)
				{
					if(modifyMe != null)
					{
						if(modifyMe.activeSelf != objectState)
						{
							modifyMe.SetActive(objectState);
						}
					} else {
						Debug.LogWarning("Missing Object (which was safely ignored) was found with " + sourceOfCall + " " + gOArray + " " + objectState, gameObject);
					}
				}
			}
		}
	}
	
	//If the option is enabled, the script runs via a GameObject that has been interacted with, disabled in anyway, or enabled via another object.
	public override void Interact()
	{
		if(activateOnInteract)
		{
			toggUltimaTimerPartOne();
		}
	}
	
	private void OnDisable()
	{
		if(activateOnDisable)
		{
			toggUltimaTimerPartOne();
		}
	}
	
	//The OnEnable function here not only activates the main toggle function, but also can check if the Objects to Enable have been added to the List Object.
	private void OnEnable()
	{
		if(!activateOnInteract) {
			this.DisableInteractive = true;
		}
		
		//Commented out due to issue within VRChat itself. May attempt to get working again in the future.
		//Should timeOfLastActivation is left at 0, this script won't run until elapsed time exceeds the activationDelay, so it's set here subtracting the activationDelay to allow for immediate use.
		timeOfLastActivation = Time.time - activationDelay;
		
		if(!toggleListUpdated)
		{
			toggleListUpdated = true;
			if(autoAddOTEToOTD && objectWithList != null && objectsToEnable.GetLength(0) > 0)
			{
				objectWithList._autoUpdateOTD(objectsToEnable, gameObject);
			}
		}
		
		if(activateOnEnable)
		{
			toggUltimaTimerPartOne();
		}
	}
	
	
	//This function is run to determine if there's a waiting period before running the object toggles.
	public void toggUltimaTimerPartOne()
	{
		//Logically if objectWithBouncer does not exist, the code should run. Networking.IsOwner() will only matter if objectWithBouncer is present, so it is more efficient code to not test if objectWithBouncer exists a second time for each location that it may exist.
		if((!(objectWithBouncer != null) || objectWithBouncer._toggUltimaCheckList(gameObject)) && ((!(objectWithList != null) || !(objectWithList.objectWithBouncer != null)) || objectWithList.objectWithBouncer._toggUltimaCheckList(gameObject)) &&  ((!(objectWithSync != null) || !(objectWithSync.objectWithBouncer != null)) || objectWithSync.objectWithBouncer._toggUltimaCheckList(gameObject)))
		{
			if(objectWithSync != null && Time.time >= (objectWithSync.timeOfLastCooldown + objectWithSync.cooldownPeriod))
			{
				objectWithSync.coolingDown = false;
			}
			if(activationDelay > 0)
			{
				if(ignoreWhileWaiting && !waitingToActivate || !ignoreWhileWaiting)
				{
					if(objectWithSync != null)
					{
						if(!objectWithSync.coolingDown)
						{
							SendCustomNetworkEvent(NetworkEventTarget.All, "toggUltimaTimerPartTwoSync");
						}
					} else {
						toggUltimaTimerPartTwoLocal();
					}
				}
			//The else here will run in the case there is supposed to not be a delay before running after being activated.
			} else {
				//The delay based variables are still tracked here for debugging purposes should a world developer wish to test general functionality of their setup faster where there is supposed to be a delay.
				if(objectWithSync != null)
				{
					if(!objectWithSync.coolingDown)
					{
						timeOfLastActivation = Time.time;
						objectWithSync._toggUltimaDebugLog("No Timer Run Globally. " + timeOfLastActivation);
						waitingToActivate = true;
						timerPartThreeActivationFilter = true;
						timerSyncFirstRunFilter = false;
						SendCustomNetworkEvent(NetworkEventTarget.All, "toggUltimaActionFilter");
					}
				} else {
					timeOfLastActivation = Time.time;
					waitingToActivate = true;
					timerPartThreeActivationFilter = true;
					timerSyncFirstRunFilter = false;
					toggUltimaActionFilter();
				}
			}
		}
	}
	
	public void toggUltimaTimerPartTwoLocal()
	{
		timeOfLastActivation = Time.time;
		if(objectWithSync != null)
		{
			objectWithSync._toggUltimaDebugLog("Timer 2 Locally Run. " + timeOfLastActivation);
		}
		//waitingToActivate checks if the script has been activated at all and is waiting on a delay. This is to ensure only the first activation runs and any others are ignored if ignoreWhileWaiting is enabled.
		waitingToActivate = true;
		timerPartThreeActivationFilter = true;
		timerSyncFirstRunFilter = false;
		SendCustomEventDelayedSeconds("toggUltimaActionFilter", activationDelay);
	}
	
	
	public void toggUltimaTimerPartTwoSync()
	{
		//timesActivated++;
		timeOfLastActivation = Time.time;
		objectWithSync._toggUltimaDebugLog("Timer 2 Sync Run. " + timeOfLastActivation);
		waitingToActivate = true;
		SendCustomEventDelayedSeconds("toggUltimaTimerPartThreeSync", activationDelay);
	}
	
	public void toggUltimaTimerPartThreeSync()
	{
		objectWithSync._toggUltimaDebugLog("Timer 3 Sync Run Globally.");
		timerPartThreeActivationFilter = true;
		timerSyncFirstRunFilter = false;
		SendCustomNetworkEvent(NetworkEventTarget.All, "toggUltimaActionFilter");
	}
	
	
	//The actual Action function needs to be seperate from it's if statement to be allowed to run for Late Joiners.
	public void toggUltimaActionFilter()
	{
		float lookAtMeImTheTimeNow = Time.time;
		if(objectWithSync != null)
		{
			objectWithSync._toggUltimaDebugLog("Activation Attempt. " + lookAtMeImTheTimeNow);
			
			if(timerSyncFirstRunFilter)
			{
				timerSyncFirstRunFilter = false;
				//Have the synchronizing Late Joiner use the same key to the door that everyone else has. They'll be using it anyway later like everyone else.
				timerPartThreeActivationFilter = true;
			}
		}
		
		//If ignoreWhileWaiting or resetTimerOnActivate are false, then the conditions on the right of each don't matter. Otherwise if either are true, then their respective requirement has to be met to run.
		if(!(activationDelay > 0) || ((!ignoreWhileWaiting || waitingToActivate && timerPartThreeActivationFilter) && (!resetTimerOnActivate || ((timeOfLastActivation + activationDelay <= lookAtMeImTheTimeNow) && timerPartThreeActivationFilter))))
		{
			toggUltimaActivate();
			if(objectWithSync != null)
			{
				objectWithSync._toggUltimaDebugLog("Activation Run. " + lookAtMeImTheTimeNow);
			}
		}
	}
	
	public void toggUltimaActivate()
	{	
		timerPartThreeActivationFilter = false;
		
		if(objectWithSync != null)
		{
			objectWithSync._cooldownFunctionHot();
		}
			
		if(objectWithList != null)
		{
			if(objectWithList.interactibleObjectsToReenable != null && objectWithList.interactibleObjectsToReenable.GetLength(0) > 0)
			{
				_toggUltimaModifyGameObjectArray(objectWithList.interactibleObjectsToReenable, true, "objectWithList.interactibleObjectsToReenable");
			}
			
			objectWithList.interactibleObjectsToReenable = objectsToReenable;
			objectWithList._setLastToggleInactive(this);
			objectWithList.lastToggleActive = this;
		}
		
		if(oneWayActivation)
		{
			toggleIsActive = false;
		}
		
		if(!toggleIsActive)
		{
			if(objectWithList != null) 
			{
				if(objectWithList.objectsToDisable != null && objectWithList.objectsToDisable.GetLength(0) > 0)
				{
					_toggUltimaModifyGameObjectArray(objectWithList.objectsToDisable, false, "objectWithList.objectsToDisable");
				}
				
				if(objectWithList.objectsToReenable != null && objectWithList.objectsToReenable.GetLength(0) > 0)
				{
					_toggUltimaModifyGameObjectArray(objectWithList.objectsToReenable, false, "objectWithList.objectsToReenable");
				}
			}
			
			if(objectsToReenable != null && objectsToReenable.GetLength(0) > 0)
			{
				_toggUltimaModifyGameObjectArray(objectsToReenable, false, "objectsToReenable");
			}
				if(objectsToEnable != null && objectsToEnable.GetLength(0) > 0)
			{
				_toggUltimaModifyGameObjectArray(objectsToEnable, true, "objectsToEnable");
			}
			
			toggleIsActive = true;
			if(objectWithSync != null && !objectWithSync.firstRun)
			{
				objectWithSync._toggUltimaUpdateVariables();
			}
		} else {
			if(objectsToEnable != null && objectsToEnable.GetLength(0) > 0)
			{
				_toggUltimaModifyGameObjectArray(objectsToEnable, false, "objectsToEnable");
			}
			
			if(objectWithList != null) 
			{
				if(objectWithList.objectsToDisable != null && objectWithList.objectsToDisable.GetLength(0) > 0)
				{
					_toggUltimaModifyGameObjectArray(objectWithList.objectsToDisable, false, "objectWithList.objectsToDisable");
				}
				
				if(objectWithList.objectsToReenable != null && objectWithList.objectsToReenable.GetLength(0) > 0)
				{
					_toggUltimaModifyGameObjectArray(objectWithList.objectsToReenable, true, "objectWithList.objectsToReenable");
				}
			}
			
			if(objectsToReenable != null && objectsToReenable.GetLength(0) > 0)
			{
				_toggUltimaModifyGameObjectArray(objectsToReenable, true, "objectsToReenable");
			}
			
			toggleIsActive = false;
			if(objectWithSync != null && !objectWithSync.firstRun)
			{
				objectWithSync._toggUltimaUpdateVariables();
			}
		}
		waitingToActivate = false;
	}
}