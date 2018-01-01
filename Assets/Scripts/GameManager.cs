using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bomberman.Utilities;
using BombermanNetworkPlayer = Bomberman.Networking.NetworkPlayer;

public class GameManager : Singleton<GameManager> {

	public static int PLAYER_LIVES = 3;

	public List<SpawnPoint> SpawnPoints {
		get {
			return spawnPoints;
		}
	}

	public List<PowerUp> PowerUps {
		get {
			return powerUps;
		}
	}

	public float PowerUpChance {
		get {
			return powerUpChance;
		}
	}

	[SerializeField]
	protected List<SpawnPoint> spawnPoints;

	[SerializeField]
	[Range(0,1f)]
	protected float powerUpChance = 0.33f;

	[SerializeField]
	protected List<PowerUp> powerUps;

	public GameObject explosionCenter, explosionLeft, explosionRight, explosionUp, explosionDown, explosionH, explosionV;

	public void EvaluateVictory(List<BombermanNetworkPlayer> players) {

		int numAlive = players.Count;

		for (int i = 0; i < players.Count; i++) {
			if (players [i].Player.Lives == 0) {
				numAlive--;
			}
		}

		if (numAlive == 0) {
			for (int i = 0; i < players.Count; i++) {
				players [i].Player.RpcSetTie ();
			}
		} else if (numAlive == 1) {
			for (int i = 0; i < players.Count; i++) {
				if (players [i].Player.Lives > 0) {
					players [i].Player.RpcSetVictory ();
				} else {
					players [i].Player.RpcSetLoss ();
				}
			}	
		}
	}
}
