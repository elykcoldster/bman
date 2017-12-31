using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Bomberman.Utilities;
using BombermanNetworkManager = Bomberman.Networking.NetworkManager;

public class GameMenu : Singleton<GameMenu> {

	[SerializeField]
	protected GameObject gameMenuPanel;
	[SerializeField]
	protected CanvasGroup scoreBoard;
	[SerializeField]
	protected Text playerScorePrefab;

	public List<int> Scores {
		get {
			return this.scores;
		}
	}

	private List<int> scores;
	private List<Text> scoreTexts;
	private bool hasInitialized = false;

	private BombermanNetworkManager networkManager;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			// gameMenuPanel.SetActive (!gameMenuPanel.activeSelf);
		}
	}

	public void Init() {
		networkManager = BombermanNetworkManager.Instance;

		scores = new List<int> ();
		scoreTexts = new List<Text> ();

		for (int i = 0; i < networkManager.connectedPlayers.Count; i++) {
			Text score = Instantiate (playerScorePrefab).GetComponent<Text> ();
			score.transform.SetParent (scoreBoard.transform);

			scoreTexts.Add (score);

			score.text = networkManager.connectedPlayers [i].username + ": " + GameManager.PLAYER_LIVES.ToString();
			scores.Add (0);
		}

		hasInitialized = true;
	}

	public void GainPoint(int id) {
		scores [id]++;
		UpdateScores ();
	}

	public void UpdateValues(int id, int lives) {
		if (!hasInitialized) {
			return;
		}

		scoreTexts[id].text = networkManager.connectedPlayers [id].username + ": " + lives.ToString ();

	}

	private void UpdateScores() {
		for (int i = 0; i < scoreTexts.Count; i++) {
			scoreTexts [i].text = networkManager.connectedPlayers [i].username + ": " + scores [i].ToString ();
		}
	}
}
