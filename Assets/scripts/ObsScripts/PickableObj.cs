using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObj : MonoBehaviour
{


    [SerializeField] float minDistance = 4;//触发可交互的距离
    private Transform player;//玩家位置
    private Transform thisTransf;//该物体位置
    private bool entered = false;//判断是否已进入区域


    // [SerializeField] BagManager bagmanager;//背包对象
    [SerializeField] string ID, Name, detail, buyPrice, sellPrice, spritePath, getHp, getMp;//物品信息
    ButtonObjPick pickButton;//预制体
    private ButtonObjPick pButton;//按钮脚本
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thisTransf = this.transform;
        pickButton = Resources.Load<ButtonObjPick>("Prefabs/UIElement-Button-PickObj");
        //entered = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(thisTransf.position, player.position);//获得物品与玩家间的距离
        //进入感应范围
        if (distance <= minDistance)
        {
            if (entered) return;
            else
            {
                EnableButton();
                entered = true;
            }

        }
        //走出感应范围
        else
        {
            if (!entered) return;
            else
            {
                if (pButton != null) pButton.DestroyButton(2);
                entered = false;
            }
        }
    }

    //激活按钮
    void EnableButton()
    {
        //print("进入物品区域");
        //实例化为画布的子物体
        pButton = Instantiate(pickButton);
        pButton.GetObj(thisTransf);
        pButton.Fade_Appear(0.2f);

    }

    //捡起物品事件
    public void PickUpThis()
    {
        // bagmanager.SaveConsumByXml(ID, Name, detail, buyPrice, sellPrice, "Sprites/Items/" + spritePath, getHp, getHp);
        //保存物品信息
        BagManager.Instance.SaveConsumByXml(ID, Name, detail, buyPrice, sellPrice, "Sprites/Items/" + spritePath, getHp, getMp);
        //Consumable c1 = BagManager.Instance.LoadConsumByXml(ID);
        //BagManager.Instance.ItemList.Add(c1.ID, c1);
        //BagManager.Instance.StoreItems(c1.ID);//存储物品

        player.GetComponent<PlayCharacter>().TurnAtRightNow(thisTransf);//玩家转向
        GameManager.Instance.Tip("获得物品: " + Name);//弹出提示
        pButton.DestroyButton(0.5f);
        Destroy(this.gameObject, 0.6f);
    }
}