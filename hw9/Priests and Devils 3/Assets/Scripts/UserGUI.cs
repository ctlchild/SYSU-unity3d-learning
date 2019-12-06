using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;

public class UserGUI : MonoBehaviour {
	UserAction action = null;
	private string hint="";
	public int status = 0;
	public int tips = 0;
	GUIStyle style;
	GUIStyle buttonStyle;
	GUIStyle hintStyle;

	void Start() {
		action = Director.getInstance ().currentSceneController as UserAction;

		style = new GUIStyle();
		style.fontSize = 40;
		style.alignment = TextAnchor.MiddleCenter;

		buttonStyle = new GUIStyle("button");
		buttonStyle.fontSize = 30;
		
	}
	void OnGUI() {
		hintStyle = new GUIStyle {
			fontSize = 15,
			fontStyle = FontStyle.Normal
      	};
		GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height / 2 - 210, 100, 50),hint, hintStyle);
		if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 90, 100, 50), "Tips", buttonStyle)){
			if (tips==-1) {
				hint="None";
			}
			else {
				int o=tips/16,fromP=(tips%16)/4,fromD=tips%4;
				hint = "Hint:\n" + "From:  Devils: " + fromD.ToString() + "   Priests: " + fromP.ToString() +
				"\nto:  Devils: " + (3-fromD).ToString() + "   Priests: " + (3-fromP).ToString();
			}
			Debug.Log(hint);
			Debug.Log(tips);
		}
		if (status == 1) {
			GUI.Label(new Rect(Screen.width/2-50, Screen.height/2-85, 100, 50), "Gameover!", style);
			if (GUI.Button(new Rect(Screen.width/2-70, Screen.height/2, 140, 70), "Restart", buttonStyle)) {
				status = 0;
				action.restart ();
			}
		} else if(status == 2) {
			GUI.Label(new Rect(Screen.width/2-50, Screen.height/2-85, 100, 50), "You win!", style);
			if (GUI.Button(new Rect(Screen.width/2-70, Screen.height/2, 140, 70), "Restart", buttonStyle)) {
				status = 0;
				action.restart ();
			}
		}
	}
}
