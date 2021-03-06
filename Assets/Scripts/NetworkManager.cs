﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

using Bomberman.Networking;

namespace Bomberman.Networking {

	public class NetworkManager : UnityEngine.Networking.NetworkManager {

		public static NetworkManager Instance {
			get;
			protected set;
		}

		public static string lobbyScene = "lobby";
		public static string gameScene = "field";

		public bool AllPlayersReady {
			get {

				if (connectedPlayers.Count == 0) {
					return false;
				}

				for (int i = 0; i < connectedPlayers.Count; i++) {
					if (!connectedPlayers [i].ready || connectedPlayers[i].username.Length == 0) {
						return false;
					}
				}

				return true;
			}
		}

		public List<NetworkPlayer> connectedPlayers;

		// Use this for initialization
		void Awake () {

			if (Instance != null) {
				Destroy (gameObject);
			} else {
				Instance = this;
				connectedPlayers = new List<NetworkPlayer> ();
			}
			
		}

		public void RegisterPlayer(NetworkPlayer player) {
			connectedPlayers.Add (player);

			player.OnEnterLobbyScene ();

			UpdatePlayerIDs ();
		}

		public void DeregisterPlayer(NetworkPlayer player) {
			int index = connectedPlayers.IndexOf (player);

			if (index >= 0) {
				connectedPlayers.RemoveAt (index);
			}
		}
	
		public void StartMatchmaking(string gameName) {
			StartMatchMaker();

			matchMaker.CreateMatch (gameName, 4, true, string.Empty, string.Empty, string.Empty, 0, 0, OnMatchCreate);
		}

		public void ListMatches() {
			StartMatchMaker();

			matchMaker.ListMatches (0, 10, string.Empty, true, 0, 0, OnMatchList);
		}

		public void BeginMatch() {
			if (AllPlayersReady) {

				ServerChangeScene (gameScene);
				UnlistMatch ();

				// matchMaker.SetMatchAttributes(matchInfo.networkId, false, 0, (success, info) => Debug.Log("Match hidden"));
			}
		}

		public void BackToMenu() {

			if (Network.isServer) {
				if (matchMaker != null && matchInfo != null) {
					matchMaker.DestroyMatch (matchInfo.networkId, 0, (success, info) => {
						//StopMatchMaker ();
						StopHost ();

						matchInfo = null;
					});
				} else {
					//StopMatchMaker ();
					StopHost ();
				}
			} else {
				if (matchMaker != null && matchInfo != null) {
					matchMaker.DropConnection (matchInfo.networkId, matchInfo.nodeId, 0, (success, info) => {
						StopMatchMaker ();
						StopClient ();

						matchInfo = null;
					});
				} else {
					StopMatchMaker ();
					StopClient ();
				}
			}

			SceneManager.LoadScene (lobbyScene);
		}

		public void JoinMatch(MatchInfoSnapshot matchInfo) {
			matchMaker.JoinMatch (matchInfo.networkId, string.Empty, string.Empty, string.Empty, 0, 0, OnMatchJoined);
		}

		private void UpdatePlayerIDs() {
			for (int i = 0; i < connectedPlayers.Count; i++) {
				connectedPlayers [i].SetPlayerId (i);
			}
		}

		private void UnlistMatch () {
			if (matchMaker != null) {
				matchMaker.SetMatchAttributes (matchInfo.networkId, false, 0, OnSetMatchAttributes);
			}
		}

		public override void OnStopServer ()
		{
			base.OnStopServer ();

			Debug.Log ("StopServer");

			for (int i = 0; i < connectedPlayers.Count; i++) {
				NetworkPlayer player = connectedPlayers [i];

				if (player != null) {
					NetworkServer.Destroy (player.gameObject);
				}
			}

			connectedPlayers.Clear ();
		}

		public override void OnStopClient ()
		{
			base.OnStopClient ();

			Debug.Log ("StopClient");

			for (int i = 0; i < connectedPlayers.Count; i++) {
				NetworkPlayer player = connectedPlayers [i];

				if (player != null) {
					Destroy (player.gameObject);
				}
			}

			connectedPlayers.Clear ();
		}

		public override void OnMatchCreate (bool success, string extendedInfo, MatchInfo matchInfo)
		{
			base.OnMatchCreate (success, extendedInfo, matchInfo);

			MenuUI.Instance.ShowLobbyPanel ();
		}

		public override void OnMatchList (bool success, string extendedInfo, List<UnityEngine.Networking.Match.MatchInfoSnapshot> matchList)
		{
			base.OnMatchList (success, extendedInfo, matchList);

			MenuUI.Instance.PopulateServerList (matchList);

		}

		public override void OnMatchJoined (bool success, string extendedInfo, MatchInfo matchInfo)
		{
			base.OnMatchJoined (success, extendedInfo, matchInfo);

			print (matchInfo.networkId.ToString() + " joined");
		}

		public override void OnClientConnect (NetworkConnection conn)
		{
			// base.OnClientConnect (conn);

			Debug.Log ("OnClientConnect");

			ClientScene.Ready (conn);
			ClientScene.AddPlayer (0);

		}

		public override void OnClientDisconnect (NetworkConnection conn)
		{
			Debug.Log ("OnClientDisconnect");

			base.OnClientDisconnect (conn);
		}

		public override void OnClientSceneChanged (NetworkConnection conn)
		{
			Debug.Log ("Scene Changed");

			base.OnClientSceneChanged (conn);

			string sceneName = SceneManager.GetActiveScene ().name;

			if (sceneName == gameScene) {
				for (int i = 0; i < connectedPlayers.Count; i++) {
					NetworkPlayer np = connectedPlayers [i];
					np.OnEnterGameScene ();
				}

				GameMenu.Instance.Init ();
			} else if (sceneName == lobbyScene) {
				for (int i = 0; i < connectedPlayers.Count; i++) {
					NetworkPlayer np = connectedPlayers [i];
					np.OnEnterLobbyScene ();
				}
			}
		}

	}

}