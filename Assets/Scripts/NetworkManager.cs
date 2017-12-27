using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

using Bomberman.Networking;

namespace Bomberman.Networking {

	public class NetworkManager : UnityEngine.Networking.NetworkManager {

		public static NetworkManager Instance {
			get;
			protected set;
		}

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
			// print (AllPlayersReady);
		}

		public void JoinMatch(MatchInfoSnapshot matchInfo) {
			matchMaker.JoinMatch (matchInfo.networkId, string.Empty, string.Empty, string.Empty, 0, 0, OnMatchJoined);
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

	}

}