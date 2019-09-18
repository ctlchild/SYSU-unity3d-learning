using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class FirstController : MonoBehaviour, SceneController, UserAction {
	UserGUI userGUI;
	public CoastCon fromCoast;
	public CoastCon toCoast;
	public BoatCon boat;
	private ChaCon[] characters;

	void Awake() {
		Director director = Director.getInstance ();
		director.currentSceneController = this;
		userGUI = gameObject.AddComponent <UserGUI>() as UserGUI;
		characters = new ChaCon[6];
		loadResources();
		loadCharacter();
	}

	public void loadResources() {
		Vector3 river_pos = new Vector3(0,0.5F,0);
		GameObject river = Instantiate (Resources.Load ("Perfabs/River", typeof(GameObject)), river_pos, Quaternion.identity, null) as GameObject;
		river.name = "river";
		fromCoast = new CoastCon ("from");
		toCoast = new CoastCon ("to");
		boat = new BoatCon ();
	}

	public void loadCharacter() {
		for (int i = 0; i < 3; i++) {
			ChaCon cha = new ChaCon ("priest");
			cha.setName("priest" + i);
			cha.setPosition (fromCoast.getEmptyPosition ());
			cha.getOnCoast (fromCoast);
			fromCoast.getOnCoast (cha);
			characters [i] = cha;
		}
		for (int i = 0; i < 3; i++) {
			ChaCon cha = new ChaCon ("devil");
			cha.setName("devil" + i);
			cha.setPosition (fromCoast.getEmptyPosition ());
			cha.getOnCoast (fromCoast);
			fromCoast.getOnCoast (cha);
			characters [i+3] = cha;
		}
	}

	public void ClickBoat() {
		if (boat.isEmpty ()) return;
		boat.Move ();
		userGUI.status = check();
	}

	public void ClickCha(ChaCon cha) {
		if (cha.isOnBoat ()) {
			CoastCon nowCoast;
			if (boat.getStatus () == -1) nowCoast = toCoast; // to->-1; from->1	
			else nowCoast = fromCoast;
			boat.getOffBoat (cha.getName());
			cha.moveToPosition (nowCoast.getEmptyPosition ());
			cha.getOnCoast (nowCoast);
			nowCoast.getOnCoast (cha);
		} 
		else {									// character on coast
			CoastCon nowCoast = cha.getCoastCon ();
			if (boat.getEmptyIndex () == -1) return; // boat is full
			if (nowCoast.getStatus () != boat.getStatus ())	return; // boat is not on the side of character
			nowCoast.getOffCoast(cha.getName());
			cha.moveToPosition (boat.getEmptyPosition());
			cha.getOnBoat (boat);
			boat.getOnBoat (cha);
		}
		userGUI.status = check();
	}

	int check() {	// 0->not finish, 1->lose, 2->win
		int fromP = 0, fromD = 0, toP = 0, toD = 0;
		int[] fromCount = fromCoast.getCharacterNum();
		fromP += fromCount[0];
		fromD += fromCount[1];

		int[] toCount = toCoast.getCharacterNum ();
		toP += toCount[0];
		toD += toCount[1];

		if (toP + toD == 6) return 2; //win
		int[] boatCount = boat.getCharacterNum ();
		if (boat.getStatus () == -1) {
			toP += boatCount[0]; toD += boatCount[1];	// boat at toCoast
		}
		else {
			fromP += boatCount[0]; fromD += boatCount[1];	// boat at fromCoast	
		}
		if (fromP < fromD && fromP > 0) return 1; //lose		
		if (toP < toD && toP > 0) return 1; //lose
		return 0;			// not finish
	}

	public void restart() {
		boat.reset ();
		fromCoast.reset ();
		toCoast.reset ();
		for (int i = 0; i < characters.Length; i++) characters [i].reset ();
	}
}
