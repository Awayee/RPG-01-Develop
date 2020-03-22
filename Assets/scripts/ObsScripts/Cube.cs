using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{

    public GameObject player;//获得玩家
    public GameObject obsmanager;//获得物品管理器

    private float distance;//获得物品与玩家间的距离

    private int Type = 0;//0代表cube,1代表木头,2代表石头
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(this.transform.position, player.transform.position);//获得物品与玩家间的距离
        if (distance < 4)
        {
            bool isMin = obsmanager.GetComponent<ObsManager>().IsMinDistance(distance);//判断是不是最近的距离
            if (isMin)
            {
                obsmanager.GetComponent<ObsManager>().SetObsType(Type);
            }
        }
    }
}