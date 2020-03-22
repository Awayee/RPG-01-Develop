using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//处理多点触控事件
public class TouchControl : MonoBehaviour
{
    [SerializeField] private RectTransform Ctr, CPoint, Arrow;//操作盘位置
    //Transform MainCamera;//主相机
    private float pointMax;//控制点滑动的最远距离
    //private Vector3 Direction;//滑动距离
    private PlayCharacter PlayerC;//玩家
    private Image ctrImg;//操作盘
    //private Color colorCtr;//操作盘颜色
    private Image pointImg;//控制点
    private Image arrowImg;//箭头
    private float a_Ctr, a_Bar, a_Arrow;//透明度
    private Vector2 ScrnSize;//屏幕尺寸
    private float w, h;    //坐标转换

    //触控位置,长度为设备最多支持的触控点数
    private Vector2[] StartPos = new Vector2[10];//手指开始触控时的位置，用于判断事件类型
    private Vector2[] MovedPos = new Vector2[10];//手指移动时触控点的的位置，用于处理事件
    int[] State = new int[10];//用于判断每个触控点处理的事件

    private List<int> CtrlIndex = new List<int>();//存放触控位置的索引列表，作标记用
    private float CStartDis;//用于缩放视野，多指开始触控时，记录距离
    //List<Vector3> CtrlPos = new List<Vector3>();//位于控制视角区域的控制点个数，提取自StartPos，用于缩放

    CFollow cameraF;//跟随相机，用于视野缩放
    float cameraDis;//当前相机距离

    public float rotateDel = 0.1f;//镜头旋转系数   
    public float scaleDel = 0.1f;//视野缩放系数
    [SerializeField] private bool ctrlViewRotate = true;//决定是否能旋转视野
    [SerializeField] private bool ctrlViewScale = true;//决定是否能缩放视野
    private bool temp_scaleView;//用于记录预设
    public bool ctrlMove = true;//决定摇杆是否可用

    //private playercontrol playercontrol;//windows控制不冲突用

    // Use this for initialization
#if UNITY_EDITOR_WIN 
    bool running = false;
#endif
    void Start()
    {
        //获取控制盘相关对象
        ctrImg = Ctr.GetComponent<Image>();
        pointImg = CPoint.GetComponent<Image>();
        arrowImg = Arrow.GetComponent<Image>();
        //获取玩家的移动脚本
        PlayerC = GameObject.FindObjectOfType<PlayCharacter>();
        //GameObject CameraTarget = GameObject.FindGameObjectWithTag("CameraTarget");
        cameraF = GameObject.FindObjectOfType<CFollow>();
        //MainCamera = Camera.main.transform;
        //得到坐标转换因子
        w = GameManager.Instance.wRatio;
        h = GameManager.Instance.hRatio;
        ScrnSize = GameManager.Instance.screenSize;//屏幕尺寸
        //print("W:" + w + ", H: " + h + "HalsScreenSize: " + ScrnSize);

        //初始化
        //Direction = Vector3.zero;
        pointMax = Ctr.sizeDelta.x * 0.5f - CPoint.sizeDelta.x * 0.5f;//控制点滑动的最远距离
        //print(maxDis);
        // 记录透明度
        a_Ctr = ctrImg.color.a;
        a_Bar = pointImg.color.a;
        a_Arrow = arrowImg.color.a;
        HideController();//隐藏摇杆

        temp_scaleView = ctrlViewScale;
        Input.multiTouchEnabled = true;//开启多点触控

        //playercontrol = Player.GetComponent<playercontrol>();//windows控制不冲突用

    }

    // Update is called once per frame
    void Update()
    {
        #region Windows下的输入事件 

#if UNITY_EDITOR_WIN

        //按下WASD移动
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
            PlayerC.Move(new Vector2(h, v));
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            PlayerC.StopMove();
        }
        //按Q和E旋转视野
        if (Input.GetKey(KeyCode.Q))
        {
            if (ctrlViewRotate)
                cameraF.RotateView(0, -20 * rotateDel);
        }
        if (Input.GetKey(KeyCode.E))
        {
            if (ctrlViewRotate)
                cameraF.RotateView(0, 20 * rotateDel);
        }
        //PlayerC.Move(new Vector3(h, 0, v));
        if (Input.GetKeyDown(KeyCode.Space))//空格键跳跃
        {
            PlayerC.Jump();
        }
        if (Input.GetKeyDown(KeyCode.J))//冲刺
        {
            PlayerC.Rush();
        }
        if (Input.GetKeyDown(KeyCode.H))//死亡
        {
            PlayerC.Die();
        }
        //Shift键加速
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            running = PlayerC.moveSpeed <= (PlayerC.runSpeed + PlayerC.walkSpeed) / 2 ? false : true;
            //print("running:" + running);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            PlayerC.moveSpeed += 0.5f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))//松开恢复速度
        {
            PlayerC.moveSpeed = running ? PlayerC.runSpeed : PlayerC.walkSpeed;
        }
        if (Input.GetKeyDown(KeyCode.F))//攻击
        {
            PlayerC.NormalAttack();
        }

#endif

        #endregion

        //print(Direction);
        if (!ctrlMove && !ctrlViewRotate && !ctrlViewScale) return;//如果都不能控制

        //触控点数
        if (Input.touchCount == 0) return;
        //多点触控操作
        else if (Input.touchCount > 0)
        {
            int tCount = Input.touchCount;
            Touch[] inputTouches = Input.touches;
            //print("touchCoutn:" + tCount);
            // bool viewScaled = false;//判断视野是否已缩放
            // bool scaledEnd = false;//缩放结束
            for (int i = 0; i <= tCount - 1; i++)
            {
                //-------------------开始触控---------------------------------
                if (inputTouches[i].phase == TouchPhase.Began)
                {
                    // Ray ray = Camera.main.ScreenPointToRay(inputTouches[i].position);//从屏幕向场景发射一条射线
                    // 	RaycastHit hit;//
                    //     if(Physics.Raycast (ray, out hit, 1)){
                    //         print(hit.transform.name.ToString());
                    //         }

                    //先判断是否点击到了UI上的按钮
                    if (EventSystem.current.IsPointerOverGameObject(inputTouches[i].fingerId))
                    {
                        //print("点击到了按钮");
                        continue;//终止本次循环
                    }

                    int fID = inputTouches[i].fingerId;//利用FingerID来辨识触控点，以获取触控点对应的功能
                    //获取初始坐标
                    StartPos[fID] = inputTouches[i].position;
                    MovedPos[fID] = inputTouches[i].position;
                    //print("StartPosition[" + fID + "]:" + StartPos[fID]);

                    /*摇杆感应区域，
                     * 作为操作盘或滑屏的判定条件，touches.phase的所有函数中对应事件的判定条件应一致
                     * 屏幕左下区域：StartPos[fID].y < ScreenSize.y / 2 && StartPos[fID].x < ScreenSize.x / 2
                     * 屏幕左下宽1/3，高1/2区域：StartPos[fID].y < ScreenSize.y / 2 && StartPos[fID].x < ScreenSize.x / 3
                     * 屏幕左下以1/2高为半径的扇形区域：StartPos[fID].magnitude < ScreenSize.y / 2
                     */
                    if (StartPos[fID].y < 0.5f * ScrnSize.y && StartPos[fID].x < ScrnSize.x / 3)
                    {
                        if (!ctrlMove)
                        {
                            PlayerC.StopMove();
                            continue;
                        }
                        //print("移动");
                        State[fID] = 1;
                        //转换坐标
                        StartPos[fID] = ScreenToUI(StartPos[fID]);
                        //显示操作盘
                        ShowController(StartPos[fID]);
                    }

                    /*滑动控制视角的区域
                     *不能与摇杆区域重叠
                     *屏幕上半部分：StartPos[fID].y > ScreenSize.y / 2
                     *屏幕上中2/3区域：StartPos[fID].y > ScreenSize.y / 2 && (StartPos[fID].x > ScreenSize.x / 6 && StartPos[fID].x<ScreenSize.x * 5 /6)
                     *屏幕中间1/3区域：StartPos[fID].x > ScreenSize.x / 3 && StartPos[fID].x < ScreenSize.x * 2 / 3
                     *屏幕下中及上半部分T形区域：(StartPos[fID].x > ScreenSize.x / 3 && StartPos[fID].x < ScreenSize.x * 2 / 3) || StartPos[fID].y > ScreenSize.y / 2
                     */
                    else if (StartPos[fID].y > 0.5f || StartPos[fID].y < 0.5f && StartPos[fID].x > ScrnSize.x / 3 && StartPos[fID].x < ScrnSize.x * 2 / 3)
                    {

                        //if (!ctrlViewRotate) continue;
                        //print("控制视野");
                        State[fID] = 2;
                        //存放索引，标记为控制区域内的点
                        //当出现多个控制区域内的点时，取前两个
                        if (CtrlIndex.Count < 2)
                        {
                            CtrlIndex.Add(fID);
                            if (CtrlIndex.Count == 2 && ctrlViewScale) cameraDis = cameraF.GetViewDistance();//记录此时的相距离
                            // print("cameraDis: " + cameraDis);
                        }

                    }

                }
                //-------------手指悬停----------------------------
                /*
                else if( inputTouches[i].phase == TouchPhase.Stationary)
                {
                    int fID = inputTouches[i].fingerId;
                    if (State[fID] == 1)
                    {
                        //print("移动");
                        MovedPos[fID] = ScreenToUI(inputTouches[i].position);
                        Vector3 dPos = MovedPos[fID] - StartPos[fID];
                        //交互反馈
                        ControllerInteract(dPos);
                        //print("DPos:" + dPos);
                    }
                    if (State[fID] == 2)
                    {
                        // StartPos[i] = inputTouches[i].position;//以悬停时的触控点作为视野控制的起点
                        // print("startPos[" + i + "]:" + StartPos[i]);
                    }

                }
                */
                //-------------手指移动------------------------------
                else if (inputTouches[i].phase == TouchPhase.Moved || inputTouches[i].phase == TouchPhase.Stationary)
                {
                    int fID = inputTouches[i].fingerId;
                    //print("控制点索引："+i);
                    //print("控制点数量：" + Input.touchCount);
                    //print("控制点：" + StartPos[i]);

                    //角色移动
                    if (State[fID] == 1)
                    {
                        if (!ctrlMove)
                        {
                            PlayerC.StopMove();
                            continue;
                        }
                        //print("移动");
                        MovedPos[fID] = ScreenToUI(inputTouches[i].position);
                        //Vector2 deltaPos = VectorToUI(inputTouches[i].deltaPosition);
                        Vector3 dPos = MovedPos[fID] - StartPos[fID];
                        //print("DPos:" + dPos);
                        ControllerInteract(dPos);
                    }

                    //视野控制
                    else if (State[fID] == 2)
                    {

                        //print(CtrlIndex.Count);
                        //Vector3 deltaPos =VectorToUI(Input.touches[i].deltaPosition);//向量转换
                        //print("CtrlPos.Count:" + CtrlPos.Count);
                        //print("deltaPos:"+deltaPos);
                        //print("Points:" + CtrlPoints);

                        //单指拖动旋转镜头
                        if (CtrlIndex.Count == 1 && ctrlViewRotate)
                        {
                            Vector3 deltaPos = inputTouches[i].deltaPosition;//触摸点偏移量
                            float DeltaR = deltaPos.x;
                            cameraF.RotateView(0, DeltaR * rotateDel);//旋转相机

                        }
                        // 多指张合缩放视野
                        else if (CtrlIndex.Count >= 2 && ctrlViewScale)
                        {
                            MovedPos[fID] = inputTouches[i].position;
                            //计算缩放值
                            CStartDis = Vector3.Distance(StartPos[CtrlIndex[0]], StartPos[CtrlIndex[1]]);//上一组触控点的距离
                            float movedDis = Vector2.Distance(MovedPos[CtrlIndex[0]], MovedPos[CtrlIndex[1]]);//新触控点距离
                            float DeltaS = movedDis - CStartDis;//取差值
                            cameraF.SetViewDistance(cameraDis - scaleDel * DeltaS);//设置相机距离
                        }
                    }

                    //print("Touch Position:"+pos);
                }

                //-------------手指离开----------------------------
                else if (inputTouches[i].phase == TouchPhase.Ended)
                {
                    int fID = inputTouches[i].fingerId;
                    if (State[fID] == 1)
                    {
                        CPoint.localPosition = Vector3.zero;
                        //Direction = Vector3.zero;
                        PlayerC.StopMove();//角色停止移动
                        //隐藏控件
                        HideController();

                    }
                    else if (State[fID] == 2)
                    {
                        if (ctrlViewScale) cameraDis = cameraF.GetViewDistance();
                        CtrlIndex.Remove(fID);//清除控制点索引
                    }

                    State[fID] = 0;//数组元素置空
                    MovedPos[fID] = Vector2.zero;//清除
                    StartPos[fID] = Vector2.zero;//触控点位置
                    CStartDis = 0;


                }
            }
        }
    }

    float VectorAngle(Vector2 from, Vector2 to)//计算向量夹角(得到-180度到180度的角）
    {
        Vector3 cross = Vector3.Cross(from, to);
        float angle = Vector2.Angle(from, to);
        return cross.z > 0 ? -angle : angle;
    }
    private Vector2 ScreenToUI(Vector2 ScreenPos)    //坐标转换（屏幕坐标转换为UI坐标）
    {
        Vector2 TransPos = ScreenPos;
        TransPos -= 0.5f * ScrnSize;
        return new Vector2(w * TransPos.x, h * TransPos.y);
    }

    private Vector2 VectorToUI(Vector2 ScreenVector)    //向量转换（方向适配，不用平移坐标）
    {
        Vector2 TransVec = ScreenVector;
        return new Vector2(w * TransVec.x, h * TransVec.y);
    }

    private void ShowController(Vector2 location)//显示摇杆
    {
        ctrImg.color = new Color(ctrImg.color.r,
                                 ctrImg.color.g,
                                 ctrImg.color.b, a_Ctr);
        pointImg.color = new Color(pointImg.color.r,
                                    pointImg.color.g,
                                    pointImg.color.b, a_Bar);

        Ctr.localPosition = location;
        //开启操作盘动画
        StopAllCoroutines();
        StartCoroutine(ControllerFadeIn(location));
    }

    private void HideController()//隐藏摇杆
    {

        pointImg.color = new Color(pointImg.color.r,
                                    pointImg.color.g,
                                    pointImg.color.b, 0);
        arrowImg.color = new Color(arrowImg.color.r,
                                   arrowImg.color.g,
                                   arrowImg.color.b, 0);
        //开始 动画
        StopAllCoroutines();
        StartCoroutine(ControllerFadeOut());

    }
    private IEnumerator ControllerFadeIn(Vector2 location)//摇杆渐显效果
    {
        Ctr.localPosition = location;
        ctrImg.color = new Color(ctrImg.color.r, ctrImg.color.g, ctrImg.color.b, 1);//暂时变为黑色
        ctrImg.rectTransform.sizeDelta = pointImg.rectTransform.sizeDelta;//暂时调整操作盘的尺寸

        float t = Time.unscaledTime;
        float dtime = 0;
        while (dtime <= 1)
        {
            dtime = (Time.unscaledTime - t) * 2;
            //加速动画
            ctrImg.color = new Color(ctrImg.color.r, ctrImg.color.g, ctrImg.color.b, Mathf.Lerp(ctrImg.color.a, a_Ctr, dtime));//颜色变浅
            ctrImg.rectTransform.sizeDelta = Vector2.Lerp(ctrImg.rectTransform.sizeDelta, 220 * Vector2.one, dtime);//变大
            yield return new WaitForEndOfFrame();
        }
        yield break;

    }
    private IEnumerator ControllerFadeOut()//摇杆渐隐效果
    {
        // ctrImg.color = Color.black;//暂时变为黑色
        // ctrImg.rectTransform.sizeDelta = Vector2.one;//暂时调整操作盘的尺
        //记录此时的透明度和尺寸
        float _a = ctrImg.color.a;
        Vector2 _size = ctrImg.rectTransform.sizeDelta;
        float t = Time.unscaledTime;
        float dtime = 0;
        while (dtime <= 1)
        {
            dtime = (Time.unscaledTime - t) * 2;
            //匀速动画
            ctrImg.color = new Color(ctrImg.color.r, ctrImg.color.g, ctrImg.color.b, Mathf.Lerp(_a, 0, dtime));//颜色变浅直至消失
            ctrImg.rectTransform.sizeDelta = Vector2.Lerp(_size, 300 * Vector2.one, dtime);//变大
            yield return new WaitForEndOfFrame();
        }
        Ctr.localPosition = new Vector3(0, -1080, 0);
        yield break;

    }

    private void ControllerInteract(Vector2 deltaV)//摇杆交互
    {
        CPoint.localPosition = deltaV;//定位控制点
        float vmg = deltaV.magnitude;
        if (vmg > 20)
        {//触发移动的阈值
            //显示箭头
            float angle = VectorAngle(new Vector2(0, 1), deltaV);
            Arrow.localRotation = Quaternion.Euler(0, 0, -angle);
            arrowImg.color = new Color(arrowImg.color.r,
                                        arrowImg.color.g,
                                        arrowImg.color.b, a_Arrow);
            PlayerC.Move(deltaV);//玩家移动
            if (vmg > pointMax)//如果控制点移出摇杆外，则使控制点停靠在边缘
                CPoint.localPosition = deltaV.normalized * pointMax;

        }
        else
        {//隐藏箭头
            arrowImg.color = new Color(arrowImg.color.r,
                                        arrowImg.color.g,
                                        arrowImg.color.b, 0);

            PlayerC.StopMove();//玩家停止移动
        }



    }

    public void EnableTouchMove()//启用触摸移动
    {
        ctrlMove = true;
    }
    public void DisableTouchMove()//禁用触摸移动
    {
        ctrlMove = false;
        HideController();
    }
    public void EnableTouchControlView()//启用触摸控制视野
    {
        ctrlViewRotate = true;
        ctrlViewScale = temp_scaleView;
    }
    public void DisableTouchControlView()//禁用触摸控制视野
    {
        ctrlViewRotate = false;
        ctrlViewScale = false;
    }
}
