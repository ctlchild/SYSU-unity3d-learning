using UnityEngine;
using Vuforia;
public class VirtualButtonEventHandler :  MonoBehaviour, Vuforia.IVirtualButtonEventHandler
{
    public GameObject vb;
    public Animator ani;
    void Start()
    {
        VirtualButtonBehaviour vbb = vb.GetComponent<VirtualButtonBehaviour>();
        // 为每个虚拟按钮注册按钮响应事件。
        if (vbb)
        {
            vbb.RegisterEventHandler(this);
        }
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        Debug.Log("OnButtonPressed: " + vb.VirtualButtonName);
        ani.SetTrigger("Idle_Run");
    }
    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        Debug.Log("OnButtonReleased: " + vb.VirtualButtonName);
        ani.SetTrigger("Run_Idel");
    }
}