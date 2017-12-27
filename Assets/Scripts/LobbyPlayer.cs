using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BombermanNetworkPlayer = Bomberman.Networking.NetworkPlayer;

namespace Bomberman.UI {
	
	public class LobbyPlayer : MonoBehaviour {

		[SerializeField]
		protected InputField nameInput;

		[SerializeField]
		protected Button readyButton;

		[SerializeField]
		Color notReadyColor;
		[SerializeField]
		Color readyColor;

		private BombermanNetworkPlayer m_networkPlayer;

		public void Init(BombermanNetworkPlayer networkPlayer) {

			m_networkPlayer = networkPlayer;

			m_networkPlayer.syncVarsChanged += OnSyncVarsChanged;

			readyButton.image.color = notReadyColor;

			MenuUI.Instance.playerList.AddPlayer (this);

			if (!m_networkPlayer.hasAuthority) {
				nameInput.interactable = false;
				readyButton.interactable = false;
			} else {
				nameInput.onEndEdit.AddListener (OnNameChanged);
				readyButton.onClick.AddListener (OnReadyClick);
			}
		}

		public void SetReady(bool r) {
			readyButton.image.color = r ? readyColor : notReadyColor;
		}

		private void OnReadyClick() {
			m_networkPlayer.CmdSetReady ();
		}

		private void UpdateValues () {
			nameInput.text = m_networkPlayer.username;
		}

		private void OnNameChanged(string str) {
			m_networkPlayer.CmdNameChanged (str);
		}

		private void OnSyncVarsChanged(BombermanNetworkPlayer player) {
			UpdateValues();
		}
	}

}