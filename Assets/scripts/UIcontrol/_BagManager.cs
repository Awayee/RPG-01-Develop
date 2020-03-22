using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Xml;


public class _BagManager : MonoBehaviour
{

    //获取XML文件中的文本
    public List<string> InnerText;
    
    //private string filePath = Application.temporaryCachePath;
    public string filepath;//用于存储XML文件的路径
    
    
    //Xml文件类 该类没有继承MonoBehaviour
    public static XmlDocument ConsumXml = new XmlDocument();
    XmlElement Item1 = ConsumXml.CreateElement("Item");

    public static XmlDocument WeaponXml = new XmlDocument();
    XmlElement Item2 = WeaponXml.CreateElement("Item");

    public static XmlDocument ArmorXml = new XmlDocument();
    XmlElement Item3 = ArmorXml.CreateElement("Item");




    private bool isShow = false;
    private bool isDrag = false;

    public PanalUI Panalui;//获取脚本PanalUI中的函数，用于判断是否还有空位
    public TooltipUI TooltipUI;//获得提示框的脚本
    public DragItemUI DragItemUi;//获得拖拽的脚本



    //单例模式
    private static _BagManager _instance;
    public static _BagManager Instance
    {
        get { 
            // if(null==_instance)//如果未实例化
            // {
            //     GameObject _packagePage = Resources.Load<GameObject>("Prefabs/GameUI-Page-Package");
            //     _instance = Instantiate(_packagePage).GetComponent<_BagManager>();//实例化
            //     _instance.transform.SetParent(GameManager.Instance.UICanvas);//设为画布的子物体
            //     _instance.gameObject.SetActive(false);//隐藏

            // }
            return _instance; }
    }
    public Dictionary<int,Item> ItemList=new Dictionary<int, Item>();//这里的int相当于int类型得key，用于索引

    //启动时执行
    void Awake()
    {
       
        //单例
        // _instance = this;

        //事件监听
        ItemList = new Dictionary<int, Item>();

        
        //存储消耗品的XML文件
        //参数分别为：ID,名字，描述，购买价格，回收价格，图标路径，恢复血量，恢复魔量
        //SaveConsumByXml("1", "血瓶", "用于恢复角色40点血量", "20", "10", "Sprites/Items/hp", "40", "0");
        //SaveConsumByXml("2", "魔瓶", "用于恢复角色30点魔量", "30", "15", "Sprites/Items/mp", "0", "30");
        //加载消耗品的XML文件
        //Consumable c1=LoadConsumByXml("1");
        //Consumable c2 = LoadConsumByXml("2");
        //ItemList.Add(c1.ID, c1);
        //ItemList.Add(c2.ID, c2);

        //存储武器的XML文件
        //参数分别为：ID,名字，描述，购买价格，回收价格，图标路径，伤害值
        //SaveWeaponByXml("3", "斧头", "商店里能够买到的普通的斧头", "100", "30", "Sprites/Items/axe", "100");
        //SaveWeaponByXml("4", "短剑", "商店里能够买到的普通的短剑", "90", "20", "Sprites/Items/sword", "90");
        //Weapon w1 = LoadWeaponByXml("3");
        //Weapon w2 = LoadWeaponByXml("4");
        //ItemList.Add(w1.ID, w1);
        //ItemList.Add(w2.ID, w2);

        //存储防具的XML文件
        //参数分别为：ID,名字，描述，购买价格，回收价格，图标路径，力量，防御，敏捷
        //SaveArmorByXml("5", "破旧的头盔", "铁质的头盔，保护头部不受伤害", "80", "40", "Sprites/Items/helmets", "5", "25","10");
        //SaveArmorByXml("6", "皮质的铠甲", "皮质的铠甲，勉强能够保护身体", "120", "60", "Sprites/Items/armor", "10", "35", "8");
        ////加载防具的XML
        //Armor a1=LoadArmorByXml("5");
        //Armor a2 = LoadArmorByXml("6");
        //ItemList.Add(a1.ID, a1);
        //ItemList.Add(a2.ID, a2);

        GridUI.onEnter += GridUI_OnEnter;
        GridUI.onExit += GridUI_OnExit;
        GridUI.OnLeftBeginDrag += GridUI_OnLeftBeginDrag;
        GridUI.OnLeftEndDrag += GridUI_OnLeftEndDrag;
    }

    // //用于鼠标定位
    // void Update()
    // {
    //     /*先进性鼠标位置的转换*/
    //     Vector2 position;
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(gameObject.transform as RectTransform,
    //         new Vector2(Input.mousePosition.x, Input.mousePosition.y ), null, out position);//输入坐标为鼠标坐标，输出一个position


    //     /*使用这个position进行定位*/
    //     if (isDrag)
    //     {
    //         DragItemUi.Show();
    //         DragItemUi.SetLocalPosition(position);//position是鼠标的位置
    //     }
    //     else if (isShow)
    //     {
            
    //         TooltipUI.Show();
    //         TooltipUI.SetLocalPosition(position);
    //     }
    // }

    //生成物体
    public void StoreItems(int ItemID)
    {
        if (!ItemList.ContainsKey(ItemID))//判断物品列表中是不是有这个ID的物体
        {
            return;
        }

        
        Transform EmptyGrid = Panalui.GetEmpty();//使用PanalUI脚本中的GetEmpty()方法获得空的Grid

        if (EmptyGrid == null)
        {
            Debug.Log("背包已经满了");
            return;
        }
        Item temp = ItemList[ItemID];//如果有,将这个Item取出
        this.CreateNewItem(temp, EmptyGrid);

      
    }


    

    #region 事件回调
    //监听事件监听到鼠标时，显示文本
    public void GridUI_OnEnter(Transform GridTransform)
    {
        
        Item item = ItemModel.GetItem(GridTransform.name);//获得Item信息
        if (item==null)
        {
            return;
        }
        string text = GetTooltipText(item);
        TooltipUI.UpdateTooltip(text);
        isShow = true;
        
    }
    public void GridUI_OnExit()
    {
        isShow = false;
        TooltipUI.UpdateTooltip("");
        TooltipUI.Hide();
    }
    //监听事件，监听拖拽事件
    public void GridUI_OnLeftBeginDrag(Transform GridTransform)
    {

        if (GridTransform.childCount==0)
        {
            return;
        }
        else
        {
            Item item = ItemModel.GetItem(GridTransform.name);//获得Item信息
            //DragItemUi.UpdateItemName(item.name);
            DragItemUi.UpdateItemSprites(item.Icon);
            Debug.Log("item.Icon"+ item.Icon);
            Destroy(GridTransform.GetChild(0).gameObject);//将背包原来的物体删除
            

            isDrag = true;
        }
    }

   
    public void GridUI_OnLeftEndDrag(Transform GridTransform, Transform GridEndTransform)
    {
        isDrag = false;
        DragItemUi.Hide();

        /*判断拖拽结束时指着的格子上有没有物体，如果有就进行交换，没有就把物品直接放上去*/


        if (GridEndTransform == null)//扔东西
        {
            ItemModel.DeleteItem(GridTransform.name);//删除之前格子的信息
            Debug.LogWarning("物品已扔");
        }


        else if (GridEndTransform.tag == "Grid")//如果拖到另一个格子或者当前格子里里
        {
            if (GridEndTransform.childCount == 0)//直接扔进去
            {
                Item item = ItemModel.GetItem(GridTransform.name);//获得之前的数据
                this.CreateNewItem(item, GridEndTransform);//创建出新的Item
                ItemModel.DeleteItem(GridTransform.name);//删除之前格子的信息
            }
            else//交换
            {
                Item item1 = ItemModel.GetItem(GridTransform.name);//获得之前的数据
                Item item2 = ItemModel.GetItem(GridEndTransform.name);//获得之后的数据
                Destroy(GridEndTransform.GetChild(0).gameObject);//将该位置的物体删除
                ItemModel.DeleteItem(GridTransform.name);//删除之后格子的信息
                ItemModel.DeleteItem(GridTransform.name);//删除之前格子的信息
                this.CreateNewItem(item1, GridEndTransform);//创建出新的Item
                this.CreateNewItem(item2, GridTransform);//创建出新的Item

            }

        }
        else//如果拖到UI其他地方
        {
            Item item = ItemModel.GetItem(GridTransform.name);//获得之前的数据
            this.CreateNewItem(item, GridTransform);//创建出新的Item
        }
    }
#endregion


    //提示框文字显示
    private string GetTooltipText(Item item)
    {
        if (item==null)
        {
            return "";
        }
        StringBuilder sb=new StringBuilder();//用于存放字符串数组
        sb.AppendFormat("<color=red>{0}</color>\n\n",item.name);
        switch (item.IDtype)
        {
            case "Armor":
                Armor armor=item as Armor;
                sb.AppendFormat("力量：{0}\n防御：{1}\n敏捷：{2}\n\n",armor.Power,armor.Defend,armor.mingjie);
                break;
            case "Weapon":
                Weapon weapon=item as Weapon;
                
                sb.AppendFormat("攻击：{0}\n\n", weapon.damage);
                break;
            default:
                break;
        }

        sb.AppendFormat("<size=25><color=white>购买价格：{0}\n出售价格：{1}</color></size>\n\n<color=yellow><size=20>描述：{2}</size></color>",item.buyPrice,item.sellPrice,item.Description);
        return sb.ToString();
    }


    //创建新Item
    public void CreateNewItem(Item item,Transform parent)
    {
        GameObject ItemObject = Resources.Load<GameObject>("Prefabs/Item");//使用Resources的方法获得预制体
        if (item!=null)
        {
            ItemObject.GetComponent<_ItemUI>().UpdateItemName(item.name);//通过GetComponent<ItemUI>()能够获得挂载在该物体上的脚本，可调用其中的函数
            ItemObject.GetComponent<_ItemUI>().UpdateItemSprites(item.Icon);
            GameObject ItemUse = GameObject.Instantiate(ItemObject);//创建出预制体

        ItemUse.transform.SetParent(parent);//将Grid设为生成的物体的父物体

        //大小跟父物体一样大小，位置重叠就行
        ItemUse.transform.localPosition = Vector3.zero;
        ItemUse.transform.localScale = Vector3.one;
        //保存
        ItemModel.StoreItem(parent.name,item);//保存的是网格的名字，还有Item的信息
        }
        
    }

    //重新排列
    public void Rearrange()
    {
        List<Transform> nums = Panalui.FindItem();
       var items=new List<Item>();//用于存储即将重新排序的Items
        for (int i = 0; i < nums.Count; i++)
        {
            items.Add(ItemModel.GetItem(nums[i].name));//获得Item信息
            ItemModel.DeleteItem(nums[i].name);//清空表
        }



        for (int i = 0; i < items.Count; i++)
        {
            Transform EmptyGrid = Panalui.GetGridname(i);//使用PanalUI脚本中的GetEmpty()方法获得空的Grid

            if (EmptyGrid == null)
            {
                Debug.Log("背包已经满了");
                return;
            }
            Item temp = items[i];
            this.CreateNewItem(temp, EmptyGrid);
        }
    }






    
    //存储消耗品的XML数据
    public void SaveConsumByXml(string ID,string Name, string Descript, string buyprice, string sellprice,string icon,string backhp,string backmp)
    {
        //消耗品的存储路径
        filepath = Application.temporaryCachePath + "/Consumable.xml";



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
        ConsumXml.Save(filepath);

    }
    //读取消耗品的XML数据
    public Consumable LoadConsumByXml(string id)
    {
        Consumable consum;
        InnerText=new List<string>();
         //xml文件类
         XmlDocument xmlDoc = new XmlDocument();
        //读取该路径的文件
        xmlDoc.Load(filepath);
        //获取节点    
        XmlNodeList node = xmlDoc.SelectSingleNode("Item").ChildNodes;
        foreach (XmlElement temp in node)
        {
            //获取子节点的attribute值
            if ( id == temp.GetAttribute("ID"))
            {
                for (int i = 0; i < temp.ChildNodes.Count; i++)
                {
                    InnerText.Add(temp.ChildNodes[i].InnerText);
                    
                }

            }
            
        }
        consum = new Consumable(int.Parse(id), InnerText[0], InnerText[1], int.Parse(InnerText[2]), int.Parse(InnerText[3]), InnerText[4], int.Parse(InnerText[5]), int.Parse(InnerText[6]));
        return consum;
    }

    //存储武器的XML数据
    public void SaveWeaponByXml(string ID, string Name, string Descript, string buyprice, string sellprice, string icon,string Damage)
    {
        //消耗品的存储路径
        filepath = Application.temporaryCachePath + "/Weapon.xml";



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
        WeaponXml.Save(filepath);
    }
    //读取武器的XML数据
    public Weapon LoadWeaponByXml(string id)
    {
        Weapon weapon;
        InnerText = new List<string>();
        //xml文件类
        XmlDocument xmlDoc = new XmlDocument();
        //读取该路径的文件
        xmlDoc.Load(filepath);
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
        weapon=new Weapon(int.Parse(id), InnerText[0], InnerText[1], int.Parse(InnerText[2]), int.Parse(InnerText[3]), InnerText[4], int.Parse(InnerText[5]));
        return weapon;
    }

    //存储防具的XML数据
    public void SaveArmorByXml(string ID, string Name, string Descript, string buyprice, string sellprice, string icon, string Power, string Defend, string mingjie)
    {

        //消耗品的存储路径
        filepath = Application.temporaryCachePath + "/Armor.xml";



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
        ArmorXml.Save(filepath);

    }
    //读取防具的XML数据
    public Armor LoadArmorByXml(string id)
    {
        Armor armor;
        InnerText = new List<string>();
        //xml文件类
        XmlDocument xmlDoc = new XmlDocument();
        //读取该路径的文件
        xmlDoc.Load(filepath);
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
