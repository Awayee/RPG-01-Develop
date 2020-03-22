using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    
    /*鼠标拖拽的监听*/
    public static Action<Transform> OnLeftBeginDrag;//这里加上Transform是为了能够得到格子的信息
    public static Action<Transform,Transform> OnLeftEndDrag;//传递两个参数一个是原来的格子，一个是现在的格子

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button==PointerEventData.InputButton.Left)//检查事件，如果左键按下
        {
            if (OnLeftBeginDrag!=null)
            {
                OnLeftBeginDrag(transform);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)//检查事件，如果左键按下
        {
            if (OnLeftEndDrag != null)
            {
                if (eventData.pointerEnter==null)//如果拖到背包外
                {
                    OnLeftEndDrag(transform,null);//这里的Transform的信息是刚开始拖拽时那个格子的信息
                }
                else
                {
                    OnLeftEndDrag(transform,eventData.pointerEnter.transform);
                }
                
            }
        }
    }







    /*鼠标进出grid的监听*/
    public static Action<Transform> onEnter;
    public static Action onExit;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter.tag == "Grid")
        {
            if (onEnter!=null)
            {
                
                onEnter(transform);//传参时将当前transform传过去，监听事件时将鼠标指着的格子的transfrom也传过去
            }
        }
       
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerEnter.tag == "Grid")
        {
            if (onExit != null)
            {
                onExit();
            }
        }
    }
}
