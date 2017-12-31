using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using BombermanNetworkManager = Bomberman.Networking.NetworkManager;
using BombermanNetworkPlayer = Bomberman.Networking.NetworkPlayer;

public class PlayerManager : NetworkBehaviour {

	[SerializeField]
	protected float syncFrequency = 10f;
	[SerializeField]
	protected float deathTime = 2f;
	[SerializeField]
	protected float invincibleTime = 3f;
	[SerializeField]
	protected Color invincibleColor;

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

	public PlayerBomb playerBomb {
		get {
			return GetComponent<PlayerBomb> ();
		}
	}

	public float syncInterval {
		get {
			return 1 / syncFrequency;
		}
	}

	public bool Dead {
		get {
			return dead;
		}
	}

	public int PlayerId {
		get {
			return playerId;
		}
	}

	public int Lives {
		get {
			return lives;
		}
	}

	public bool Invincible {
		get {
			return invincible;
		}
	}

	[SyncVar]
	private Vector2 syncPosition;
	[SyncVar]
	private float syncH;
	[SyncVar]
	private float syncV;
	[SyncVar]
	private int playerId;
	[SyncVar(hook="OnLivesChanged")]
	private int lives;
	[SyncVar(hook="OnDeath")]
	private bool dead;
	[SyncVar(hook="OnInvincible")]
	private bool invincible;

	private float syncTimer = 0f, deathTimer = 0f, invincibleTimer = 0f;
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
		ProcessDeath ();
		ProcessInvincible ();
	}

	public void Init(BombermanNetworkPlayer netPlayer) {
		networkPlayer = netPlayer;
		playerId = networkPlayer.PlayerId;
		lives = GameManager.PLAYER_LIVES;

		spawnPoint = GameManager.Instance.SpawnPoints [networkPlayer.PlayerId].transform.position;

		CmdSetInitialized (spawnPoint);
	}

	public void SpawnBomb(float t) {
		int x = Mathf.RoundToInt(rb.position.x / 0.5f);
		int y = Mathf.RoundToInt(rb.position.y / 0.5f);

		CmdSpawnBomb (new Vector2(x * 0.5f, y * 0.5f), t);
	}

	private void AnimateLocalPlayer() {
		if (!hasAuthority) {
			return;
		}

		SetAnimation (playerMovement.h_Input, playerMovement.v_Input);
	}

	private void ProcessDeath() {
		if (!hasAuthority || !Dead) {
			return;
		}

		deathTimer += Time.deltaTime;

		if (deathTimer >= deathTime) {
			deathTimer = 0f;

			if (lives > 0) {
				CmdResetDeathFlag ();
			}
		}
	}

	private void ProcessInvincible() {
		if (!hasAuthority || !Invincible) {
			return;
		}

		invincibleTimer += Time.deltaTime;

		if (invincibleTimer >= invincibleTime) {
			invincibleTimer = 0f;
			CmdSetInvincible (false);
		}
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

	private void OnDeath(bool value) {
		dead = value;

		if (dead) {
			anim.SetTrigger ("death");
		} else {
			anim.SetTrigger ("revive");
		}
	}

	private void OnLivesChanged(int v) {
		lives = v;
		GameMenu.Instance.UpdateValues (playerId, lives);
	}

	private void OnInvincible(bool v) {
		invincible = v;

		sr.color = invincible ? invincibleColor : Color.white;
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

	[Command]
	private void CmdSpawnBomb(Vector2 position, float t) {
		GameObject bombObj = Instantiate (playerBomb.BombPrefab);
		bombObj.transform.position = position;
		bombObj.GetComponent<Bomb> ().Init (t);

		NetworkServer.SpawnWithClientAuthority (bombObj, networkPlayer.gameObject);
	}

	[Command]
	public void CmdSetDeathFlag() {
		if (dead) {
			return;
		}

		dead = true;
		lives--;

		GameManager.Instance.EvaluateVictory (BombermanNetworkManager.Instance.connectedPlayers);
	}

	[Command]
	private void CmdResetDeathFlag() {
		if (!dead) {
			return;
		}

		dead = false;
		//rb.position = GameManager.Instance.SpawnPoints [Random.Range (0, GameManager.Instance.SpawnPoints.Count)].transform.position;
		CmdSetInvincible(true);
	}

	[Command]
	private void CmdSetInvincible(bool inv) {
		invincible = inv;
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

	[ClientRpc]
	public void RpcSetVictory() {
		if (!hasAuthority) {
			return;
		}

		Debug.Log ("Victory!");
	}

	[ClientRpc]
	public void RpcSetLoss() {
		if (!hasAuthority) {
			return;
		}

		Debug.Log ("You lost!");
	}

	[ClientRpc]
	public void RpcSetTie() {
		if (!hasAuthority) {
			return;
		}

		Debug.Log ("It's a tie!");
	}
}
