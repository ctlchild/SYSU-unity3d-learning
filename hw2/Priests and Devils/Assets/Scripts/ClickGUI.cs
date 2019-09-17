using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class ClickGUI : MonoBehaviour {
	UserAction action;
	ChaCon characterController;

	public void setController(ChaCon characterCtrl) {
		characterController = characterCtrl;
	}

	void Start() {
		action = Director.getInstance ().currentSceneController as UserAction;
	}

	void OnMouseDown() {
		if (gameObject.name == "boat") {
			action.ClickBoat ();
		} else {
			action.ClickCha (characterController);
		}
	}
}
