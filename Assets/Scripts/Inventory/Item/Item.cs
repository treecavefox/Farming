using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory 
{
    public class Item : MonoBehaviour
    {
        public int itemID;

        //物品图片
        private SpriteRenderer spriteRenderer;
        //碰撞体尺寸
        private BoxCollider2D coll;
        //物品信息
        private ItemDetails itemDetails;

        //开始获取物品图片
        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (itemID != 0) 
            {
                Init(itemID);
            }
        }

        public void Init(int ID) 
        {
            itemID = ID;
            itemDetails = InventoryManager.Instance.GetItemDetails(itemID);
            if (itemDetails != null) 
            {
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite != null ? itemDetails.itemOnWorldSprite : itemDetails.itemIcon;

                //修改碰撞体尺寸(图片生成时会以锚点为重心，因此还需要修改锚点)
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }
        }
    }
}

