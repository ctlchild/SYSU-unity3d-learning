using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRecorder : MonoBehaviour {
    public FirstSceneController sceneController;
    public int score = 0; //分数
    // Use this for initialization
    void Start () {
        sceneController = (FirstSceneController) SSDirector.GetInstance ().CurrentScenceController;
        sceneController.recorder = this;
    }
    public int GetScore () {
        return score;
    }
    public void AddScore () {
        score++;
    }
}