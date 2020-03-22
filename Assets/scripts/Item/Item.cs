using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item  {
    public int ID { get; private set; }//物品ID
    public string name { get; private set; }//物品名称
    public string Description{ get; private set; }//物品描述

    public int buyPrice { get; private set; }//获取购买价格
    public int sellPrice { get; private set; }//获取出售价格

    public string Icon { get; private set; }//获取图片，是一个路径
    public string IDtype { get; protected set; }//用于设置类型

    /*添加一个构造函数*/
    public Item(int ID, string name, string Description, int buyPrice, int sellPrice, string Icon)
    {
        this.ID = ID;
        this.name = name;
        this.Description = Description;
        this.buyPrice = buyPrice;
        this.sellPrice = sellPrice;
        this.Icon = Icon;
    }
}
