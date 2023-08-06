using UnityEngine;

[System.Serializable]
public class ItemDetails 
{
    public int itemID;

    public string itemName;

    public ItemType itemType;   //��Ʒ����

    public Sprite itemIcon;

    public Sprite itemOnWorldSprite;    //��ͼ����ʾ��ͼƬ

    [TextArea]
    public string itemDescription;  //����

    public int itemUseRadius;   //����ʹ�÷�Χ

    public bool canPickedup;    //���Ա�����

    public bool canDropped;     //���Ա��ӵ�����

    public bool canCarried;     //���Ա�ʰȡ

    public int itemPrice;      //�۸�

    [Range(0,1)]
    public float sellPercentage;    //�ۿ۰ٷֱ�

    public ItemDetails(int id,string name,ItemType type) 
    {
        itemID = id;
        itemName = name;
        itemType = type;
    }
}

