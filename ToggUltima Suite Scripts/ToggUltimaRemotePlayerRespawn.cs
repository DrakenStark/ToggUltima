using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark

//Version 3.4

public class ToggUltimaRemotePlayerRespawn : UdonSharpBehaviour
{
	[Header("Remote Toggle Object")]	
	[Tooltip("Object To Remotely Toggle:\n- Whenever this object is activated, the object attached here will perform its activation.")]
	[SerializeField] private ToggUltimaAction objectToRemotelyInteract = null;
	
	
	public override void OnPlayerRespawn(VRCPlayerApi player)
	{
		if(Networking.LocalPlayer == player && objectToRemotelyInteract != null && gameObject.activeSelf && objectToRemotelyInteract.enabled)
		{
			objectToRemotelyInteract.toggUltimaTimerPartOne();
		} else {
			Debug.LogWarning("No Object was set or found to remotely activate.", gameObject);
		}
	}
}