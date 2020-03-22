using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smokeself : MonoBehaviour {
    private float startsize;//初始自己大小的系数
    private float goalsize;//目标自己大小的系数
    private Rigidbody2D rigid;//自身刚体组件
    private Vector3 statlocalscale;//起初自身的大小
    private Vector3 goallocalscale;//最大自身的大小
    private bool canbigger=true;//是否变大标志
    // Use this for initialization
    void Start () {
        
        transform.localRotation=Quaternion.LookRotation(new Vector3(Random.Range(0f, 180f), Random.Range(0f, 180f), Random.Range(0f, 180f)));//随机生成角度并转化成转换成四元数
        startsize = Random.Range(0.3f, 0.5f);//随机产生大小系数
        goalsize = Random.Range(1.2f, 1f);//随机产生大小系数
        goallocalscale = goalsize*transform.localScale;//先确定最终的大小
        transform.localScale = startsize * transform.localScale;
        statlocalscale= startsize * transform.localScale;//获得起初自身的大小
        Invoke("destoryself", 3f);
    }
    void destoryself()
    {
        Destroy(gameObject);
    }
	// Update is called once per frame
	void Update () {
        if(transform.localScale.x > goallocalscale.x * 0.9f)
            {
            canbigger = false;
            }

        if (canbigger)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, goallocalscale, Time.deltaTime *2f);
        }
        else {
            transform.localScale = Vector3.Lerp(transform.localScale,new Vector3(0,0,0), Time.deltaTime * 3f);
        }
        if (transform.localScale.x<0.05f)
        {
            Destroy(gameObject);
        }
    }
}
