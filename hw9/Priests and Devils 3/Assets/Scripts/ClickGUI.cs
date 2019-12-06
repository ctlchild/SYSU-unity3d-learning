using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class ClickGUI : MonoBehaviour {
	UserAction action = null;
	ChaCon character = null;
	BoatCon boat;

	public void setChaCon(ChaCon _character) {
		character = _character;
	}
	public void setBoatCon(BoatCon _boat) {
		boat = _boat;
	}

	void Start() {
		action = Director.getInstance ().currentSceneController as UserAction;
	}
	void OnMouseDown() {
		if (gameObject.name == "boat") {
			action.ClickBoat ();
		} else {
			action.ClickCha (character);
		}
	}
}

