using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour {
    //分数变化
    public delegate void ScoreEvent ();
    public static event ScoreEvent ScoreChange;
    //游戏结束变化
    public delegate void GameoverEvent ();
    public static event GameoverEvent GameoverChange;

    //玩家逃脱
    public void PlayerEscape () {
        if (ScoreChange != null) {
            ScoreChange ();
        }
    }
    //玩家被捕
    public void PlayerGameover () {
        if (GameoverChange != null) {
            GameoverChange ();
        }
    }
}