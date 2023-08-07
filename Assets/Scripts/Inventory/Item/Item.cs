using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory 
{
    public class Item : MonoBehaviour
    {
        public int itemID;

        //��ƷͼƬ
        private SpriteRenderer spriteRenderer;
        //��ײ��ߴ�
        private BoxCollider2D coll;
        //��Ʒ��Ϣ
        private ItemDetails itemDetails;

        //��ʼ��ȡ��ƷͼƬ
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

                //�޸���ײ��ߴ�(ͼƬ����ʱ����ê��Ϊ���ģ���˻���Ҫ�޸�ê��)
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }
        }
    }
}

