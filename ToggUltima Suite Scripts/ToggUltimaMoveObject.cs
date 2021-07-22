using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

//By DrakenStark
//Discord: Draken Stark#2888
//Twitter & Telegram: @DrakenStark
//Discord Server: https://discord.gg/ZC4zd3hN5v

//Version 3.4

public class ToggUltimaMoveObject : UdonSharpBehaviour
{
	[SerializeField] private GameObject _gameObjectToMove = null;
	[SerializeField] private GameObject _objectMoveLocation = null;
	[SerializeField] private bool _includeRotation = true;
	
	
	private void OnEnable()
	{
		_moveObject();
		gameObject.SetActive(false);
	}
	
	
	private void _moveObject()
	{
		if(_gameObjectToMove != null && _objectMoveLocation != null)
		{	
			_gameObjectToMove.transform.position = _objectMoveLocation.transform.position;
			if(_includeRotation)
			{
				_gameObjectToMove.transform.rotation = _objectMoveLocation.transform.rotation;
			}
		}
	}
}