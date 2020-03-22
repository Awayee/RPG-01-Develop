using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

//相机跟随，滚轮控制镜头
public class CFollow : MonoBehaviour
{
    Transform Player;
    Transform ThisTransf;
    Transform MainCamera;

    private float defaultDis;//相机默认距离
    public Vector3 targetOffset;//相机偏移量
    public float HDvdD = 0.618f;//相机高度和水平距离之比
    public float minDistance = 5;//相机最近距离
    public float maxDistance = 15;//相机最远距离

    public float FollowSpeed = 1;//相机跟进速度
    //private float scroll;//鼠标滚轮滚动量
    public float scrollSpeed = 1;//滚轮灵敏度
    ///public float minDistance;//最小变化距离，

    //Vector3 nowP,previousP;//玩家上一帧的位置//玩家当前位置
    //private bool canScale = false;//是否能缩放
    // Use this for initialization
    private bool following = false;
    void Start()
    {
        MainCamera = Camera.main.transform;
        ThisTransf = this.transform;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        defaultDis = 0.5f * (minDistance + maxDistance);
        MainCamera.transform.localPosition = new Vector3(0, HDvdD * defaultDis, -defaultDis);
        //调整相机仰角
        float mcAngle = 0.6f * defaultDis + 23;
        MainCamera.localEulerAngles = new Vector3(mcAngle, 0, 0);
        CameraFollow();
        //transform.position = previousP;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(following)
        {
            ThisTransf.position = Vector3.Lerp(ThisTransf.position, Player.position, 0.1f);
        }
        //transform.position = Vector3.Lerp(transform.position, Player.position, FollowSpeed);//相机跟随
#if UNITY_EDITOR_WIN
        //滚轮控制视野
        //print("Mouse Scrolled:" + Input.GetAxis("Mouse ScrollWheel"));
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {

            //print("Mouse Scrolled:" + Input.GetAxis("Mouse ScrollWheel"));
            float crntDis = GetViewDistance();
            float scroll = Input.GetAxis("Mouse ScrollWheel") * 10;
            SetViewDistance(crntDis-scroll);
        }
        /*    
        else
        {
            //提示框逐渐消失
            poptip.GetComponent<PopTip>().PopDownTip();
        }*/
#endif
        


    }

    public void CameraFollow()
    {
        following = true;
    }
    public void StopFollow()
    {
        following = false;
    }
    public float GetViewDistance()//得到相机距离
    {
        return -MainCamera.localPosition.z;
    }
    public void SetViewDistance(float dis)//设置相机距离
    {
        // print("dis: " +dis);
        if (dis > maxDistance) dis = maxDistance;
        else if (dis < minDistance) dis = minDistance;
        //Vector3 oldCamPos = MainCamera.localPosition;
        Vector3 targetPos = new Vector3(0, HDvdD * dis, -dis);//调整z轴位置
        MainCamera.localPosition = targetPos;
        //调整相机仰角
        float dAngle = 0.6f * dis + 23;
        MainCamera.localEulerAngles = new Vector3(dAngle, 0, 0);
        //显示提示框
        //TipMessage.Instance.Tip("视野距离：" + Mathf.CeilToInt(targetDis), 1);
    }
    public void ScaleView(float deltaDis)//调整相机距离
    {
        //Vector3 oldCamPos = MainCamera.localPosition;
        Vector3 targetPos = MainCamera.localPosition - new Vector3(0, HDvdD * deltaDis, -deltaDis);//调整z轴位置
        float targetDis = -targetPos.z;
        //MainCamera.LookAt(this.transform);//使相机时刻朝向目标
        //print("deltaDis:" + deltaDis);
        if (targetDis >= minDistance && targetDis <= maxDistance)
        {
            MainCamera.localPosition = targetPos;
            //调整相机仰角
            float dAngle = -0.6f * deltaDis;
            MainCamera.Rotate(new Vector3(dAngle, 0, 0));
        }
        //超出范围则调整数值
        else if (targetDis < minDistance)
        {
            MainCamera.localPosition = new Vector3(0, HDvdD * minDistance, -minDistance);
            //调整相机仰角
            float mcAngle = 0.6f * minDistance + 23;
            MainCamera.localEulerAngles = new Vector3(mcAngle, 0, 0);
            return;
        }
        else if (targetDis > maxDistance)
        {
            MainCamera.localPosition = new Vector3(0, HDvdD * maxDistance, -maxDistance);
            //调整相机仰角
            float mcAngle = 0.6f * maxDistance + 23;
            MainCamera.localEulerAngles = new Vector3(mcAngle, 0, 0);
            return;
        }
        //显示提示框
        //TipMessage.Instance.Tip("视野距离：" + Mathf.CeilToInt(targetDis), 1);
    }
    public void RotateView(float x, float y)//视野旋转，x为绕横轴旋转值，y为绕纵轴旋转值
    {
        transform.Rotate(new Vector3(x, y, 0), Space.World);
        //MainCamera.LookAt(this.transform);//使相机时刻朝向目标
    }
}
