using UdonSharp;
using UnityEngine;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark
//Discord Server: https://discord.gg/ZC4zd3hN5v

//Special Thanks to Texelsaur

//Version 3.4

public class ToggUltimaList : UdonSharpBehaviour
{
	[Header("Toggled Objects")]	
	[Tooltip("Objects To Disable:\n- Whenever an object with ToggUltimaAction using this script as an Object With List, Objects listed here will be Disabled if not directly Enabled by that script.")]
	public GameObject[] objectsToDisable = null;
	[Tooltip("Objects To Reenable:\n- Whenever all objects with ToggUltimaAction are simultaniously Deactivated, Objects listed here will be Enabled.")]
	public GameObject[] objectsToReenable = null;
	[HideInInspector] public GameObject[] interactibleObjectsToReenable = null;
	[HideInInspector] public ToggUltimaAction lastToggleActive = null;
	
	[Header("Optional Features")]
	[Tooltip("Object With Bouncer:\n- Use an object with a ToggUltima Bouncer script here to restrict legitimate use of all attached ToggUltima Action scripts to specific users.\n- Notice: This is not a end all be all to world interaction security and will only keep the players who are not using mods in check. The Bouncer script and its implementation is just a deterrent and will only be developed as such.")]
	[SerializeField] public ToggUltimaBouncer objectWithBouncer = null;
	
	
	public void _setLastToggleInactive(ToggUltimaAction checkActive)
	{
		if(lastToggleActive != null && lastToggleActive != checkActive)
		{
			lastToggleActive.toggleIsActive = false;
		}
	}
	
	
	public void _autoUpdateOTD(GameObject[] arrayToAdd, GameObject sourceActionScript)
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