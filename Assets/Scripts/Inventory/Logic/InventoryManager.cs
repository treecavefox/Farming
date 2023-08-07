using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory 
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        public ItemDataList_SO itemDataList_SO;

        /// <summary>
        /// 根据ID获取物品详细
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID) 
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }
    }
}

