using UdonSharp;
using UnityEngine;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark

//Version 2.5

public class OnlyOneGroupToggleList : UdonSharpBehaviour
{
	[Header("Toggled Objects")]	
	[Tooltip("Objects To Disable:\n- Whenever an object with OnlyOneGroupToggleAction using this script as an Object With List, Objects listed here will be Disabled if not directly Enabled by that script.")]
	public GameObject[] objectsToDisable;
	[Tooltip("Objects To Reenable:\n- Whenever all objects with OnlyOneGroupToggleAction are simultaniously Deactivated, Objects listed here will be Enabled.")]
	public GameObject[] objectsToReenable;
	[HideInInspector] public GameObject[] interactibleObjectsToReenable;
	[HideInInspector] public OnlyOneGroupToggleAction lastToggleActive;
	
	public void setLastToggleInactive(OnlyOneGroupToggleAction checkActive)
	{
		if(lastToggleActive != null && lastToggleActive != checkActive)
		{
			lastToggleActive.toggleActive = false;
		}
	}
	
	public void autoUpdateOTD(GameObject[] arrayToAdd, GameObject sourceActionScript)
	{
		if(arrayToAdd != null && arrayToAdd.GetLength(0) > 0)
		{
			GameObject[] mergeGroup = new GameObject[(objectsToDisable.GetLength(0) + arrayToAdd.GetLength(0))];
			for(int gONumber = 0; gONumber < objectsToDisable.GetLength(0); gONumber++)
			{
				if(objectsToDisable[gONumber] != null)
				{
					mergeGroup[gONumber] = objectsToDisable[gONumber];
				} else {
					Debug.LogWarning("Missing Object was found (and safely ignored) in Objects To Disable.", gameObject);
				}
			}
			for(int gONumber = 0; gONumber < arrayToAdd.GetLength(0); gONumber++)
			{
				if(arrayToAdd[gONumber] != null)
				{
					mergeGroup[(gONumber + objectsToDisable.GetLength(0))] = arrayToAdd[gONumber];
				} else {
					Debug.LogWarning("Missing Object was found (and safely ignored) while trying to add to Objects To Disable.", sourceActionScript);
				}
			}
			objectsToDisable = mergeGroup;
		}
	}
}