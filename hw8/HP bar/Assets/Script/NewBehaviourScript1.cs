using UnityEngine;

using System.Collections;


public class NewBehaviourScript1 : MonoBehaviour
{
    public float movespeed;
   
    void Start()
    {


    }

    void Update()
    {       

        float MoveH = Input.GetAxis("Horizontal");
        float MoveV = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(MoveH, 0, MoveV)* Time.deltaTime * movespeed);    //按水平或者垂直的方向键，                                                                                                                                              就怎么移动
        if (Input.GetButton("Jump"))
        {
            Rigidbody r = GetComponent<Rigidbody>();
            r.AddForce(new Vector3(0,0.5f,0),ForceMode.Impulse);
        }
    }
}