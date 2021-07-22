using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark
//Discord Server: https://discord.gg/ZC4zd3hN5v

//Version 3.4

public class ToggUltimaCollision : UdonSharpBehaviour
{
	[Header("Remote Toggle Object")]	
	[Tooltip("Object To Remotely Toggle:\n- Whenever this object is activated, the object attached here will perform its activation.")]
	[SerializeField] private ToggUltimaAction _objectToRemotelyInteract = null;
	[Header("Collision Toggle Options")]
	[SerializeField] private GameObject[] _allowedObjects = null;
	[SerializeField] private bool _activateOnEnter = true;
	[SerializeField] private bool _activateOnExit = false;
	[SerializeField] private bool _activateOnEnable = false;
	
	
	private void OnEnable()
	{
		if(_activateOnEnable)
		{
			_remotelyActivateAction();
			gameObject.SetActive(false);
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if(_activateOnEnter && other != null)
		{
			_checkCollisionObject(other.gameObject);
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		if(_activateOnExit && other != null)
		{
			_checkCollisionObject(other.gameObject);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if(_activateOnEnter && other != null)
		{
			_checkCollisionObject(other.gameObject);
		}
	}
	
	private void OnCollisionExit(Collision other)
	{
		if(_activateOnExit && other != null)
		{
			_checkCollisionObject(other.gameObject);
		}
	}
	
	
	private void _checkCollisionObject(GameObject collidedObject)
	{
		bool objectFound = false;
		if(_allowedObjects != null)
		{
			if(_allowedObjects.GetLength(0) < 1)
			{
				objectFound = true;
			} else {
				for(int arrayIndex = 0; arrayIndex < _allowedObjects.GetLength(0); arrayIndex++)
				{
					if(_allowedObjects[arrayIndex] == collidedObject)
					{
						objectFound = true;
					}
				}
			}
		} else {
			objectFound = true;
		}
		
		if(objectFound)
		{
			_remotelyActivateAction();
		}
	}
		
	private void _remotelyActivateAction()
	{
		if(_objectToRemotelyInteract != null)
		{
			_objectToRemotelyInteract.toggUltimaTimerPartOne();
		} else {
			Debug.LogWarning("No Object was set or found to remotely activate.", gameObject);
		}
	}
}