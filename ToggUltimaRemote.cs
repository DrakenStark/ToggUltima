using UdonSharp;
using UnityEngine;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark

//Version 2.5

public class OnlyOneGroupToggleRemote : UdonSharpBehaviour
{
	[Header("Remote Toggle Object")]	
	[Tooltip("Object To Remotely Toggle:\n- Whenever this object is activated, the object attached here will perform its activation.")]
	[SerializeField] private OnlyOneGroupToggleAction objectToRemotelyInteract;
	[Header("Optional Features")]
	[Tooltip("One Way Activation:\n- Check this box to prevent players from disabling this Toggle via a second interaction.\n- While One Way Activation is enabled, the only way this Toggle may be Deactivated is via another Toggle through an Object With List.")]
	[SerializeField] private bool oneWayActivation = false;
	[Tooltip("Activate On Interact:\n- Check this box to enable the ability for players to directly interact with the object containing this script.")]
	[SerializeField] private bool activateOnInteract = true;
	[Tooltip("Activate On Enable:\n- Check this box to activate this script whenever the object containing it is enabled (upon load or after it had been disabled).")]
	[SerializeField] private bool activateOnEnable = false;
	[Tooltip("Activate On Disable:\n- Check this box to activate this script whenever the object containing it is disabled (will not run without being enabled first).")]
	[SerializeField] private bool activateOnDisable = false;
	
	public override void Interact()
	{
		if(objectToRemotelyInteract != null && activateOnInteract)
		{
			objectToRemotelyInteract.onlyOneToggleTimerCheck();
		} else if(objectToRemotelyInteract == null) {
			Debug.LogWarning("No Object was set or found to remotely activate.", gameObject);
		}
	}
	
	private void OnDisable()
	{
		if(objectToRemotelyInteract != null && activateOnDisable)
		{
			objectToRemotelyInteract.onlyOneToggleTimerCheck();
		} else if(objectToRemotelyInteract == null) {
			Debug.LogWarning("No Object was set or found to remotely activate.", gameObject);
		}
	}
	
	private void OnEnable()
	{
		if(objectToRemotelyInteract != null && activateOnEnable)
		{
			objectToRemotelyInteract.onlyOneToggleTimerCheck();
		} else if(objectToRemotelyInteract == null) {
			Debug.LogWarning("No Object was set or found to remotely activate.", gameObject);
		}
	}
}