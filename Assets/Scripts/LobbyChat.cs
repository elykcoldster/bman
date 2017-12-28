using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using BombermanNetworkPlayer = Bomberman.Networking.NetworkPlayer;

public class LobbyChat : MonoBehaviour {

	[SerializeField]
	public Text chatText;
	[SerializeField]
	public InputField chatInput;

	public int numLines = 10;

	private List<string> chatLines;

	private BombermanNetworkPlayer networkPlayer;

	public void SetPlayer(BombermanNetworkPlayer player) {
		
		if (player.hasAuthority) {
			networkPlayer = player;
		}
	}

	public void OnSendClick() {
		if (networkPlayer.username.Length > 0 && chatInput.text.Length > 0) {
			networkPlayer.SendChat (chatInput.text);
			chatInput.text = "";
		}
	}

	public void AddText(string text) {
		chatText.text += text;
	}

}
