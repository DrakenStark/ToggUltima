using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark

//Special Thanks to Zephyxus

//Version 2.5

public class OnlyOneGroupToggleSync : UdonSharpBehaviour
{
	[SerializeField] private GameObject[] toggleActionScripts;
	private bool[] toggleActionScriptStates;
	private bool[] toggleCatchUpToState;


	public void toggleActiveSync(GameObject actionScriptObject, bool toggleState, bool afterToggling)
	{
		
	}
	
	public override void OnPlayerJoined(VRCPlayerApi player)
	{
		SendCustomNetworkEvent(NetworkEventTarget.player, "syncJoinedPlayer");
	}
	
	private void syncJoinedPlayer()
	{
		
	}
}