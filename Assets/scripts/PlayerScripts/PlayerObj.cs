using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : MonoBehaviour {

    PlayCharacter playCh;
    Animator playAnmtr;

    [Header("ObjectOptions")]
    public GameObject Wood;//绑定角色身上的木头
    public GameObject Stone;//绑定角色身上的石头
    public GameObject Cube;//绑定角色身上的Cube

    public GameObject WoodInstance;//绑定要生成到地上的木头
    public GameObject StoneInstance;//绑定要生成到地上的石头
    public GameObject CubeInstance;//绑定要生成到地上的Cube

    private GameObject CurrentActiveObs;//当前活跃的物品
    public GameObject WoodLeftHand;//绑定左手拿木头的IK位置
    public GameObject WoodRightHand;//绑定右手拿木头的IK位置
    public GameObject StoneLeftHand;//绑定左手拿石头的IK位置
    public GameObject StoneRightHand;//绑定右手拿石头的IK位置
    public GameObject CubeLeftHand;//绑定左手拿Cube的IK位置
    public GameObject CubeRightHand;//绑定右手拿Cubes的IK位置

    //public Obs[] obs;//获得物品列表的数组
    //public Obs CurrentObs;//得到当前的物品

    public bool picked;//已捡到物体

    public ObsType CurrentObsType;//0代表cube，1代表木头，2代表石头
    public string objName;//拿起的物品名称

    // Use this for initialization
    void Start () {
        playCh = GetComponent<PlayCharacter>();
        playAnmtr = playCh.animator;
	}
	
    #region 物品操作

    //捡起物品
    public void PickUpObs(Transform obj, ObsType obType)
    {
        //TurnAt(obj);
        if (playAnmtr.GetBool("isHoldObs") == false)
        {
            playCh.TurnAtRightNow(obj);//转向物体
            playCh.DisableMove();
            Invoke("EnableMove", 1.4f);//一定时间后启用移动

            playAnmtr.SetTrigger("isPickUpObs");
            playAnmtr.SetBool("isHoldObs", true);
            Destroy(obj.gameObject, 0.5f);//销毁场景中的该物体
            Invoke("SetObsAlive", 0.5f);

            //记录
            CurrentObsType = obType;
            objName = obj.GetComponent<MoveableObj>().objName;

        }

        //量词
        string c = null;
        switch (obType){
            case ObsType.Cube:
                c = "个";
                break;
            case ObsType.Stone:
                c = "块";
                break;
            case ObsType.Wood:
                c = "棵";
                break;
            default:
                c = null;
                break;
        }

        picked = true;
        GameManager.Instance.Tip("拿起一" + c + objName);
    }
    void EnableMove()//启用移动
    {
        playCh.EnableMove();
    }
    //放下物品
    public void TakeOffObs()
    {
        if (playAnmtr.GetBool("isHoldObs") == true)
        {
            playCh.DisableMove();
            Invoke("EnableMove", 1.4f);
            playAnmtr.SetTrigger("isTakeOffObs");
            playAnmtr.SetBool("isHoldObs", false);
            Invoke("SetObsDisalive", 0.5f);
        }
        picked = false;
        GameManager.Instance.Tip("放下" + objName);

    }
    //将物品激活
    public void SetObsAlive()
    {
        if (CurrentObsType == ObsType.Cube)
        {
            Cube.SetActive(true);
        }

        else if (CurrentObsType == ObsType.Wood)
        {
            Wood.SetActive(true);
        }
        else if (CurrentObsType == ObsType.Stone)
        {
            Stone.SetActive(true);
        }
    }
    //将物品取消激活
    public void SetObsDisalive()
    {
        if (CurrentObsType == ObsType.Cube)
        {
            Cube.SetActive(false);
            Instantiate(CubeInstance, this.transform.position + transform.forward * 2 + transform.up, Quaternion.identity);
        }

        if (CurrentObsType == ObsType.Wood)
        {
            Wood.SetActive(false);
            Instantiate(WoodInstance, this.transform.position + transform.forward * 2 + transform.up, Quaternion.identity);
        }
        if (CurrentObsType == ObsType.Stone)
        {
            Stone.SetActive(false);
            Instantiate(StoneInstance, this.transform.position + transform.forward * 2 + transform.up, Quaternion.identity);
        }
    }
    #endregion

    //用于设置手的IK位置
    private void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex == 1)//说明当前是被HoldLog这一层调用的
        {
            //weight用于设置动画的权重
            int weight = playAnmtr.GetBool("isHoldObs") ? 1 : 0;

            if (CurrentObsType == ObsType.Cube)
            {
                //使用SetIKPosition来设置IK点，参数分别为：匹配的部位，匹配的点（可以通过挂载上空物体来直接获取那个店）
                playAnmtr.SetIKPosition(AvatarIKGoal.LeftHand, CubeLeftHand.transform.position);
                playAnmtr.SetIKRotation(AvatarIKGoal.LeftHand, CubeLeftHand.transform.rotation);
                //一下是设置IK的权重，因为OnAnimatorIk会一直调用，所以就靠权重来控制它是否受影响。
                playAnmtr.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
                playAnmtr.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
                playAnmtr.SetIKPosition(AvatarIKGoal.RightHand, CubeRightHand.transform.position);
                playAnmtr.SetIKRotation(AvatarIKGoal.RightHand, CubeRightHand.transform.rotation);
                playAnmtr.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
                playAnmtr.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
            }

            if (CurrentObsType == ObsType.Wood)
            {
                //使用SetIKPosition来设置IK点，参数分别为：匹配的部位，匹配的点（可以通过挂载上空物体来直接获取那个店）
                playAnmtr.SetIKPosition(AvatarIKGoal.LeftHand, WoodLeftHand.transform.position);
                playAnmtr.SetIKRotation(AvatarIKGoal.LeftHand, WoodLeftHand.transform.rotation);
                //一下是设置IK的权重，因为OnAnimatorIk会一直调用，所以就靠权重来控制它是否受影响。
                playAnmtr.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
                playAnmtr.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
                playAnmtr.SetIKPosition(AvatarIKGoal.RightHand, WoodRightHand.transform.position);
                playAnmtr.SetIKRotation(AvatarIKGoal.RightHand, WoodRightHand.transform.rotation);
                playAnmtr.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
                playAnmtr.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
            }
            if (CurrentObsType == ObsType.Stone)
            {
                //使用SetIKPosition来设置IK点，参数分别为：匹配的部位，匹配的点（可以通过挂载上空物体来直接获取那个店）
                playAnmtr.SetIKPosition(AvatarIKGoal.LeftHand, StoneLeftHand.transform.position);
                playAnmtr.SetIKRotation(AvatarIKGoal.LeftHand, StoneLeftHand.transform.rotation);
                //一下是设置IK的权重，因为OnAnimatorIk会一直调用，所以就靠权重来控制它是否受影响。
                playAnmtr.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
                playAnmtr.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
                playAnmtr.SetIKPosition(AvatarIKGoal.RightHand, StoneRightHand.transform.position);
                playAnmtr.SetIKRotation(AvatarIKGoal.RightHand, StoneRightHand.transform.rotation);
                playAnmtr.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
                playAnmtr.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
            }
        }
    }

}
