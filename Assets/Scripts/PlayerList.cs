using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bomberman.UI;

public class PlayerList : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddPlayer(LobbyPlayer player) {
		player.transform.SetParent (this.transform);
	}
}
