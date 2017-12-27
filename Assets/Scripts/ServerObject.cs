using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

using Bomberman.Networking;

public class ServerObject : MonoBehaviour {

	[SerializeField]
	protected Text matchName;

	public MatchInfoSnapshot MatchInfo {
		get;
		protected set;
	}

	public void Initialize(MatchInfoSnapshot matchInfo) {
		this.MatchInfo = matchInfo;
		matchName.text = matchInfo.name;
	}

	public void JoinMatch() {
		NetworkManager.Instance.JoinMatch (MatchInfo);
		MenuUI.Instance.ShowLobbyPanel ();
	}
}
