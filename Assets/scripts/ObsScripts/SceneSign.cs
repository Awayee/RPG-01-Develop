using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//切换场景的标志
public class SceneSign : MonoBehaviour
{

    public int sceneIndex;//场景序号

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.GetComponent<PlayCharacter>().DisableMove();//停止移动
            GameManager.Instance.LoadSceneIndex(sceneIndex);
        }

    }

}
