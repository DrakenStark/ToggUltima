using UdonSharp;
using UnityEngine;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark

//Special Thanks to ArtySilvers

//Version 2.5

public class OnlyOneGroupToggleAction : UdonSharpBehaviour
{
	[Header("List Object")]
	[Tooltip("Object With List:\n- An object that has an OnlyOneGroupToggleList script may be dragged here to support mutual exclusivity.\n- The OnlyOneGroupToggleList must be configured to disable all objects you wish to have mutually exclusive.\n-All OnlyOneGroupToggleAction scripts using the same Object With List will be mutually exclusive with each other.")]
	[SerializeField] private OnlyOneGroupToggleList objectWithList;
	[Tooltip("Auto Add OTE To OTD:\n- Automatically adds the Objects To Enable to the Object With List's Objects To Disable when the object this script is in is loaded or enabled for the first time.")]
	[SerializeField] private bool autoAddOTEToOTD = false;
	private bool toggleListUpdated = false;
	[Header("Multiplayer Sync Object")]
	[Tooltip("Object With Sync:\n- An object that has an OnlyOneGroupToggleSync script may be dragged here to support keeping this script synced up with all players.")]
	[SerializeField] private OnlyOneGroupToggleSync objectWithSync;
	[Header("Toggled Objects")]
	[Tooltip("Objects To Enable:\n- Drag Objects here to be Enabled while this Toggle is Active via interaction with by a player.\n- To be interacted with, the object with this script must have a collider!")]
	[SerializeField] private GameObject[] objectsToEnable;
	[Tooltip("Objects To Enable:\n- Drag Objects here to be Disabled while this Toggle is Active via interaction with by a player.\n- Upon this Toggle being Deactivated via player interaction or another Toggle being Activated, these objects will be Enabled.\n- If you want objects that will stay disabled after this Toggle being Activated, include them into the Disabled Objects section of an Object With List.\n- To be interacted with, the object with this script must have a collider!")]
	[SerializeField] private GameObject[] objectsToReenable;
	[HideInInspector] public bool toggleActive = false;
	[Header("Optional Features")]
	[Tooltip("One Way Activation:\n- Check this box to prevent players from disabling this Toggle via a second interaction.\n- While One Way Activation is enabled, the only way this Toggle may be Deactivated is via another Toggle through an Object With List.")]
	[SerializeField] private bool oneWayActivation = false;
	[Tooltip("Activate On Interact:\n- Check this box to enable the ability for players to directly interact with the object containing this script.")]
	[SerializeField] private bool activateOnInteract = true;
	[Tooltip("Activate On Enable:\n- Check this box to activate this script whenever the object containing it is enabled (upon load or after it had been disabled).")]
	[SerializeField] private bool activateOnEnable = false;
	[Tooltip("Activate On Disable:\n- Check this box to activate this script whenever the object containing it is disabled (will not run without being enabled first).")]
	[SerializeField] private bool activateOnDisable = false;
	[Tooltip("Activation Delay:\n- If set to 0, this feature, Ignore While Waiting, and Reset Timer On Activate will be ignored and the script will always activate immediately when activated.\n- If set to anything above 0, whenever this script is activated, it will wait that many seconds before toggling objects.")]
	[SerializeField] private float activationDelay = 0f;
	[Tooltip("Ignore While Waiting:\n- This will be ignored if Activation Delay is set to 0.\n- Cannot be used simultaniously with Reset Timer On Activate.\n- Once activated and while waiting, any activations will be ignored until after the time is up and objects are toggled.")]
	[SerializeField] private bool ignoreWhileWaiting = true;
	private bool hasBeenActivated = false;
	[Tooltip("Reset Timer On Activate:\n- This will be ignored if Activation Delay is set to 0.\n- Cannot be used simultaniously with Ignore While Waiting.\n- Once activated and while waiting, any activation will reset the delay for when the objects will be toggled.")]
	[SerializeField] private bool resetTimerOnActivate = false;
	private int timesActivated = 0;
	
	
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
			onlyOneToggleTimerCheck();
		}
	}
	
	private void OnDisable()
	{
		if(activateOnDisable)
		{
			onlyOneToggleTimerCheck();
		}
	}
	
	//The OnEnable function here not only activates the main function, but also can check if the Objects to Enable have been added to the List Object.
	private void OnEnable()
	{
		if(!toggleListUpdated)
		{
			toggleListUpdated = true;
			if(objectWithList != null && objectsToEnable.GetLength(0) > 0)
			{
				objectWithList.autoUpdateOTD(objectsToEnable, gameObject);
			}
		}
		
		if(activateOnEnable)
		{
			onlyOneToggleTimerCheck();
		}
	}
	
	//This function is run to determine if there's a waiting period before running the object toggles.
	public void onlyOneToggleTimerCheck()
	{
		if(activationDelay > 0)
		{
			if(ignoreWhileWaiting && !hasBeenActivated)
			{
				timesActivated++;
				hasBeenActivated = true;
				if(objectWithSync != null)
				{
					//objectWithSync.toggleActiveSync(gameObject, toggleActive, false);
				}
				SendCustomEventDelayedSeconds("_onlyOneToggleActivate", activationDelay);
			} else if(!ignoreWhileWaiting) {
				timesActivated++;
				hasBeenActivated = true;
				if(objectWithSync != null)
				{
					//objectWithSync.toggleActiveSync(gameObject, toggleActive, false);
				}
				SendCustomEventDelayedSeconds("_onlyOneToggleActivate", activationDelay);
			}
		} else {
			timesActivated++;
			hasBeenActivated = true;
			if(objectWithSync != null)
			{
				//objectWithSync.toggleActiveSync(gameObject, toggleActive, false);
			}
			SendCustomEventDelayedSeconds("_onlyOneToggleActivate", activationDelay);
		}
		//Debug.Log(timesActivated);
	}

	
	public void _onlyOneToggleActivate()
	{
		if(!(activationDelay > 0) || ((!ignoreWhileWaiting || hasBeenActivated) && (!resetTimerOnActivate || timesActivated < 2)))
		{
			if(objectWithList != null)
			{
				if(objectWithList.interactibleObjectsToReenable != null && objectWithList.interactibleObjectsToReenable.GetLength(0) > 0)
				{
					_toggUltimaModifyGameObjectArray(objectWithList.interactibleObjectsToReenable, true, "objectWithList.interactibleObjectsToReenable");
				}
				
				objectWithList.interactibleObjectsToReenable = objectsToReenable;
				objectWithList.setLastToggleInactive(this);
				objectWithList.lastToggleActive = this;
			}
			
			if(oneWayActivation)
			{
				toggleActive = false;
			}
			
			if(!toggleActive)
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
				
				toggleActive = true;
				if(objectWithSync != null)
				{
					//objectWithSync.toggleActiveSync(gameObject, toggleActive, true);
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
				
				toggleActive = false;
				if(objectWithSync != null)
				{
					//objectWithSync.toggleActiveSync(gameObject, toggleActive, true);
				}
			}
			hasBeenActivated = false;
		}
		if(timesActivated > 0)
		{
			timesActivated--;
			//Debug.Log(timesActivated);
		}
	}
}