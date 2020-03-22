using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeto2d : MonoBehaviour {
    private GameObject player;//玩家
    private Camera maincamera;//获取主相机
    private int amount;//碰撞次数
    private bool nowischangemode;//装换模式中
    public GameObject cameratarget2d;//2d的摄像机目标标志
    public GameObject cameratarget3d;//2d的摄像机目标标志
    public GameObject cameratarget;//摄像机实时依从的位置标志
   // private Quaternion turnto2d; //转换成四元数用
    public Transform  transform2d;//转换成2d是玩家强制移动到目标位置
    public TouchControl ts;//触摸

    PlayCharacter cc;
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        amount = 0;
        nowischangemode = false;
        //turnto2d= Quaternion.LookRotation(cameratarget.transform.forward);//转换成四元数
        cc = player.GetComponent<PlayCharacter>();
    }
	
	// Update is called once per frame
	void Update () {
        if (cc.is2dmode == true)
        {
            ts.DisableTouchControlView();
        }
        if (nowischangemode)
        {
            if (cc.is2dmode == false)
            {
               changemodeto2d();
                if ((cameratarget.transform.forward-cameratarget2d.transform.forward).sqrMagnitude<0.01f)
                {
                    nowischangemode = false;
                    cc.nowischangemode = false;
                    cc.is2dmode = true;
                    ts.DisableTouchControlView();

                }
            }
           else if (cc.is2dmode == true)
            {
                changemodeto3d();
                if ((cameratarget.transform.forward-cameratarget3d.transform.forward).sqrMagnitude < 0.01f)
                {
                    Debug.Log("3d!");
                    nowischangemode = false;
                    cc.nowischangemode = false;
                    cc.is2dmode = false;
                    ts.EnableTouchControlView();
                }
            }


        }
		
	}
    private void OnTriggerEnter(Collider other)
    {
    
        if (other.gameObject.tag=="Player")
        {
            amount++;
            if (amount==1)
            {
                Debug.Log("changemode");
                if (cc.is2dmode == true)
                {
                    nowischangemode = true;
                    cc.nowischangemode = true;
                    Camera.main.orthographic = false;
                   // cc.is2dmode = false;
                }
                else if (cc.is2dmode == false)
                {
                    Camera.main.orthographic = true;
                    nowischangemode = true;
                    cc.nowischangemode = true;
                    //cc.is2dmode = true;
                }
                amount = 0;
            }
            

        }
        
    }
    public void changemodeto2d()
        {
        ts.EnableTouchControlView();
        Vector3 larp = Vector3.Lerp(cameratarget.transform.forward, cameratarget2d.transform.forward, 1f * Time.deltaTime);
          var turnto = Quaternion.LookRotation(larp);//转换成四元数
          // Quaternion slerp = Quaternion.Slerp(cameratarget.transform.rotation, turnto2d, 20f*Time.deltaTime);
           cameratarget.transform.rotation = turnto;
        //玩家角色摆正
        Vector3 larpplayer = Vector3.Lerp(player.transform.forward, transform.up, 3f * Time.deltaTime);
        var playerturnto = Quaternion.LookRotation(larpplayer);//转换成四元数
        //Quaternion slerp = Quaternion.Slerp(transform.rotation, turnto, turnspeed * Time.deltaTime);
        player.transform.rotation = playerturnto;
        //玩家位置摆正
        Vector3 playerposition = Vector3.Lerp(player.transform.position, transform2d.position, 3f * Time.deltaTime);
        //var playerturnto = Quaternion.LookRotation(larpplayer);//转换成四元数
        //Quaternion slerp = Quaternion.Slerp(transform.rotation, turnto, turnspeed * Time.deltaTime);
        player.transform.position = playerposition;
    }


    public void changemodeto3d()
    {
        ts.EnableTouchControlView();
        Vector3 larp = Vector3.Lerp(cameratarget.transform.forward, cameratarget3d.transform.forward, 1f * Time.deltaTime);
        var turnto = Quaternion.LookRotation(larp);//转换成四元数
        Debug.Log(cameratarget.transform.forward);
        Debug.Log(cameratarget3d.transform.forward);
        // Quaternion slerp = Quaternion.Slerp(cameratarget.transform.rotation, turnto2d, 20f*Time.deltaTime);
        cameratarget.transform.rotation = turnto;
    }
}
