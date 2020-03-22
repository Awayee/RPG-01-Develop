using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
public class BagManager : MonoBehaviour
{
    //获取XML文件中的文本
    public List<string> InnerText;

    //private string filePath = Application.temporaryCachePath;
    private string filePath;//用于存储XML文件的路径


    //Xml文件类 该类没有继承MonoBehaviour
    public  XmlDocument ConsumXml;
    XmlElement Item1;

    public static XmlDocument WeaponXml = new XmlDocument();
    XmlElement Item2 = WeaponXml.CreateElement("Item");

    public static XmlDocument ArmorXml = new XmlDocument();
    XmlElement Item3 = ArmorXml.CreateElement("Item");



    //单例模式
    private static BagManager _instance;
    public static BagManager Instance
    {
        get
        {
            if (null == _instance)//如果未实例化
            {
                //新建空物体并绑定脚本
                GameObject bagManager = new GameObject("bagManager");
                _instance = bagManager.GetComponent<BagManager>();
                if (null == _instance)
                {
                    _instance = bagManager.AddComponent<BagManager>();
                }

            }
            return _instance;
        }
    }

    public Dictionary<int, Item> ItemList = new Dictionary<int, Item>();//这里的int相当于int类型得key，用于索引

    //启动时执行
    void Awake()
    {
        ConsumXml = new XmlDocument();
        Item1 = ConsumXml.CreateElement("Item");
        filePath = Application.temporaryCachePath;
        //单例
        //_instance = this;

        //事件监听
        ItemList = new Dictionary<int, Item>();
    }

    public void SaveConsumByXml(List<Consumable> consus, bool reWrite)//存入列表变量
    {
        if (reWrite)//是否删除所有数据并重写
        {
            Item1.RemoveAll();
        }

        //print("length of consus: " + consus.Count);
        if (consus.Count == 0)
        {
            ConsumXml.AppendChild(Item1);
            ConsumXml.Save(filePath + "/Consumable.xml");
        }
        else
        {
            for (int i = 0; i < consus.Count; i++)
            {
                //print(consus[i].ID.ToString());
                SaveConsumByXml(consus[i].ID.ToString(), consus[i].name, consus[i].Description,
                consus[i].buyPrice.ToString(), consus[i].sellPrice.ToString(), consus[i].Icon,
                consus[i].backHp.ToString(), consus[i].backMp.ToString());
            }
        }


    }

    //存储消耗品的XML数据
    public void SaveConsumByXml(string ID, string Name, string Descript, string buyprice, string sellprice, string icon, string backhp, string backmp)
    {
        //消耗品的存储路径
        //filepath = Application.temporaryCachePath + "/Consumable.xml";



        //创建consumable节点
        XmlElement consumable = ConsumXml.CreateElement("consumable");
        //创建name节点
        XmlElement name = ConsumXml.CreateElement("name");
        //创建Description节点
        XmlElement Description = ConsumXml.CreateElement("Description");
        //创建buyPrice节点
        XmlElement buyPrice = ConsumXml.CreateElement("buyPrice");
        //创建sellPrice节点
        XmlElement sellPrice = ConsumXml.CreateElement("sellPrice");
        //创建Icon节点
        XmlElement Icon = ConsumXml.CreateElement("Icon");
        //创建backHp节点
        XmlElement backHp = ConsumXml.CreateElement("backHp");
        //创建backMp节点
        XmlElement backMp = ConsumXml.CreateElement("backMp");


        //设置consumable节点的属性ID值
        consumable.SetAttribute("ID", ID);
        //设置title节点的文本
        name.InnerText = Name;
        Description.InnerText = Descript;
        buyPrice.InnerText = buyprice;
        sellPrice.InnerText = sellprice;
        Icon.InnerText = icon;
        backHp.InnerText = backhp;
        backMp.InnerText = backmp;

        //将price节点设置为consumable子节点
        consumable.AppendChild(name);
        consumable.AppendChild(Description);
        consumable.AppendChild(buyPrice);
        consumable.AppendChild(sellPrice);
        consumable.AppendChild(Icon);
        consumable.AppendChild(backHp);
        consumable.AppendChild(backMp);


        //将consumable节点设为Item子节点
        Item1.AppendChild(consumable);
        ConsumXml.AppendChild(Item1);
        //保存Xml文件到指定路径
        ConsumXml.Save(filePath + "/Consumable.xml");


    }
    //读取消耗品的XML数据
    public List<Consumable> LoadConsumByXml()
    {
        List<Consumable> consums = new List<Consumable>();
        //Consumable tempcosum = new Consumable();//暂存
        InnerText = new List<string>();
        //xml文件类
        XmlDocument xmlDoc = new XmlDocument();

        if (!File.Exists(filePath + "/Consumable.xml")) return null;
        //读取该路径的文件
        xmlDoc.Load(filePath + "/Consumable.xml");
        if (null == xmlDoc) return null;
        //获取节点    
        XmlNodeList nodes = xmlDoc.SelectSingleNode("Item").ChildNodes;
        foreach (XmlElement temp in nodes)
        {
            //获取子节点的attribute值
            //print(temp.GetAttribute("ID"));
            Consumable tempcosum = new Consumable(int.Parse(temp.GetAttribute("ID")),
                                                temp.ChildNodes[0].InnerText,
                                                temp.ChildNodes[1].InnerText,
                                                int.Parse(temp.ChildNodes[2].InnerText),
                                                int.Parse(temp.ChildNodes[3].InnerText),
                                                temp.ChildNodes[4].InnerText,
                                                int.Parse(temp.ChildNodes[5].InnerText),
                                                int.Parse(temp.ChildNodes[6].InnerText));//暂存

            //tempcosum.name = temp.ChildNodes[0].InnerText;
            consums.Add(tempcosum);

        }
        //consums = new Consumable(int.Parse(id), InnerText[0], InnerText[1], int.Parse(InnerText[2]), int.Parse(InnerText[3]), InnerText[4], int.Parse(InnerText[5]), int.Parse(InnerText[6]));
        return consums;
    }

    //存储武器的XML数据
    public void SaveWeaponByXml(string ID, string Name, string Descript, string buyprice, string sellprice, string icon, string Damage)
    {
        //消耗品的存储路径
        //filepath = Application.temporaryCachePath + "/Weapon.xml";



        //创建Weapon节点
        XmlElement Weapon = WeaponXml.CreateElement("Weapon");
        //创建name节点
        XmlElement name = WeaponXml.CreateElement("name");
        //创建Description节点
        XmlElement Description = WeaponXml.CreateElement("Description");
        //创建buyPrice节点
        XmlElement buyPrice = WeaponXml.CreateElement("buyPrice");
        //创建sellPrice节点
        XmlElement sellPrice = WeaponXml.CreateElement("sellPrice");
        //创建Icon节点
        XmlElement Icon = WeaponXml.CreateElement("Icon");
        //创建damage节点
        XmlElement damage = WeaponXml.CreateElement("damage");



        //设置Weapon节点的属性ID值
        Weapon.SetAttribute("ID", ID);
        //设置title节点的文本
        name.InnerText = Name;
        Description.InnerText = Descript;
        buyPrice.InnerText = buyprice;
        sellPrice.InnerText = sellprice;
        Icon.InnerText = icon;
        damage.InnerText = Damage;

        //将price节点设置为Weapon子节点
        Weapon.AppendChild(name);
        Weapon.AppendChild(Description);
        Weapon.AppendChild(buyPrice);
        Weapon.AppendChild(sellPrice);
        Weapon.AppendChild(Icon);
        Weapon.AppendChild(damage);



        //将Weapon节点设为Item子节点
        Item2.AppendChild(Weapon);
        WeaponXml.AppendChild(Item2);
        //保存Xml文件到指定路径
        WeaponXml.Save(filePath + "/Weapon.xml");
    }
    //读取武器的XML数据
    public Weapon LoadWeaponByXml(string id)
    {
        Weapon weapon;
        InnerText = new List<string>();
        //xml文件类
        XmlDocument xmlDoc = new XmlDocument();
        //读取该路径的文件
        xmlDoc.Load(filePath + "/Weapon.xml");
        //获取节点    
        XmlNodeList node = xmlDoc.SelectSingleNode("Item").ChildNodes;
        foreach (XmlElement temp in node)
        {
            //获取子节点的attribute值
            if (id == temp.GetAttribute("ID"))
            {
                for (int i = 0; i < temp.ChildNodes.Count; i++)
                {
                    InnerText.Add(temp.ChildNodes[i].InnerText);

                }

            }

        }
        weapon = new Weapon(int.Parse(id), InnerText[0], InnerText[1], int.Parse(InnerText[2]), int.Parse(InnerText[3]), InnerText[4], int.Parse(InnerText[5]));
        return weapon;
    }

    //存储防具的XML数据
    public void SaveArmorByXml(string ID, string Name, string Descript, string buyprice, string sellprice, string icon, string Power, string Defend, string mingjie)
    {

        //消耗品的存储路径
        //filepath = Application.temporaryCachePath + "/Armor.xml";



        //创建Armor节点
        XmlElement Armors = ArmorXml.CreateElement("Armor");
        //创建name节点
        XmlElement name = ArmorXml.CreateElement("name");
        //创建Description节点
        XmlElement Description = ArmorXml.CreateElement("Description");
        //创建buyPrice节点
        XmlElement buyPrice = ArmorXml.CreateElement("buyPrice");
        //创建sellPrice节点
        XmlElement sellPrice = ArmorXml.CreateElement("sellPrice");
        //创建Icon节点
        XmlElement Icon = ArmorXml.CreateElement("Icon");
        //创建Power节点
        XmlElement power = ArmorXml.CreateElement("Power");
        //创建Defend节点
        XmlElement defend = ArmorXml.CreateElement("Defend");
        //创建Icon节点
        XmlElement Mingjie = ArmorXml.CreateElement("mingjie");


        //设置Armor节点的属性ID值
        Armors.SetAttribute("ID", ID);
        //设置title节点的文本
        name.InnerText = Name;
        Description.InnerText = Descript;
        buyPrice.InnerText = buyprice;
        sellPrice.InnerText = sellprice;
        Icon.InnerText = icon;
        power.InnerText = Power;
        defend.InnerText = Defend;
        Mingjie.InnerText = mingjie;

        //将price节点设置为Armor子节点
        Armors.AppendChild(name);
        Armors.AppendChild(Description);
        Armors.AppendChild(buyPrice);
        Armors.AppendChild(sellPrice);
        Armors.AppendChild(Icon);
        Armors.AppendChild(power);
        Armors.AppendChild(defend);
        Armors.AppendChild(Mingjie);


        //将Armor节点设为Item子节点
        Item3.AppendChild(Armors);
        ArmorXml.AppendChild(Item3);
        //保存Xml文件到指定路径
        ArmorXml.Save(filePath + "/Armor.xml");

    }
    //读取防具的XML数据
    public Armor LoadArmorByXml(string id)
    {
        Armor armor;
        InnerText = new List<string>();
        //xml文件类
        XmlDocument xmlDoc = new XmlDocument();
        //读取该路径的文件
        xmlDoc.Load(filePath + "/Armor.xml");
        //获取节点    
        XmlNodeList node = xmlDoc.SelectSingleNode("Item").ChildNodes;
        foreach (XmlElement temp in node)
        {
            //获取子节点的attribute值
            if (id == temp.GetAttribute("ID"))
            {
                for (int i = 0; i < temp.ChildNodes.Count; i++)
                {
                    InnerText.Add(temp.ChildNodes[i].InnerText);

                }

            }

        }
        armor = new Armor(int.Parse(id), InnerText[0], InnerText[1], int.Parse(InnerText[2]), int.Parse(InnerText[3]), InnerText[4], int.Parse(InnerText[5]), int.Parse(InnerText[6]), int.Parse(InnerText[7]));
        return armor;
    }
}
