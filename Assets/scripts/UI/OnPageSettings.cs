using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPageSettings : MonoBehaviour
{

    PlayCharacter player;
    GameControl gamec;
    [SerializeField] ButtonSlider Volume_Audio, Volume_BGM;//开关
    [SerializeField] Toggle autoAttack, paintOnMainMenu, paintOnLoading;//滑动条
    [SerializeField] ButtonNormal replay, backHome, exit;//按钮
    //GameObject miniMap;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayCharacter>();
        gamec = GameObject.FindObjectOfType<GameControl>();
        //miniMap = GameManager.Instance.UICanvas.Find("miniMap").gameObject;
        Initialize();
    }
	void Initialize()
	{
        //获取数据
        autoAttack.isOn = player.autoAttack;
        paintOnMainMenu.isOn = GameManager.Instance.paintOnMainMenu;
        paintOnLoading.isOn = GameManager.Instance.paintOnLoading;
        //miniMapOn.isOn = miniMap.activeInHierarchy;

        Volume_Audio.SetValue(GameManager.Instance.volume_Audio);
        Volume_BGM.SetValue(GameManager.Instance.volume_BGM);
		
		//添加监听事件
		autoAttack.onValueChanged.AddListener(ChangePlayerAutoAttack);
        paintOnMainMenu.onValueChanged.AddListener(GameManager.Instance.setPaintOnMainMenu);
        paintOnLoading.onValueChanged.AddListener(GameManager.Instance.setPaintOnLoading);

        Volume_Audio.onValueChanged.AddListener(GameManager.Instance.SetAudioVolume);
        Volume_BGM.onValueChanged.AddListener(GameManager.Instance.SetBGMVolume);

        replay.onClick.AddListener(gamec.ReplayGame);
        backHome.onClick.AddListener(gamec.BackToMenu);
        exit.onClick.AddListener(gamec.QuitGame);

        //miniMapOn.onValueChanged.AddListener(miniMap.SetActive);
    }
	public void ChangePlayerAutoAttack(bool autoAtk)//设置玩家是否自动攻击
	{
        player.autoAttack = autoAtk;
    }

}
