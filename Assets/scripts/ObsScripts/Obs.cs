using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这个为物品的类，用于储存物品的类型，变量
[System.Serializable]
public class Obs
{
    public GameObject Object;
    public ObsType obstype;

}

public enum ObsType
{
    Wood,
    Stone,
    Cube
}