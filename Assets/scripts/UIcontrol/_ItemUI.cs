using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class _ItemUI : MonoBehaviour
{


    public Text ItemName;
    public Image images;
    //public Image ItemImage;
    
    public void UpdateItemName(string name)
    {
        ItemName.text = name;
    }

    public void UpdateItemSprites(string loadpath)
    {

        Debug.Log("加载路径" + loadpath);
        Sprite sprites = Resources.Load(loadpath, typeof(Sprite)) as Sprite;
        Debug.Log(sprites);
        images.sprite = sprites;
    }
}
