using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanalUI : MonoBehaviour
{
    public List<Transform> nums=new List<Transform>();
    public Transform[] Grids;

    public Transform GetEmpty()
    {
        for (int i = 0; i < Grids.Length; i++)
        {
            if (Grids[i].childCount==0)//如果该背包格是空的，也就是说没有子物体
            {
                return Grids[i];
            }

        }

        return null;//如果格子全满，返回空
    }

    //获得一个字典，存放Item信息
    public List<Transform> FindItem()
    {
        if (nums.Count != 0)
        {
            nums.Clear();
        }
        for (int i = 0; i < Grids.Length; i++)
        {
            if (Grids[i].childCount != 0)
            {
                nums.Add(Grids[i]);
                Destroy(Grids[i].transform.GetChild(0).gameObject);
            }

        }

        return nums;
    }



    //获得指定的格子
    public Transform GetGridname(int num)
    {
        return Grids[num];
    }
}
