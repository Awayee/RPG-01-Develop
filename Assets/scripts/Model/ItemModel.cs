using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//该类用于存储数据
public static class ItemModel  {

	private  static Dictionary<string,Item> GridItem=new Dictionary<string, Item>();//创建一个字典来存储数据



    //存储
    public static void StoreItem(string name,Item item)
    {
        //如果字典中已经有这个key了，采取删除再添加的方式
        if (GridItem.ContainsKey(name))
        {
            DeleteItem(name);
        }
        GridItem.Add(name,item);
    }
    //读取
    public static Item GetItem(string name)//这里的name是网格的名字
    {
        if (GridItem.ContainsKey(name))
        {
            return GridItem[name];
        }
        else
        {
            return null;
        }
    }
    //删除
    public static void DeleteItem(string name)
    {
        if (GridItem.ContainsKey(name))
        {
            GridItem.Remove(name);
        }
    }
}
