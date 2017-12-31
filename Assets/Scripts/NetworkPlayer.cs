using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Bomberman.UI;

namespace Bomberman.Networking {
	
	public class NetworkPlayer : NetworkBehaviour {

		public event Action<NetworkPlayer> syncVarsChanged;

		[SerializeField]
		protected GameObject playerPrefab;
		[SerializeField]
		protected GameObject lobbyPrefab;

		public LobbyPlayer lobbyObject {
			get;
			protected set;
		}

		public int PlayerId {
			get {
				return playerId;
			}
		}

		public PlayerManager Player {
			get {
				return player;
			}
		}

		[SyncVar(hook = "OnNameChanged")]
		public string username = "";

		[SyncVar(hook = "OnInitialize")]
		private bool initialized = false;

		[SyncVar(hook = "OnReady")]
		public bool ready = false;

		private PlayerManager player;
		private int playerId;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void OnEnterLobbyScene() {
			if (initialized) {
				if (lobbyObject == null) {
					CreateLobbyObject ();
				}
			}
		}

		public void OnEnterGameScene() {
			if (hasAuthority) {
				CmdSpawnGameScenePlayer ();
			}
		}

		public override void OnStartClient () {
			DontDestroyOnLoad (this);
	
			NetworkManager.Instance.RegisterPlayer(this);
		}

		public override void OnStartLocalPlayer() {
			// Spawn player Prefab
			base.OnStartLocalPlayer();
			Debug.Log ("Local Player Start");

			Initialize ();
		}

		public override void OnNetworkDestroy ()
		{
			base.OnNetworkDestroy ();

			if (lobbyObject != null) {
				Destroy (lobbyObject.gameObject);
			}

			NetworkManager.Instance.DeregisterPlayer (this);
		}

		private void CreateLobbyObject() {
			lobbyObject = Instantiate (lobbyPrefab).GetComponent<LobbyPlayer> ();
			lobbyObject.Init (this);
		}

		private void Initialize() {
			CmdInitialize ();
		}

		private void OnInitialize(bool value) {
			if (!initialized && value) {
				initialized = value;
				MenuUI.Instance.lobbyChat.SetPlayer (this);
				CreateLobbyObject ();
			}
		}

		private void OnNameChanged(string value) {
			username = value;

			if (syncVarsChanged != null) {
				syncVarsChanged (this);
			}
		}

		private void OnReady(bool value) {
			lobbyObject.SetReady (value);
		}

		public void SendChat(string str) {
			if (isServer) {
				RpcSendChat (str);
			} else {
				CmdSendChat (str);
			}
		}

		public void SetPlayerId(int id) {
			playerId = id;
		}

		[Command]
		public void CmdNameChanged(string str) {
			this.username = str;
		}

		[Command]
		public void CmdSendChat(string str) {
			RpcSendChat (str);
		}

		[Command]
		public void CmdSetReady() {
			ready = !ready;
		}

		[Command]
		private void CmdInitialize() {
			initialized = true;
		}

		[Command]
		private void CmdSpawnGameScenePlayer() {
			Debug.Log ("Spawn Game Scene Player");

			GameObject playerObject = Instantiate (playerPrefab);
			NetworkServer.SpawnWithClientAuthority (playerObject, connectionToClient);

			player = playerObject.GetComponent<PlayerManager> ();
			player.Init (this);
		}

		[ClientRpc]
		private void RpcSendChat(string str) {
			MenuUI.Instance.lobbyChat.AddText(username + ": " + str + "\n");
		}
	}

}