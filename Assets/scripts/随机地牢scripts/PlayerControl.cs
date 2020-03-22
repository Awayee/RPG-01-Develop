using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public int Speed=10;//设置移动的速度

    public static Vector3 PlayerPos;//获得玩家的坐标
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");
        transform.Translate(h*Speed*Time.deltaTime,0,v * Speed * Time.deltaTime, Space.World);
        PlayerPos=new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void OnTriggerEnter(Collider col)
    {
        Destroy(col.gameObject);
        Debug.Log("获得宝物了！！");
    }
}
