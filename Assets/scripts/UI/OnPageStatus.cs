using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPageStatus : MonoBehaviour
{
    PlayCharacter player;
    [SerializeField] private Text txt_Energy, txt_Life,AttackPower,SkillPower,physicDeff,skillDeff;
    [SerializeField]private ValueBar bar_Energy, bar_Life;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(true);
            //Debug.Log(child.name);
        }

        //获取组件
         if (null == player) player = GameObject.FindObjectOfType<PlayCharacter>();
        // if (null == txt_Energy) txt_Energy = transform.Find("Info/Text_Energy").GetComponent<Text>();
        // if (null == txt_Life) txt_Life = transform.Find("Info/Text_Life").GetComponent<Text>();
        // if (null == bar_Energy) bar_Energy = transform.Find("Info/Bar_Energy").GetComponent<ValueBar>();
        // if (null == bar_Life) bar_Life = transform.Find("Info/Bar_Life").GetComponent<ValueBar>();
        // if (null == txt_AttackPower) txt_AttackPower = transform.Find("Info/Text_AttackPower").GetComponent<Text>();
        

        // bar_Life.gameObject.SetActive(true);
        // bar_Energy.gameObject.SetActive(true);


        //初始化
        bar_Life.SetValue(0);
        bar_Life.Hide(0.1f, true);
        bar_Energy.SetValue(0);
        bar_Energy.Hide(0.1f, true);

        //赋值
        bar_Life.SetValue(player.life / player.maxLife);
        bar_Life.Display(1, true);
        txt_Life.text = ((int)player.life).ToString() + "/" + ((int)(player.maxLife)).ToString();

        bar_Energy.SetValue(player.energy / player.maxEnergy);
        bar_Energy.Display(1, true);
        txt_Energy.text = ((int)player.energy).ToString() + "/" + ((int)(player.maxEnergy)).ToString();

        AttackPower.text = player.attackPower.ToString();


    }
    // void OnDisable()
    // {
    //     if (null == player) player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayCharacter>();
    //     if (null == txt_Energy) txt_Energy = transform.Find("Text_Energy").GetComponent<Text>();
    //     if (null == txt_Life) txt_Life = transform.Find("Text_Life").GetComponent<Text>();
    //     if (null == bar_Energy) bar_Energy = transform.Find("Bar_Energy").GetComponent<ValueBar>();
    //     if (null == bar_Life) bar_Life = transform.Find("Bar_Life").GetComponent<ValueBar>();

    //     // bar_Life.SetValue(0);
    //     // bar_Life.Hide(0.1f, true);
    //     // bar_Energy.SetValue(0);
    //     // bar_Energy.Hide(0.1f, true);

    // }
}
