using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Bomberman.Networking;
using Bomberman.UI;
using Bomberman.Utilities;

public class MenuUI : Singleton<MenuUI> {

	public static string[] USER_NAMES = { "Jack", "Michael" };

	[SerializeField]
	protected CanvasGroup serverList;
	[SerializeField]
	protected CanvasGroup lobbyPanel;
	[SerializeField]
	public LobbyChat lobbyChat;
	[SerializeField]
	public PlayerList playerList;
	[SerializeField]
	protected GameObject serverListPrefab;
	[SerializeField]
	protected InputField username;
	[SerializeField]
	protected InputField gameName;

	public string Username {
		get {
			return username.text;
		}
	}

	List<ServerObject> serverListObjects;

	// Use this for initialization
	void Start () {
		serverListObjects = new List<ServerObject> ();
	}

	public void OnHostClicked() {
		string hostName = gameName.text;
		if (hostName.Length == 0) {
			hostName = "default";
		}
		NetworkManager.Instance.StartMatchmaking (hostName);
	}

	public void OnSearchClicked() {
		NetworkManager.Instance.ListMatches ();
	}

	public void PopulateServerList(List<UnityEngine.Networking.Match.MatchInfoSnapshot> matchList) {

		foreach (ServerObject so in serverListObjects) {
			Destroy (so.gameObject);
		}

		serverListObjects = new List<ServerObject> ();

		for (int i = 0; i < matchList.Count; i++) {
			GameObject serverObj = (GameObject)Instantiate (serverListPrefab);
			serverObj.transform.SetParent (serverList.transform);

			ServerObject so = serverObj.GetComponent<ServerObject> ();
			so.Initialize (matchList [i]);

			serverListObjects.Add (so);
		}
	}

	public void ShowLobbyPanel() {
		lobbyPanel.gameObject.SetActive (true);
	}
		
}
