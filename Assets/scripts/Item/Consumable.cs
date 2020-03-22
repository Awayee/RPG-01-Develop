using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    public class Consumable:Item
    {
        /*消耗品包括回血，回MP两个属性*/
        public int backHp { get; private set; }
        public int backMp { get; private set; }
        public Consumable(int ID, string name, string Description, int buyPrice, int sellPrice, string Icon,int backHp,int backMp) : base(ID, name, Description, buyPrice, sellPrice, Icon)
        {
            this.backHp = backHp;
            this.backMp = backMp;
            base.IDtype = "Consumable";//设置好类型
    }
}

