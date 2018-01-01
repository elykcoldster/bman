using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

	public float MoveSpeed {
		get {
			return moveSpeed;
		}
	}

	[SerializeField]
	[SyncVar]
	protected float moveSpeed = 2f;

	private PlayerManager playerManager;

	public float h_Input, v_Input;

	void Awake () {
		playerManager = GetComponent<PlayerManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		KeyboardInput ();
	}

	private void KeyboardInput() {
		if (!playerManager.hasAuthority || playerManager.Dead) {
			playerManager.rb.velocity = Vector2.zero;
			return;
		}

		h_Input = Input.GetAxis ("Horizontal");
		v_Input = Input.GetAxis ("Vertical");

		playerManager.rb.velocity = (Vector2.right * h_Input + Vector2.up * v_Input).normalized * moveSpeed;

	}

	public void SetMoveSpeed(float m) {
		moveSpeed = m;
	}
}
