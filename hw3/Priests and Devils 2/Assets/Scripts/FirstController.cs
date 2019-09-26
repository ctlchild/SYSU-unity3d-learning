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
	public MySceneActionManager actionManager;
	public Judger judger;

	void Awake() {
		Director director = Director.getInstance ();
		director.currentSceneController = this;
		userGUI = gameObject.AddComponent <UserGUI>() as UserGUI;
		characters = new ChaCon[6];
		loadResources();
		actionManager = gameObject.AddComponent<MySceneActionManager>() as MySceneActionManager;
		//judger = gameObject.AddComponent<Judger>() as Judger;
		judger = Judger.getInstance();
	}

	public void loadResources() {
		Vector3 river_pos = new Vector3(0,0.5F,0);
		GameObject river = Instantiate (Resources.Load ("Perfabs/River", typeof(GameObject)), river_pos, Quaternion.identity, null) as GameObject;
		river.name = "river";
		fromCoast = new CoastCon ("from");
		toCoast = new CoastCon ("to");
		boat = new BoatCon ();
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
		actionManager.moveBoat(boat.getGameObject(),boat.BoatMoveToPosition(),boat.move_speed);
		userGUI.status = Judger.getInstance().check(fromCoast,toCoast,boat);
	}

	public void ClickCha(ChaCon cha) {
		if (cha.isOnBoat ()) {
			CoastCon nowCoast;
			if (boat.getStatus () == -1) nowCoast = toCoast; // to->-1; from->1	
			else nowCoast = fromCoast;
			boat.getOffBoat (cha.getName());

			Vector3 end_pos = nowCoast.getEmptyPosition();                                         //动作分离版本改变
            Vector3 middle_pos = new Vector3(cha.getGameObject().transform.position.x, end_pos.y, end_pos.z);  //动作分离版本改变
            actionManager.moveCha(cha.getGameObject(), middle_pos, end_pos, cha.move_speed);  //动作分离版本改变

			cha.getOnCoast (nowCoast);
			nowCoast.getOnCoast (cha);
		} 
		else {									// character on coast
			CoastCon nowCoast = cha.getCoastCon ();
			if (boat.getEmptyIndex () == -1) return; // boat is full
			if (nowCoast.getStatus () != boat.getStatus ())	return; // boat is not on the side of character
			nowCoast.getOffCoast(cha.getName());
			
			Vector3 end_pos = boat.getEmptyPosition();                                             //动作分离版本改变
            Vector3 middle_pos = new Vector3(end_pos.x, cha.getGameObject().transform.position.y, end_pos.z); //动作分离版本改变
            actionManager.moveCha(cha.getGameObject(), middle_pos, end_pos, cha.move_speed);  //动作分离版本改变
			
			cha.getOnBoat (boat);
			boat.getOnBoat (cha);
		}
		userGUI.status = Judger.getInstance().check(fromCoast,toCoast,boat);
	}

	public void restart() {
		boat.reset ();
		fromCoast.reset ();
		toCoast.reset ();
		for (int i = 0; i < characters.Length; i++) characters[i].reset ();
	}
}
