using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum Player { player0, player1, player2};

public class PvP : MonoBehaviour {
    bool playing = true;  //游戏是否在进行
    int turn = 1;  //当前是谁的回合
    int[,] symbol = new int [3,3];  //0 未放置 1 x 2 o
    //public Texture2D img;

    void Start () {
        Reset();
    }
    void Reset() {
        playing = true;
        turn = 1;
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                symbol[i, j] = 0;
            }
        }
        GUI.enabled = true;
    }

    void OnGUI() {
        //position parameters
        int bHeight = 100;
        int bWidth = 100;
        float height = Screen.height * 0.5f - 150;
        float width = Screen.width * 0.5f - 150;

        //UI Style parameters
        GUIStyle tStyle = new GUIStyle {
            fontSize = 50,
            fontStyle = FontStyle.Bold
        };
        GUIStyle mStyle = new GUIStyle {
            fontSize = 25,
            fontStyle = FontStyle.Bold
        };
        mStyle.normal.textColor = Color.red;

        //GUIStyle bgStyle = new GUIStyle();
        //bgStyle.normal.background = img;
        //GUI.Label(new Rect(0, 0, 1024, 781), "", bgStyle);



        //winner parameters
        int winner = Check();
        string msg = "";

        //Back button
        // if (GUI.Button(new Rect(width - bWidth * 2, height - bHeight * 1.5f, bWidth / 2, bHeight / 2), "Back")) {
        //     Application.LoadLevel("Welcome");
        // }

        //Reset button
        if (GUI.Button(new Rect(width + bWidth / 2, height + 3.5f * bHeight, bWidth * 2, bHeight / 2), "Reset")) {
            Reset();
            return;
        }

        //Check if someone wins or draw
        if (winner>=1) {
            msg = (winner == 1 ? "Player1(X) Wins!" : "Player2(O) Wins!");
            GUI.Label(new Rect(width + 50, height - 75, 100, 100), msg, mStyle);
            playing = false;
            GUI.enabled = false;
        }
        if (winner==-1){
            msg = "Draw!";
            GUI.Label(new Rect(width + 100, height - 75, 50, 100), msg, mStyle);
            playing = false;
            GUI.enabled = false;
        }

        GUI.Label(new Rect(width + 20, height - 150, 100, 100), "Tic Tac Toe", tStyle);
        for (int i = 0; i < 3; ++i) {
            for (int j = 0; j < 3; ++j) {
                if (symbol[i, j] == 1) {
                    GUI.Button(new Rect(width + i * bWidth, height + j * bHeight, bWidth, bHeight), "X");
                } else if (symbol[i, j] == 2) {
                    GUI.Button(new Rect(width + i * bWidth, height + j * bHeight, bWidth, bHeight), "O");
                } else {
                    if (GUI.Button(new Rect(width + i * bWidth, height + j * bHeight, bWidth, bHeight), "")) {
                        if (playing) {
                            symbol[i, j] = turn;
                            turn = 3 - turn;
                        }
                    }
                }
            }
        }
        //Enable the reset button
        GUI.enabled = true;
    }

    int Check() {
        //Row check
        for (int i = 0; i < 3; ++i) {
            if (symbol[i, 0] > 0 &&
                symbol[i, 0] == symbol[i, 1] &&
                symbol[i, 1] == symbol[i, 2]) {
                return symbol[i, 0];
            }
        }
        //Column check
        for (int j = 0; j < 3; ++j) {
            if (symbol[0, j] > 0 &&
                symbol[0, j] == symbol[1, j] &&
                symbol[1, j] == symbol[2, j]) {
                return symbol[0, j];
            }
        }
        //Cross line check
        if (symbol[1, 1] > 0) {
            if (symbol[1, 1] == symbol[0, 0] && symbol[1, 1] == symbol[2, 2] ||
                symbol[1, 1] == symbol[0, 2] && symbol[1, 1] == symbol[2, 0]) {
                return symbol[1, 1];
            }
        }
        int draw=-1;
        for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++) if (symbol[i,j]==0) draw=0;
        return draw;
    }
}
