using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using BombermanNetworkPlayer = Bomberman.Networking.NetworkPlayer;

public class PlayerManager : NetworkBehaviour {

	[SerializeField]
	protected float syncFrequency = 10f;

	public BombermanNetworkPlayer networkPlayer {
		get;
		protected set;
	}

	public PlayerMovement playerMovement {
		get {
			return GetComponent<PlayerMovement> ();
		}
	}

	public Rigidbody2D rb {
		get {
			return GetComponent<Rigidbody2D> ();
		}
	}

	public Animator anim {
		get {
			return GetComponent<Animator> ();
		}
	}

	public SpriteRenderer sr {
		get {
			return GetComponent<SpriteRenderer> ();
		}
	}

	public float syncInterval {
		get {
			return 1 / syncFrequency;
		}
	}

	[SyncVar]
	private Vector2 syncPosition;
	[SyncVar]
	private float syncH;
	[SyncVar]
	private float syncV;

	private float syncTimer = 0f;
	private Vector2 spawnPoint;

	[SyncVar(hook="OnInitialized")]
	private bool hasInitialized = false;
	
	// Update is called once per frame
	void Update () {
		if (!hasInitialized) {
			return;
		}

		AnimateLocalPlayer ();
		Sync ();
	}

	public void Init(BombermanNetworkPlayer netPlayer) {
		networkPlayer = netPlayer;

		spawnPoint = GameManager.Instance.SpawnPoints [networkPlayer.PlayerId].transform.position;

		CmdSetInitialized (spawnPoint);
	}

	private void AnimateLocalPlayer() {
		if (!hasAuthority) {
			return;
		}

		SetAnimation (playerMovement.h_Input, playerMovement.v_Input);
	}

	private void Sync() {

		if (!hasAuthority) {
			ProcessRemoteData ();
		} else {
			SendRemoteData ();
		}
	}

	private void SendRemoteData() {
		if (syncTimer >= syncInterval) {
			SyncRB (rb.position);
			SyncAnimation (playerMovement.h_Input, playerMovement.v_Input);
			syncTimer = 0f;
		}

		syncTimer += Time.deltaTime;
	}

	private void ProcessRemoteData() {
		rb.position = Vector2.Lerp (rb.position, syncPosition, 2 * syncFrequency * Time.deltaTime);
		SetAnimation (syncH, syncV);
	}

	private void SyncRB(Vector2 position) {
		if (isServer) {
			RpcSyncRB (position);
		} else {
			CmdSyncRB (position);
		}
	}

	private void SyncAnimation(float h, float v) {
		if (isServer) {
			RpcSyncAnimation (h, v);
		} else {
			CmdSyncAnimation (h, v);
		}
	}

	private void OnInitialized(bool value) {
		hasInitialized = value;
	}

	private void SetAnimation(float h, float v) {
		anim.SetBool ("down", v < 0f);
		anim.SetBool ("up", v > 0f);
		anim.SetBool ("moving", v != 0f || h != 0f);
		anim.SetBool ("horizontal", h != 0f);

		sr.flipX = h != 0f ? (h > 0f ? true : false) : sr.flipX;
	}

	[Command]
	private void CmdSyncRB(Vector2 position) {
		syncPosition = position;
	}

	[Command]
	private void CmdSyncAnimation(float h, float v) {
		syncH = h;
		syncV = v;
	}

	[Command]
	private void CmdSetInitialized(Vector2 spawn) {
		syncPosition = spawn;
		rb.position = spawn;
		RpcSetInitialized (spawn);
	}

	[ClientRpc]
	private void RpcSyncRB(Vector2 position) {
		syncPosition = position;
	}

	[ClientRpc]
	private void RpcSetInitialized(Vector2 spawn) {
		syncPosition = spawn;
		rb.position = spawn;
		hasInitialized = true;

		// Debug.Log ("Player " + networkPlayer.PlayerId.ToString () + " Initialized");
	}

	[ClientRpc]
	private void RpcSyncAnimation(float h, float v) {
		syncH = h;
		syncV = v;
	}
}
