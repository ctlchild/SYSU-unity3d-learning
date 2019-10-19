using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adapter : MonoBehaviour,AdaActionmanager {
    public FlyActionManager fly_action_manager;
    public PhyActionManager phy_action_manager;
    public void UFOFly(GameObject disk,float angle,float power,bool op){
        if (op) phy_action_manager.UFOFly(disk,angle,power);
        else fly_action_manager.UFOFly(disk,angle,power);
    }
    void Start(){
        fly_action_manager=gameObject.AddComponent<FlyActionManager>() as FlyActionManager;
        phy_action_manager=gameObject.AddComponent<PhyActionManager>() as PhyActionManager;
    }
}
