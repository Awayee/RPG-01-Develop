using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsManager : MonoBehaviour
{
    private int CurrentType;//用于设置当前离玩家最近的物品类型
    private float CurrentDistance=10000;//保存当前离玩家最近的物品的距离

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        /*
        if (CurrentType==0)
        {
            Debug.Log("物品为Cube");
        }
        if (CurrentType == 1)
        {
            Debug.Log("物品为木头");
        }
        if (CurrentType == 2)
        {
            Debug.Log("物品为石头");
        }
         */
    }
    //用于设置离玩家最近的物品类型
    public void SetObsType(int type)
    {
        CurrentType = type;
    }

    public bool IsMinDistance(float dis)
    {
        bool isMin = false;
        if (dis<CurrentDistance)
        {
            dis = CurrentDistance;
            isMin= true;
        }

        return isMin;
    }

}
