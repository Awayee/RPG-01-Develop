using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//背包里每个物品栏挂的脚本，带有点击事件
public class ItemGrid : MonoBehaviour, IPointerClickHandler{
	
	public bool interactable = false;//可点击
    public bool selected = false;//已选中
    
    public Color selectedColor;//选中时的颜色
    private Color inselectedColor;//未选中时的颜色
    public Consumable csmb;//该项对应的物品
    Image thisImg;//该物体
    Image childImg;//子物体
    Text childTxt;//文字
    // Use this for initialization
    void Awake () {
        thisImg = GetComponent<Image>();
        inselectedColor = thisImg.color;
    }
	
	//点击事件
	public void OnPointerClick(PointerEventData eventData)
	{
		if(!interactable)return;

        if(selected)return;//如果已选中则跳过
        SelectThis();
    }
	public void SetConsum(Consumable cs)//获取该项
	{
        csmb = cs;
        //显示名字
        if(null == childTxt)childTxt = GetComponentInChildren<Text>();
        childTxt.text = cs.name;
		//读取，显示图片
        if(null == childImg)childImg = transform.GetChild(1).GetComponent<Image>();
        childImg.sprite = Resources.Load<Sprite>(cs.Icon);
        childImg.color = new Color(1, 1, 1, 1);
    }
	public void SelectThis()//选中该项 
	{
        transform.parent.GetComponent<OnGridPanel>().InselectAll();//全不选
		//选择该项
		thisImg.color = selectedColor;
        selected = true;
        //显示信息 
		transform.parent.GetComponent<OnGridPanel>().DisplayInfo(csmb,this);

    }
    public void InselectThis()//未选中该项
	{
        if (null == thisImg)
        {
            thisImg = GetComponent<Image>();
            inselectedColor = thisImg.color;
        }
        thisImg.color = inselectedColor;
        selected = false;
    }
	public void ClearThis()//清空该项
	{
        InselectThis();
        if(csmb!=null)csmb = null;
        //名字
        childTxt.text = null;
		//图片
        childImg.sprite = null;
        interactable = false;
        childImg.color = new Color(0,0,0,0);
    }


}
