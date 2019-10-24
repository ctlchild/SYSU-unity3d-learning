using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropFactory : MonoBehaviour {
    private GameObject patrol = null; //巡逻兵
    private List<GameObject> used = new List<GameObject> (); //正在被使用的巡逻兵
    private Vector3[] vec = new Vector3[9]; //保存每个巡逻兵的初始位置

    public FirstSceneController sceneControler; //场景控制器

    public List<GameObject> GetPatrols () {
        int[] pos_x = {-6, 4, 13 };
        int[] pos_z = {-4, 6, -13 };
        int index = 0;
        //生成不同的巡逻兵初始位置
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                vec[index] = new Vector3 (pos_x[i], 0, pos_z[j]);
                index++;
            }
        }
        for (int i = 0; i < 9; i++) {
            patrol = Instantiate (Resources.Load<GameObject> ("Prefabs/Patrol"));
            patrol.transform.position = vec[i];
            patrol.GetComponent<PatrolData> ().sign = i + 1;
            patrol.GetComponent<PatrolData> ().start_position = vec[i];
            used.Add (patrol);
        }
        return used;
    }
    public void StopPatrol () {
        //切换所有侦查兵的动画
        for (int i = 0; i < used.Count; i++) {
            used[i].gameObject.GetComponent<Animator> ().SetBool ("run", false);
        }
    }
}