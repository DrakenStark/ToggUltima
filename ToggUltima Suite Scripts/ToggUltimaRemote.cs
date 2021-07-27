using UdonSharp;
using UnityEngine;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark
//Discord Server: https://discord.gg/ZC4zd3hN5v

//Version 3.5

public class ToggUltimaRemote : UdonSharpBehaviour
{
	[Header("Remote Toggle Object")]	
	[Tooltip("Object To Remotely Toggle:\n- Whenever this object is activated, the object attached here will perform its activation.")]
	[SerializeField] private ToggUltimaAction objectToRemotelyInteract = null;
	[Header("Optional Features")]
	[Tooltip("Activate On Interact:\n- Check this box to enable the ability for players to directly interact with the object containing this script.")]
	[SerializeField] private bool _activateOnInteract = true;
	[Tooltip("Activate On Enable:\n- Check this box to activate this script whenever the object containing it is enabled (upon load or after it had been disabled).")]
	[SerializeField] private bool _activateOnEnable = false;
	[Tooltip("Activate On Disable:\n- Check this box to activate this script whenever the object containing it is disabled (will not run without being enabled first).")]
	[SerializeField] private bool _activateOnDisable = false;
	
	
	private void OnEnable()
	{
		if(!activateOnInteract) {
			this.DisableInteractive = true;
		}
		
		_activationCheck(_activateOnEnable);
	}
	
	public override void Interact()
	{
		_activationCheck(_activateOnInteract);
	}
	
	private void OnDisable()
	{
		_activationCheck(_activateOnDisable);
	}
	
	
	
	private void _activationCheck(bool _activationMethod)
	{
		if(objectToRemotelyInteract != null)
		{
			if(_activationMethod != null && _activationMethod)
			{
				objectToRemotelyInteract.toggUltimaTimerPartOne();
			}
			return;
		} else {
			Debug.LogWarning("No Object was set or found to remotely activate.", gameObject);
		}
	}
}