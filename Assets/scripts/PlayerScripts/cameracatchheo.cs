using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameracatchheo : MonoBehaviour {
    GameObject hero;
    Vector3 deta;

    //鼠标控制转向；
    public GameObject chaiYouJi;
    private float axisX;//鼠标沿水平方向移动的增量
    private float axisY;//鼠标沿竖直方向移动的增量

    private Vector3 startPos;//一开始鼠标的位置
    private Vector3 nowPos;//鼠标的位置
    private Vector3 latePos;//延迟鼠标的位置
   //物体移动
    private float scaleMin = 5f;
    public float scaleMax = 15f;

    // Use this for initialization
    void Start () {
        hero = GameObject.FindGameObjectWithTag("Player");
        deta = transform.position - hero.transform.position;
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 trueposition= hero.transform.position + deta;
        transform.position = new Vector3(trueposition.x, trueposition.y, trueposition.z);

        //物体旋转

        
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;//一开始获得鼠标位置
        }




        if (Input.GetMouseButton(0))
        {


            nowPos = Input.mousePosition;//获得鼠标位置


            //判断一开始鼠标是否移动
            if (nowPos != startPos)
            {
                //按下期间鼠标是否移动
                if (nowPos != latePos)
                {
                    axisX = (nowPos.x - startPos.x) * Time.deltaTime;
                    axisY = 0;
                }
                else
                {
                    axisX = 0;
                    axisY = 0;
                }
            }
            else
            {
                axisX = 0;
                axisY = 0;
            }
        }
        else
        {
            axisX = 0;
            axisY = 0;
        }


        this.transform.Rotate(new Vector3(axisY, axisX, 0), Space.World);


    

    
	}
}
