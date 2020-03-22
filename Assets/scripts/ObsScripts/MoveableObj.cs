using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObj : MonoBehaviour
{

    Transform player;//获得玩家
    PlayerObj playerOb;//玩家脚本
    Transform thisTransf;//该物体
    //public GameObject obsmanager;//获得物品管理器
    public ObsType objType;//物品类型
    public string objName;//物品名称
    //public string Type;//物品类别
    [SerializeField] float minDistance = 4;//触发的距离
    //private float distance;//获得物品与玩家间的距离
    private bool entered = false;//判断是否已进入区域
    
    ButtonObjMove moveObjButton;//对应按钮预制体
    
    private ButtonObjMove mButton;//按钮方法

    //private int Type = 1;//0代表cube,1代表木头,2代表石头
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        moveObjButton = Resources.Load<ButtonObjMove>("Prefabs/UIElement-Button-MoveObj");
        playerOb = player.GetComponent<PlayerObj>();//玩家脚本
        thisTransf = this.transform;
        entered = false;
    }

    // Update is called once per frame
    
    void Update()
    {
        if (playerOb.picked) return;
        float distance = Vector3.Distance(thisTransf.position, player.position);//获得物品与玩家间的距离
        //进入感应范围
        if (distance <= minDistance)
        {
            if (entered) return;
            else
            {
                EnableButton();
                entered = true;
            }
            
        }
        else//走出感应范围
        {
            if (!entered) return;
            else
            {
                if(mButton!=null)mButton.DestroyObjButton(2);
                entered = false;
            }
        }
    }
    
    void EnableButton()//激活按钮预制体
    {
        //print("进入物品区域");
        mButton = Instantiate(moveObjButton);
        mButton.SetPlayerAndObj(playerOb, thisTransf);
        //mButton.SetObjType(transform, objType);
        mButton.Fade_Appear(0.2f);

    }
    public void PickUpThisObj()//玩家捡起该物体
    {
        player.GetComponent<PlayerObj>().PickUpObs(thisTransf, objType);
    }
}