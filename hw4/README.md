# Homework 4

### 编程实践——编写一个简单的鼠标打飞碟（Hit UFO）游戏

- 游戏内容要求：
  1. 游戏有 n 个 round，每个 round 都包括10 次 trial；
  2. 每个 trial 的飞碟的色彩、大小、发射位置、速度、角度、同时出现的个数都可能不同。它们由该 round 的 ruler 控制；
  3. 每个 trial 的飞碟有随机性，总体难度随 round 上升；
  4. 鼠标点中得分，得分规则按色彩、大小、速度不同计算，规则可自由设定。
- 游戏的要求：
  - 使用带缓存的工厂模式管理不同飞碟的生产与回收，该工厂必须是场景单实例的！具体实现见参考资源 Singleton 模板类
  - 尽可能使用前面 MVC 结构实现人机交互与游戏模型分离



我设计的这个游戏规则大体如下，游戏总共有3关，每关有10个trial，trial1~3只会同时出现1个飞碟，trial4~6会同时出现2个飞碟，trial7~10会同时出现3个飞碟。不同关卡飞碟的大小和速度会有区别。不同飞碟的得分会有区别。每局游戏一共有10条命，当没有成功击落飞碟会扣除一条命，生命值为0时游戏结束。

首先是实现用工厂模式管理不同飞碟的生产与回收。

```c#
public GameObject disk_prefab = null;                 //飞碟预制体
private List<DiskData> used = new List<DiskData>();   //正在被使用的飞碟列表
private List<DiskData> free = new List<DiskData>();   //空闲的飞碟列表
```

实现了函数`GetDisk`，控制器通过把当前关卡传给这个函数并带有随机性质地返回相应的飞碟，如果空闲列表中存在有飞碟则取空闲列表中的，否则将从预制件中实例化一个新的飞碟并返回。

```c#
public GameObject GetDisk(int round){
    int choice = 0;
    int scope1 = 1, scope2 = 4, scope3 = 7;           //随机的范围
    float start_y = -10f;                             //刚实例化时的飞碟的竖直位置
    string name;
    disk_prefab = null;

    //根据回合，随机选择要飞出的飞碟
    if (round == 1) choice = Random.Range(0, scope1);
    else if(round == 2) choice = Random.Range(0, scope2);
    else choice = Random.Range(0, scope3);

    //将要选择的飞碟的name
    if(choice <= scope1) name = "disk1";
    else if(choice <= scope2 && choice > scope1) name = "disk2";
    else name = "disk3";

    //寻找相同name的空闲飞碟
    for(int i=0;i<free.Count;i++) if(free[i].name == name){
        disk_prefab = free[i].gameObject;
        free.Remove(free[i]);
        break;
    }
    //如果空闲列表中没有，则重新实例化飞碟
    if(disk_prefab == null){
        if (name == "disk1") disk_prefab = Instantiate(Resources.Load<GameObject>("Prefabs/disk1"), new Vector3(0, start_y, 0), Quaternion.identity);
        else if (name == "disk2") disk_prefab = Instantiate(Resources.Load<GameObject>("Prefabs/disk2"), new Vector3(0, start_y, 0), Quaternion.identity);
        else disk_prefab = Instantiate(Resources.Load<GameObject>("Prefabs/disk3"), new Vector3(0, start_y, 0), Quaternion.identity);

        //给新实例化的飞碟赋予其他属性
        float ran_x = Random.Range(-1f, 1f) < 0 ? -1 : 1;
        disk_prefab.GetComponent<Renderer>().material.color = disk_prefab.GetComponent<DiskData>().color;
        disk_prefab.GetComponent<DiskData>().direction = new Vector3(ran_x, start_y, 0);
        disk_prefab.transform.localScale = disk_prefab.GetComponent<DiskData>().scale;
    }
    //添加到使用列表中
    used.Add(disk_prefab.GetComponent<DiskData>());
    return disk_prefab;
}
```

当飞碟完成操作（被击落或是飞出屏幕）后需要回收飞碟

```c#
//回收飞碟
public void FreeDisk(GameObject disk){
    for(int i = 0;i < used.Count; i++) if (disk.GetInstanceID() == used[i].gameObject.GetInstanceID()){
        used[i].gameObject.SetActive(false);
        free.Add(used[i]);
        used.Remove(used[i]);
        break;
    }
}
```

本次实践实现了飞碟模型与动作的分离，与上一次实践类似，只需要继承实现好的三个类：`SSAction`，`SSActionManager`，`SequenceAction`即动作类，动作管理器类，以及对于连续动作序列的类。

对于动作类，需要实现UFO飞行的动作逻辑。我实现的是UFO将从屏幕的左边或右边飞出，在重力作用下和给定角度下做斜抛运动。在生产UFO时已经随机得到了UFO是从左边还是右边飞出以及其初始的纵坐标，我们可以简单得到其运动的起始速度向量以及运动过程中的速度。

```c#
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
```

对于动作管理类，只需要简单建立起动作和管理器之间的联系并作为中间人调用动作类即可。

本实践事实上不需要实现动作序列，因为动作类足以描述飞碟运动全过程。

然后是全局控制器类

首先实现了`Start`函数，定义了计分规则，实例化了相关联的类。

```c#
void Start (){
    //初始化参数 一共3级 1级10层
    trial = new int[3][];                                                
    score_round = new int[3];                                           
    int tmp=0;
    for (int i=0;i<3;i++){
        trial[i]= new int[10];
        for (int j=0;j<10;j++){
            trial[i][j]=tmp;
            tmp+=2*(i+1);
        }
        score_round[i]=trial[i][0];
    }

    SSDirector director = SSDirector.GetInstance();     
    director.CurrentScenceController = this;             
    disk_factory = Singleton<DiskFactory>.Instance;
    score_recorder = Singleton<ScoreRecorder>.Instance;
    action_manager = gameObject.AddComponent<FlyActionManager>() as FlyActionManager;
    user_gui = gameObject.AddComponent<UserGUI>() as UserGUI;

}
```

其中，`disk_factory`和` score_recorder`即飞碟工厂和计分员都是单实例的，根据Singleton模板类实现。

```c#
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    Debug.LogError("An instance of " + typeof(T)
                        + " is needed in the scene, but there is none.");
                }
            }
            return instance;
        }
    }
}
```

然后是`Update`函数，它根据当前游戏的情况来确定使用飞碟的逻辑

```c#
void Update (){
    if(game_start){
        //游戏结束，取消定时发送飞碟
        if (game_over) CancelInvoke("LoadResources");

        //设定一个定时器，发送飞碟，游戏开始
        if (!playing_game){
            InvokeRepeating("LoadResources", 1f, speed);
            playing_game = true;
        }

        //发送飞碟
        SendDisk();

        //回合升级
        if (score_recorder.score >= score_round[1] && round == 0){
            round = 1;
            //缩小飞碟发送间隔
            speed = speed - 0.6f;
            CancelInvoke("LoadResources");
            playing_game = false;
        }
        else if (score_recorder.score >= score_round[2] && round == 1){
            round = 2;
            speed = speed - 0.5f;
            CancelInvoke("LoadResources");
            playing_game = false;
        }
    }
}
```

其中的`LoadResources`函数实现了访问工厂使用飞碟

```c#
public void LoadResources(){
    disk_queue.Enqueue(disk_factory.GetDisk(round+1)); 
    if (score_recorder.score>=trial[round][3]) 
        disk_queue.Enqueue(disk_factory.GetDisk(round+1)); 
    if (score_recorder.score>=trial[round][6])
        disk_queue.Enqueue(disk_factory.GetDisk(round+1)); 
}
```

其中的`SendDisk`函数将从工厂获得的飞碟经过处理（随机地给飞碟力与角度使其能实现斜抛运动）传递给动作管理器类实现飞碟运动，并对飞碟飞出后的情况（被击落或是飞出屏幕）进行处理。

```c#
private void SendDisk(){
    float position_x = 16;                       
    if (disk_queue.Count != 0){
        GameObject disk = disk_queue.Dequeue();
        disk_notshot.Add(disk);
        disk.SetActive(true);
        //设置被隐藏了或是新建的飞碟的位置
        float ran_y = Random.Range(1f, 4f);
        float ran_x = Random.Range(-1f, 1f) < 0 ? -1 : 1;
        disk.GetComponent<DiskData>().direction = new Vector3(ran_x, ran_y, 0);
        Vector3 position = new Vector3(-disk.GetComponent<DiskData>().direction.x * position_x, ran_y, 0);
        disk.transform.position = position;
        //设置飞碟初始所受的力和角度
        float power = Random.Range(10f, 15f);
        float angle = Random.Range(15f, 28f);
        action_manager.UFOFly(disk,angle,power);
    }

    for (int i = 0; i < disk_notshot.Count; i++){
        GameObject temp = disk_notshot[i];
        //飞碟飞出摄像机视野也没被打中
        if (temp.transform.position.y < -10 && temp.gameObject.activeSelf == true){
            disk_factory.FreeDisk(disk_notshot[i]);
            disk_notshot.Remove(disk_notshot[i]);
            //玩家血量-1
            user_gui.ReduceBlood();
        }
    }
}
```

需要实现击落飞碟的逻辑，通过GUI类接受`Fire1`（默认为单击鼠标左键）来访问控制器实现对飞碟的打击，记录下点击的射线然后进行判断，如果击中了则实现记分员记分，飞碟爆炸，回收飞碟逻辑。

```c#
public void Hit(Vector3 pos){
    Ray ray = Camera.main.ScreenPointToRay(pos);
    RaycastHit[] hits;
    hits = Physics.RaycastAll(ray);
    bool not_hit = false;
    for (int i = 0; i < hits.Length; i++){
        RaycastHit hit = hits[i];
        //射线打中物体
        if (hit.collider.gameObject.GetComponent<DiskData>() != null){
            //射中的物体要在没有打中的飞碟列表中
            for (int j = 0; j < disk_notshot.Count; j++)
                if (hit.collider.gameObject.GetInstanceID() == disk_notshot[j].gameObject.GetInstanceID()) not_hit = true;

            if(!not_hit) return;

            disk_notshot.Remove(hit.collider.gameObject);
            //记分员记录分数
            score_recorder.Record(hit.collider.gameObject);
            //显示爆炸粒子效果
            Transform explode = hit.collider.gameObject.transform.GetChild(0);
            explode.GetComponent<ParticleSystem>().Play();
            //执行回收飞碟
            StartCoroutine(WaitingParticle(0.02f, hit, disk_factory, hit.collider.gameObject));
            break;
        }
    }
}
```

GUI类显示基本的游戏情况，包括分数，生命，记录玩家射击的过程并传递给控制器，当游戏结束时出现重新游戏按钮以及记录最高分并显示。逻辑比较简单。

游戏演示图如下：

![](./演示.gif)

