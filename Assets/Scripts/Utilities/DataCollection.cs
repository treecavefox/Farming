using UnityEngine;

[System.Serializable]
public class ItemDetails 
{
    public int itemID;

    public string itemName;

    public ItemType itemType;   //物品种类

    public Sprite itemIcon;

    public Sprite itemOnWorldSprite;    //地图上显示的图片

    [TextArea]
    public string itemDescription;  //描述

    public int itemUseRadius;   //物体使用范围

    public bool canPickedup;    //可以被举起

    public bool canDropped;     //可以被扔到地上

    public bool canCarried;     //可以被拾取

    public int itemPrice;      //价格

    [Range(0,1)]
    public float sellPercentage;    //折扣百分比

    public ItemDetails(int id,string name,ItemType type) 
    {
        itemID = id;
        itemName = name;
        itemType = type;
    }
}

