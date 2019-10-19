using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAction : ScriptableObject{
    public bool enable = true;                      //是否正在进行此动作
    public bool destroy = false;                    //是否需要被销毁
    public GameObject gameobject;                   //动作对象
    public Transform transform;                     //动作对象的transform
    public ISSActionCallback callback;              //动作完成后的消息通知者

    protected SSAction() { }                        
    //子类可以使用下面这两个函数
    public virtual void Start(){
        throw new System.NotImplementedException();
    }
    public virtual void Update(){
        throw new System.NotImplementedException();
    }
}

public class SSActionManager : MonoBehaviour, ISSActionCallback{
    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();    //将执行的动作的字典集合
    private List<SSAction> waitingAdd = new List<SSAction>();                       //等待去执行的动作列表
    private List<int> waitingDelete = new List<int>();                              //等待删除的动作的key                

    protected void Update(){
        foreach (SSAction ac in waitingAdd)
            actions[ac.GetInstanceID()] = ac;                                    
        waitingAdd.Clear();

        foreach (KeyValuePair<int, SSAction> kv in actions){
            SSAction ac = kv.Value;
            if (ac.destroy) waitingDelete.Add(ac.GetInstanceID());
            else if (ac.enable) ac.Update();
        }

        foreach (int key in waitingDelete){
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager){
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null){ }
}

public class SequenceAction : SSAction, ISSActionCallback{
    
    public List<SSAction> sequence;    //动作的列表
    public int repeat = -1;            //-1就是无限循环做组合中的动作
    public int start = 0;              //当前做的动作的索引

    public static SequenceAction GetSSAcition(int repeat, int start, List<SSAction> sequence){
        SequenceAction action = ScriptableObject.CreateInstance<SequenceAction>();
        action.repeat = repeat;
        action.sequence = sequence;
        action.start = start;
        return action;
    }

    public override void Update(){
        if (sequence.Count == 0) return;
        if (start < sequence.Count) sequence[start].Update();    
    }

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null){
        source.destroy = false;   
        this.start++;
        if (this.start >= sequence.Count){
            this.start = 0;
            if (repeat > 0) repeat--;
            if (repeat == 0){
                this.destroy = true;
                this.callback.SSActionEvent(this); 
            }
        }
    }

    public override void Start(){
        foreach (SSAction action in sequence){
            action.gameobject = this.gameobject;
            action.transform = this.transform;
            action.callback = this;            
            action.Start();
        }
    }

    void OnDestroy(){ }
}

public class UFOFlyAction : SSAction{
    public float gravity = -5;                                 //向下的加速度
    private Vector3 start_vector;                              //初速度向量
    private Vector3 gravity_vector = Vector3.zero;             //加速度的向量，初始时为0
    private float time;                                        //已经过去的时间
    private Vector3 current_angle = Vector3.zero;               //当前时间的欧拉角

    private UFOFlyAction() { }
    public static UFOFlyAction GetSSAction(Vector3 direction, float angle, float power){
        //初始化物体将要运动的初速度向量
        UFOFlyAction action = CreateInstance<UFOFlyAction>();
        if (direction.x == -1) action.start_vector = Quaternion.Euler(new Vector3(0, 0, -angle)) * Vector3.left * power;
        else action.start_vector = Quaternion.Euler(new Vector3(0, 0, angle)) * Vector3.right * power;
        return action;
    }

    public override void Update(){
        //计算物体的向下的速度,v=at
        time += Time.fixedDeltaTime;
        gravity_vector.y = gravity * time;

        //位移模拟
        transform.position += (start_vector + gravity_vector) * Time.fixedDeltaTime;
        current_angle.z = Mathf.Atan((start_vector.y + gravity_vector.y) / start_vector.x) * Mathf.Rad2Deg;
        transform.eulerAngles = current_angle;

        //如果物体y坐标小于-10，动作就做完了
        if (this.transform.position.y < -10){
            this.destroy = true;
            this.callback.SSActionEvent(this);      
        }
    }

    public override void Start() { }
}

public class FlyActionManager : SSActionManager{

    public UFOFlyAction fly;                            //飞碟飞行的动作

    protected void Start(){  
    }
    //飞碟飞行
    public void UFOFly(GameObject disk, float angle, float power){
        fly = UFOFlyAction.GetSSAction(disk.GetComponent<DiskData>().direction, angle, power);
        this.RunAction(disk, fly, this);
    }
}

public class PhyUFOFlyAction : SSAction{
    private Vector3 start_vector;                              //初速度向量
    public float power;
    private PhyUFOFlyAction() {}
    public static PhyUFOFlyAction GetSSAction(Vector3 direction, float angle, float power){
        //初始化物体将要运动的初速度向量
        PhyUFOFlyAction action = CreateInstance<PhyUFOFlyAction>();
        if (direction.x == -1) action.start_vector = Quaternion.Euler(new Vector3(0, 0, -angle)) * Vector3.left * power;
        else action.start_vector = Quaternion.Euler(new Vector3(0, 0, angle)) * Vector3.right * power;
        action.power = power;
        return action;
    }

    public override void Update() { 
        if (this.transform.position.y < -10){
            this.destroy = true;
            this.callback.SSActionEvent(this);
        }
    }
    public override void Start(){
        //使用重力以及给一个初速度
        gameobject.GetComponent<Rigidbody>().velocity = power / 13 * start_vector;
        gameobject.GetComponent<Rigidbody>().useGravity = true;
    }
}

public class PhyActionManager : SSActionManager{
    public PhyUFOFlyAction fly;
    protected void Start(){  
    }
    //飞碟飞行
    public void UFOFly(GameObject disk, float angle, float power){
        fly = PhyUFOFlyAction.GetSSAction(disk.GetComponent<DiskData>().direction, angle, power);
        this.RunAction(disk, fly, this);
    }

}