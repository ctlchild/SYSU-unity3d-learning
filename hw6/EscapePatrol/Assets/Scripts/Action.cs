using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAction : ScriptableObject {
    public bool enable = true; //是否正在进行此动作
    public bool destroy = false; //是否需要被销毁
    public GameObject gameobject; //动作对象
    public Transform transform; //动作对象的transform
    public ISSActionCallback callback; //动作完成后的消息通知者

    protected SSAction () { }
    //子类可以使用下面这两个函数
    public virtual void Start () {
        throw new System.NotImplementedException ();
    }
    public virtual void Update () {
        throw new System.NotImplementedException ();
    }
}

public class SSActionManager : MonoBehaviour, ISSActionCallback {
    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction> (); //将执行的动作的字典集合
    private List<SSAction> waitingAdd = new List<SSAction> (); //等待去执行的动作列表
    private List<int> waitingDelete = new List<int> (); //等待删除的动作的key                

    protected void Update () {
        foreach (SSAction ac in waitingAdd) {
            actions[ac.GetInstanceID ()] = ac;
        }
        waitingAdd.Clear ();

        foreach (KeyValuePair<int, SSAction> kv in actions) {
            SSAction ac = kv.Value;
            if (ac.destroy) {
                waitingDelete.Add (ac.GetInstanceID ());
            } else if (ac.enable) {
                //运动学运动更新
                ac.Update ();
            }
        }

        foreach (int key in waitingDelete) {
            SSAction ac = actions[key];
            actions.Remove (key);
            DestroyObject (ac);
        }
        waitingDelete.Clear ();
    }

    public void RunAction (GameObject gameobject, SSAction action, ISSActionCallback manager) {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add (action);
        action.Start ();
    }

    public void SSActionEvent (SSAction source, int intParam = 0, GameObject objectParam = null) {
        if (intParam == 0) {
            //侦查兵跟随玩家
            PatrolFollowAction follow = PatrolFollowAction.GetSSAction (objectParam.gameObject.GetComponent<PatrolData> ().player);
            this.RunAction (objectParam, follow, this);
        } else {
            //侦察兵按照初始位置开始继续巡逻
            GoPatrolAction move = GoPatrolAction.GetSSAction (objectParam.gameObject.GetComponent<PatrolData> ().start_position);
            this.RunAction (objectParam, move, this);
            //玩家逃脱
            Singleton<GameEventManager>.Instance.PlayerEscape ();
        }
    }

    public void DestroyAll () {
        foreach (KeyValuePair<int, SSAction> kv in actions) {
            SSAction ac = kv.Value;
            ac.destroy = true;
        }
    }
}

public class GoPatrolAction : SSAction {
    private enum Dirction { EAST, NORTH, WEST, SOUTH }
    private float pos_x, pos_z; //移动前的初始x和z方向坐标
    private float move_length; //移动的长度
    private float move_speed = 1.2f; //移动速度
    private bool move_sign = true; //是否到达目的地
    private Dirction dirction = Dirction.EAST; //移动的方向
    private PatrolData data; //侦察兵的数据

    private GoPatrolAction () { }
    public static GoPatrolAction GetSSAction (Vector3 location) {
        GoPatrolAction action = CreateInstance<GoPatrolAction> ();
        action.pos_x = location.x;
        action.pos_z = location.z;
        //设定移动矩形的边长
        action.move_length = Random.Range (4, 7);
        return action;
    }
    public override void Update () {
        //防止碰撞发生后的旋转
        if (transform.localEulerAngles.x != 0 || transform.localEulerAngles.z != 0) {
            transform.localEulerAngles = new Vector3 (0, transform.localEulerAngles.y, 0);
        }
        if (transform.position.y != 0) {
            transform.position = new Vector3 (transform.position.x, 0, transform.position.z);
        }
        //侦察移动
        Gopatrol ();
        //如果侦察兵需要跟随玩家并且玩家就在侦察兵所在的区域，侦查动作结束
        if (data.follow_player && data.wall_sign == data.sign) {
            this.destroy = true;
            this.callback.SSActionEvent (this, 0, this.gameobject);
        }
    }
    public override void Start () {
        this.gameobject.GetComponent<Animator> ().SetBool ("run", true);
        data = this.gameobject.GetComponent<PatrolData> ();
    }

    void Gopatrol () {
        if (move_sign) {
            //不需要转向则设定一个目的地，按照矩形移动
            switch (dirction) {
                case Dirction.EAST:
                    pos_x -= move_length;
                    break;
                case Dirction.NORTH:
                    pos_z += move_length;
                    break;
                case Dirction.WEST:
                    pos_x += move_length;
                    break;
                case Dirction.SOUTH:
                    pos_z -= move_length;
                    break;
            }
            move_sign = false;
        }
        this.transform.LookAt (new Vector3 (pos_x, 0, pos_z));
        float distance = Vector3.Distance (transform.position, new Vector3 (pos_x, 0, pos_z));
        //当前位置与目的地距离浮点数的比较
        if (distance > 0.9) {
            transform.position = Vector3.MoveTowards (this.transform.position, new Vector3 (pos_x, 0, pos_z), move_speed * Time.deltaTime);
        } else {
            dirction = dirction + 1;
            if (dirction > Dirction.SOUTH) {
                dirction = Dirction.EAST;
            }
            move_sign = true;
        }
    }
}

public class PatrolFollowAction : SSAction {
    private float speed = 2f; //跟随玩家的速度
    private GameObject player; //玩家
    private PatrolData data; //侦查兵数据

    private PatrolFollowAction () { }
    public static PatrolFollowAction GetSSAction (GameObject player) {
        PatrolFollowAction action = CreateInstance<PatrolFollowAction> ();
        action.player = player;
        return action;
    }

    public override void Update () {
        if (transform.localEulerAngles.x != 0 || transform.localEulerAngles.z != 0) {
            transform.localEulerAngles = new Vector3 (0, transform.localEulerAngles.y, 0);
        }
        if (transform.position.y != 0) {
            transform.position = new Vector3 (transform.position.x, 0, transform.position.z);
        }

        Follow ();
        //如果侦察兵没有跟随对象，或者需要跟随的玩家不在侦查兵的区域内
        if (!data.follow_player || data.wall_sign != data.sign) {
            this.destroy = true;
            this.callback.SSActionEvent (this, 1, this.gameobject);
        }
    }
    public override void Start () {
        data = this.gameobject.GetComponent<PatrolData> ();
    }
    void Follow () {
        transform.position = Vector3.MoveTowards (this.transform.position, player.transform.position, speed * Time.deltaTime);
        this.transform.LookAt (player.transform.position);
    }
}

public class PatrolActionManager : SSActionManager {
    private GoPatrolAction go_patrol; //巡逻兵巡逻

    public void GoPatrol (GameObject patrol) {
        go_patrol = GoPatrolAction.GetSSAction (patrol.transform.position);
        this.RunAction (patrol, go_patrol, this);
    }
    //停止所有动作
    public void DestroyAllAction () {
        DestroyAll ();
    }
}