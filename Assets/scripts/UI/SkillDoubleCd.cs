using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//技能双重CD
public class SkillDoubleCd : MonoBehaviour
{

    public bool enable;//按钮能否交互
    public Image skillIcon;//按钮图标
    private string ButtonText;//初始按钮文字
    public Sprite buttonSprite1;//第一层CD满后未点击的按钮图片
    //public Sprite buttonSprite1Plus;//两层CD满后未点击图标时按钮图片
    public Sprite buttonSprite2;//第一次点击后的按图片
    private Image CdProgress;//图标背景

    //private Image fatherImage;//背景图片，父对象
    public Color Color1 =new Color(0.5f,0.5f,0.5f,1);//父物体按钮颜色
    public Color Color2 = new Color(0.1f,0.1f,0.1f,1);

    private Text TimeTip;//时间提示

    private float CurrentTime = 0f;//当前时间
    public float CdTime1 = 3f;//第一次段冷却时间
    public float CdTime2 = 3f;//第二次段冷却时间，在第一次释放之后计时

    public bool skilled = false;//是否已释放一段
    public bool skillState = false;//当前可释放的技能，false为第一段，true为第二段
    public bool fstPart = true;//第一段
    public bool scdPart = false;//第二段
    ButtonNormal btn;
    // Use this for initialization
    void Awake()
    {
        //SkillImg = transform.Find("Icon").GetComponent<Image>();
        //buttonSprite1 = skillIcon.sprite;

        TimeTip = GetComponentInChildren<Text>();
        CdProgress = transform.Find("CdProgress").GetComponent<Image>();
        btn = GetComponent<ButtonNormal>();
        //fatherImage = transform.parent.GetComponent<Image>();
        //开始时可以释放第一段
        //buttonImg.sprite = buttonSprite1Plus;
        fstPart = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (skilled) return;
        if (fstPart)
        {
            if (scdPart) return;
            else
            {
                //if (skillState) return;
                CurrentTime += Time.deltaTime;
                if (CurrentTime >= CdTime2)
                {
                    scdPart = true;
                    if (!skillState) Switchto1Plus();
                    EnableButton();
                    CurrentTime = 0;
                    TimeTip.text = ButtonText;//还原按钮文字
                    return;
                }
                CdProgress.fillAmount = CurrentTime / CdTime2;
                TimeTip.text = (CdTime2 - (int)CurrentTime).ToString();//显示技能剩余时间
            }
        }
        else
        {
            //if (skillState) return;
            CurrentTime += Time.deltaTime;
            CdProgress.fillAmount =CurrentTime / CdTime1;
            TimeTip.text = (CdTime1 - (int)CurrentTime).ToString();//显示技能剩余时间
            if (CurrentTime >= CdTime1)
            {
                TimeTip.text = ButtonText;//还原按钮文字
                fstPart = true;
                if (skillState)
                {
                    Switchto2();
                }
                else
                {
                    Switchto1Plus();
                    EnableButton();
                }

                CurrentTime = 0;
                return;
            }
        }
    }

    public void Enable2()//激活第二段
    {
        Switchto2();
        skillState = true;
        if (skilled) fstPart = false;//是否已释放过第一段
        //if(!skilled)fstPart = false;
        if (!scdPart)
        {
            DisableButton();
        }
        else return;
    }
    public void Baketo1()//回到第一段
    {
        skilled = false;
        skillState = false;
        if (fstPart)
        {
            Switchto1Plus();
            EnableButton();
        }
        else
        {
            if (scdPart)//如果未释放第二段，则转到第一段
            {
                fstPart = true;
                scdPart = false;
                Switchto1Plus();
                EnableButton();
            }
            else Switchto1();
        }
    }

    public void ResetCD()//释放技能
    {
        skilled = true;
        //print("Skilled:" + skilled);
        TimeTip.text = ButtonText;
    }
    public void Release2()//释放第二段
    {
        scdPart = false;
        DisableButton();
    }

    public void EnableButton()//激活按钮
    {
        enable = true;
        skillIcon.color = Color2;
        if(btn!=null) btn.EnableButton();
    }
    public void DisableButton()//禁用按钮
    {
        enable = false;
        CdProgress.fillAmount = 0;
        skillIcon.color = Color1;
        if(btn!=null) btn.DisableButton();
    }
    public void Switchto1()//切换到按钮图片1
    {
        skillIcon.sprite = buttonSprite1;
        skillIcon.color = Color1;
        //fatherImage.color = Color1;
        //fatherImage.sprite = buttonSprite1;
    }
    public void Switchto1Plus()//切换到按钮图片1plus
    {
        //fatherImage.sprite = buttonSprite1;
        //fatherImage.color = Color2;
        skillIcon.sprite = buttonSprite1;
        skillIcon.color = Color2;
    }
    public void Switchto2()//显示技能第二段的按钮图标
    {
        //fatherImage.sprite = buttonSprite2;
        //fatherImage.color = Color1;
        skillIcon.sprite = buttonSprite2;
    }
}
