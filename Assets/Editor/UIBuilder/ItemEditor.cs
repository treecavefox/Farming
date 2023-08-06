using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using UnityEditor.UIElements;
using System.Linq;

public class ItemEditor : EditorWindow
{
    private ItemDataList_SO dataBase;   //data类文件
    List<ItemDetails> itemList = new List<ItemDetails>();   //数据类文件中的道具类   
    private VisualTreeAsset itemRowTemplate;        //list模板，是根据道具列表生成时的样式
    private ListView itemListView;      //表项绘制的位置
    private ScrollView itemDetailsSection;      //右侧内容显示
    private ItemDetails activeItem;     //当前激活的物品
    private VisualElement iconPreview; //icon预览
    private Sprite defautIcon;  //默认icon

    [MenuItem("Fox/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        //VisualElement label = new Label("Hello World! From C#");
        //root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIBuilder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        //查找到表项的模板，用于生成表项
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIBuilder/ItemRowTemplste.uxml");
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");

        defautIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");

        //右侧面板赋值
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");

        //按钮
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDelectClicked;


        LoadDataBase();

        GenerateListView();
    }

    #region 按钮事件
    private void OnDelectClicked()
    {
        itemList.Remove(activeItem);
        itemListView.Rebuild();
        itemDetailsSection.visible = false;
    }

    private void OnAddItemClicked()
    {
        var newItem = new ItemDetails(1001 + itemList.Count, "New Item", ItemType.BreakTool);
        itemList.Add(newItem);
        itemListView.Rebuild();
    }
    #endregion

    /// <summary>
    /// 加载道具项目文件
    /// </summary>
    public void LoadDataBase() 
    {
        //查找editor下的data文件
        var dataArray = AssetDatabase.FindAssets("ItemDataList_SO");
        if (dataArray.Length > 1) 
        {
            //获取文件对应的路径
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            dataBase = AssetDatabase.LoadAssetAtPath<ItemDataList_SO>(path);
        }

        itemList = dataBase.itemDetailsList;

        //脏处理，否则无法保存
        EditorUtility.SetDirty(dataBase);
    }

    /// <summary>
    /// 生成表项
    /// </summary>
    private void GenerateListView() 
    {
        //每次生成表项时复制一份表项模板
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();
        //每次生成后执行的回调
        Action<VisualElement, int> bindItem = (e, i) => 
        {
            if (i < itemList.Count) 
            {
                var data = itemList[i];
                //见sample，查找用，设置对应的图片
                if(data.itemIcon !=null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = data.itemIcon.texture;
                if (!string.IsNullOrEmpty(data.itemName))
                    e.Q<Label>("Name").text = data.itemName;
                else Debug.Log("为空！");
            }
        };

        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;
        itemListView.onSelectionChange += OnListSelectionChange;

        itemDetailsSection.visible = false;
    }

    /// <summary>
    /// 根据玩家选择获取对应的物品 obj中为listView的itemSource
    /// </summary>
    /// <param name="obj"></param>
    private void OnListSelectionChange(IEnumerable<object> obj)
    {
        activeItem = (ItemDetails)obj.First();
        itemDetailsSection.visible = true;
        GetItemDetails();
    }

    /// <summary>
    /// 设置左侧物品详细
    /// </summary>
    private void GetItemDetails() 
    {
        itemDetailsSection.MarkDirtyRepaint();  //脏处理

        //设置itemID
        var integerField = itemDetailsSection.Q<IntegerField>("ItemId");

        integerField.value = activeItem.itemID;
        integerField.RegisterValueChangedCallback((evt) =>
        {
            activeItem.itemID = evt.newValue;
        });

        //item名字
        var nameFiled = itemDetailsSection.Q<TextField>("ItemName");
        nameFiled.value = activeItem.itemName;
        nameFiled.RegisterValueChangedCallback((evt) =>
        {
            activeItem.itemName = evt.newValue;
            itemListView.Rebuild();     //名字被修改时重新生成一次列表
        });

        //设置item图标
        iconPreview.style.backgroundImage = activeItem.itemIcon == null ? defautIcon.texture : activeItem.itemIcon.texture;
        var icon = itemDetailsSection.Q<ObjectField>("ItemIcon");
        icon.value = activeItem.itemIcon;
        icon.RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            activeItem.itemIcon = newIcon;
            iconPreview.style.backgroundImage = newIcon == null? defautIcon.texture : newIcon.texture;
            itemListView.Rebuild();
        });

        itemDetailsSection.Q<ObjectField>("ItemSprite").value = activeItem.itemOnWorldSprite;
        itemDetailsSection.Q<ObjectField>("ItemSprite").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemOnWorldSprite = (Sprite)evt.newValue;
        });

        itemDetailsSection.Q<EnumField>("ItemType").Init(activeItem.itemType);
        itemDetailsSection.Q<EnumField>("ItemType").value = activeItem.itemType;
        itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemType = (ItemType)evt.newValue;
        });

        itemDetailsSection.Q<TextField>("Description").value = activeItem.itemDescription;
        itemDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemDescription = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("ItemUseRadius").value = activeItem.itemUseRadius;
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemUseRadius = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanPickedup").value = activeItem.canPickedup;
        itemDetailsSection.Q<Toggle>("CanPickedup").RegisterValueChangedCallback(evt =>
        {
            activeItem.canPickedup = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanDropped").value = activeItem.canDropped;
        itemDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeItem.canDropped = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanCarried").value = activeItem.canCarried;
        itemDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeItem.canCarried = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("ItemPrice").value = activeItem.itemPrice;
        itemDetailsSection.Q<IntegerField>("ItemPrice").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemPrice = evt.newValue;
        });

        itemDetailsSection.Q<Slider>("SellPercentage").value = activeItem.sellPercentage;
        itemDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.sellPercentage = evt.newValue;
        });
    }
}