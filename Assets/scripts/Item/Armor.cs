using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Armor : Item
{
    /*防具包含力量，防御，敏捷三个属性*/
    public int Power { get; private set; }
    public int Defend { get; private set; }
    public int mingjie { get; private set; }
    public Armor(int ID, string name, string Description, int buyPrice, int sellPrice, string Icon,int Power,int Defend,int mingjie) : base(ID, name, Description, buyPrice, sellPrice, Icon)
    {
        this.Power = Power;
        this.Defend = Defend;
        this.mingjie = mingjie;
        base.IDtype = "Armor";//设置好类型
    }
}

