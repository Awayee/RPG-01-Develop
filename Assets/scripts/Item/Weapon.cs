using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Weapon: Item
    {
        /*武器包括破坏了一个属性*/
        public int damage { get; private set; }

        public Weapon(int ID, string name, string Description, int buyPrice, int sellPrice, string Icon,int damage):base( ID,  name,Description,buyPrice,sellPrice, Icon)
        {
            this.damage = damage;
            base.IDtype = "Weapon";//设置好类型
    }
        
}

