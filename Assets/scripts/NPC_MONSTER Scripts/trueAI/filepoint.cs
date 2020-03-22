using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class filepoint : MonoBehaviour {
    public int sceneid;//自身所在的场景id号
    private string SceneName;
    public int havefile;//只取用0和1，0表示没有记录 1表示有记录
    public GameObject nowisdone;
    public GameObject nowisnodone;
	// Use this for initialization
	void Start () {
        havefile = 1;
        checkself();//检查自己状态


    }
	
	// Update is called once per frame
	void Update () {
        checkself();
        SceneName = SceneManager.GetActiveScene().name;//获取场景名称

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Player")
        {
            PlayerPrefs.SetInt("havefile", havefile);
            PlayerPrefs.SetFloat("restartx", transform.position.x);
            PlayerPrefs.SetFloat("restarty", transform.position.y+10);
            PlayerPrefs.SetFloat("restartz", transform.position.z);
            PlayerPrefs.SetInt("nowskill", other.gameObject.GetComponent<PlayerSkills>().skillIndex);
            PlayerPrefs.SetString("scenename", SceneName);
            //checkself();//检查自己状态
        }
 
    }
    private void checkself()
    {
        if (transform.position.x== PlayerPrefs.GetFloat("restartx")&& transform.position.z == PlayerPrefs.GetFloat("restartz"))
        {
            nowisdone.SetActive(true);
            nowisnodone.SetActive(false);
        }
        else
        {
            nowisdone.SetActive(false);
            nowisnodone.SetActive(true);
        }
    }
}
