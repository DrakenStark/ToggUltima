using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark
//Discord Server: https://discord.gg/ZC4zd3hN5v

//Version 3.4

public class ToggUltimaRespawnObject : UdonSharpBehaviour
{
	[Header("The GameObject of this script must be enabled by default.")]	
	public GameObject _gameObjectToRespawn = null;
	[SerializeField] private bool _includeExistingRotation = false;
	public Vector3 _originalPosition = new Vector3 (0f, 0f, 0f);
	public Quaternion _setToRotation = new Quaternion (0f, 0f, 0f, 0f);
	private bool _firstRun = true;
	
	private void Start()
	{
		Debug.Log("I'm Alive!");
		gameObject.SetActive(true);
	}
	
	private void OnEnable()
	{
		if(!_firstRun)
		{
			_moveObject();
			gameObject.SetActive(false);
		} else {
			_firstRun = false;
			if(_gameObjectToRespawn != null)
			{
				_originalPosition = _gameObjectToRespawn.transform.position;
				_setToRotation = _gameObjectToRespawn.transform.rotation;
			}
			gameObject.SetActive(false);
		}
	}
	
	
	private void _moveObject()
	{
		if(_gameObjectToRespawn != null)
		{	
			if(_includeExistingRotation)
			{
				_setToRotation = _gameObjectToRespawn.transform.rotation;
			}
			_gameObjectToRespawn.transform.position = _originalPosition;
			_gameObjectToRespawn.transform.rotation = _setToRotation;
		} else {
			Debug.LogWarning("Object to respawn is missing", gameObject);
		}
	}
}