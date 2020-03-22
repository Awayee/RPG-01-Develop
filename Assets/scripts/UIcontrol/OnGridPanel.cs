using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//背包面板
public class OnGridPanel : MonoBehaviour
{

    //子物体
    [SerializeField] ItemGrid item;
    //物品信息
    List<Consumable> cnsmbs;

    public Transform info;//右边的信息框
    private Image info_Image;
    private Text info_Detail;
    private Text info_Name;
    private ButtonNormal info_Btn;
    [SerializeField] private ButtonNormal delete_Btn;
    [SerializeField] private ButtonNormal sort_Btn;

    PlayCharacter player;
    void Awake()
    {
        //获取变量
        //items = GetComponentsInChildren<ItemGrid>();
        //获取信息框
        info_Name = info.GetChild(1).GetComponentInChildren<Text>();
        info_Detail = info.GetChild(2).GetComponentInChildren<Text>();
        info_Image = info.GetChild(0).GetComponent<Image>();
        info_Image.color = new Color(1, 1, 1, 0);
        info_Btn = info.GetComponentInChildren<ButtonNormal>();
        info_Btn.gameObject.SetActive(false);//隐藏按钮
        //print("Length of Items: "+items.Length);

        player = GameObject.FindWithTag("Player").GetComponent<PlayCharacter>();

        cnsmbs = BagManager.Instance.LoadConsumByXml();
        if (null == cnsmbs || cnsmbs.Count < 1)//如果没有
        {
            info_Detail.text = "没有任何物品";
            info_Image.color = new Color(1, 1, 1, 0);

            delete_Btn.gameObject.SetActive(false);
            sort_Btn.gameObject.SetActive(false);

            return;
        }
        else GetItems();
        //初始未选
        //InselectAll();
    }
    void GetItems()//获取已有的物品
    {

        //items = new ItemGrid[cnsmbs.Count];
        for (int i = 0; i < cnsmbs.Count; i++)
        {
            ItemGrid _item = Instantiate(item);
            _item.transform.parent = this.transform;
            _item.gameObject.SetActive(true);
            _item.interactable = true;
            _item.SetConsum(cnsmbs[i]);
        }
        delete_Btn.gameObject.SetActive(true);
        sort_Btn.gameObject.SetActive(true);
    }
    public void InselectAll()//全未选中
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<ItemGrid>().InselectThis();
        }
    }

    public void DisplayInfo(Consumable cs, ItemGrid it)//在右边的框中显示详细信息
    {
        if (info_Image.color.a < 1) info_Image.color = new Color(1, 1, 1, 1);//恢复图片颜色
        info_Image.color = new Color(info_Image.color.r, info_Image.color.g, info_Image.color.b, 1);
        info_Name.color = new Color(info_Name.color.r, info_Name.color.g, info_Name.color.b, 1);
        info_Detail.color = new Color(info_Detail.color.r, info_Detail.color.g, info_Detail.color.b, 1);

        info_Image.sprite = Resources.Load<Sprite>(cs.Icon);
        info_Detail.text = cs.Description;
        info_Name.text = cs.name;
        info_Btn.gameObject.SetActive(true);//显示按钮
        info_Btn.EnableButton();

        info_Btn.onClick.RemoveAllListeners();
        info_Btn.onClick.AddListener(delegate { UseConsumable(cs, it); });//添加 点击事件

        delete_Btn.onClick.RemoveAllListeners();
        delete_Btn.onClick.AddListener(delegate { DeleteConsumable(cs, it); });

    }
    private void DeleteConsumable(Consumable c, ItemGrid ig)
    {
        cnsmbs.Remove(c);
        Destroy(ig.gameObject);
        ig.ClearThis();//清除当前项
        info_Btn.DisableButton();//禁用按钮
        //提示框颜色浅化
        info_Name.color -= new Color(0, 0, 0, 0.5f);
        info_Image.color -= new Color(0, 0, 0, 0.5f);
        info_Detail.color -= new Color(0, 0, 0, 0.5f);
    }
    private void UseConsumable(Consumable c, ItemGrid ig)//使用该物品
    {
        // print("物品已使用");
        // print(c.backHp);
        // print(c.backMp);
        //恢复生命或能量
        if (c.backHp > 0)
        {
            player.ChangeLifeValue(c.backHp);
            GameManager.Instance.Tip("获得" + c.backHp.ToString() + "点生命");
        }

        if (c.backMp > 0)
        {
            player.ChangeEnergyValue(c.backMp);
            GameManager.Instance.Tip("获得" + c.backMp.ToString() + "点能量");
        }
        DeleteConsumable(c,ig);
    }
    public void SortConsums()
    {
        List<Consumable> newCnsmbs = new List<Consumable>();
        int cnsmbsCount = cnsmbs.Count;
        while (newCnsmbs.Count < cnsmbsCount)
        {
            newCnsmbs.Add(cnsmbs[0]);
            cnsmbs.Remove(cnsmbs[0]);
            for (int i = 0; i < cnsmbs.Count; i++)
            {
                if (cnsmbs[i].name == newCnsmbs[0].name)
                {
                    newCnsmbs.Add(cnsmbs[i]);
                    cnsmbs.Remove(cnsmbs[i]);
                    i--;

                }
            }
            //print("newCnsmbs.Count: " + newCnsmbs.Count);
        }
        cnsmbs = newCnsmbs;
        //销毁所有 子物体
        ItemGrid[] tempItems = GetComponentsInChildren<ItemGrid>();
        for (int i = 0; i < tempItems.Length;i++)
        {
            Destroy(tempItems[i].gameObject);
        }
            GetItems();

    }
    void OnDestroy()//页面销毁时重新写入
    {
        if (cnsmbs != null)
            BagManager.Instance.SaveConsumByXml(cnsmbs, true);
    }
}
