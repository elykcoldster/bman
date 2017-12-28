using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bomberman.Utilities;

public class GameManager : Singleton<GameManager> {

	public List<SpawnPoint> SpawnPoints {
		get {
			return spawnPoints;
		}
	}

	[SerializeField]
	protected List<SpawnPoint> spawnPoints;


}
