using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bomberman.Networking;

public class PlayerBomb : MonoBehaviour {

	public GameObject BombPrefab {
		get {
			return bombPrefab;
		}
	}

	[SerializeField]
	protected GameObject bombPrefab;
	[SerializeField]
	protected float bombLifeTime = 3f;

	private PlayerManager playerManager;

	// Use this for initialization
	void Start () {
		playerManager = GetComponent<PlayerManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		KeyboardInput ();
	}

	private void KeyboardInput() {
		if (!playerManager.hasAuthority) {
			return;
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			playerManager.SpawnBomb (bombLifeTime);
		}
	}

}
