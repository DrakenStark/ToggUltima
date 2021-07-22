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

public class ToggUltimaPlayerTrigger : UdonSharpBehaviour
{
	[Header("Tigger Enter Toggle Options")]
	[Tooltip("Object To Remotely Toggle:\n- Whenever this object is activated, the object attached here will perform its activation.")]
	[SerializeField] private ToggUltimaAction _enterObjectToRemotelyInteract = null;
	[SerializeField] private bool _playerEnterOneWayActivation = true;
	[SerializeField] private bool _enterActivationState = true;
	
	[Header("Tigger Exit Toggle Options")]
	[Tooltip("Object To Remotely Toggle:\n- Whenever this object is activated, the object attached here will perform its activation.")]
	[SerializeField] private ToggUltimaAction _exitObjectToRemotelyInteract = null;
	[SerializeField] private bool _playerExitOneWayActivation = true;
	[SerializeField] private bool _exitActivationState = false;
	
	//[Header("Optional Multiplayer Debug Output")]
	//[Tooltip("Debug Text Output:\n- Drag a UI Text object here and it will be updated with function status info from this script.")]
	//[SerializeField] private Text debugTextOutput = null;
	
	
	private void OnPlayerTriggerEnter(VRCPlayerApi player)
	{
		if(_enterObjectToRemotelyInteract != null && player != null && player.isLocal)
		{
			//_toggUltimaDebugLog(_enterObjectToRemotelyInteract.gameObject.name + " false, " + player.isLocal);
			if(_playerEnterOneWayActivation)
			{
				_enterObjectToRemotelyInteract.toggleIsActive = !(_enterActivationState);
			}
			_enterObjectToRemotelyInteract.toggUltimaTimerPartOne();
		}
	}
	
	private void OnPlayerTriggerExit(VRCPlayerApi player)
	{
		if(_exitObjectToRemotelyInteract != null && player != null && player.isLocal)
		{
			//_toggUltimaDebugLog(_exitObjectToRemotelyInteract.gameObject.name + " true, " + player.isLocal);
			if(_playerExitOneWayActivation)
			{
				_exitObjectToRemotelyInteract.toggleIsActive = !(_exitActivationState);
			}
			_exitObjectToRemotelyInteract.toggUltimaTimerPartOne();
		}
	}
	
	
	/*public void _toggUltimaDebugLog(string debugText)
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
	}*/
}