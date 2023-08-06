using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using UnityEditor.UIElements;
using System.Linq;

public class ItemEditor : EditorWindow
{
    private ItemDataList_SO dataBase;   //data���ļ�
    List<ItemDetails> itemList = new List<ItemDetails>();   //�������ļ��еĵ�����   
    private VisualTreeAsset itemRowTemplate;        //listģ�壬�Ǹ��ݵ����б�����ʱ����ʽ
    private ListView itemListView;      //������Ƶ�λ��
    private ScrollView itemDetailsSection;      //�Ҳ�������ʾ
    private ItemDetails activeItem;     //��ǰ�������Ʒ
    private VisualElement iconPreview; //iconԤ��
    private Sprite defautIcon;  //Ĭ��icon

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

        //���ҵ������ģ�壬�������ɱ���
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIBuilder/ItemRowTemplste.uxml");
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");

        defautIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");

        //�Ҳ���帳ֵ
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");

        //��ť
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDelectClicked;


        LoadDataBase();

        GenerateListView();
    }

    #region ��ť�¼�
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
    /// ���ص�����Ŀ�ļ�
    /// </summary>
    public void LoadDataBase() 
    {
        //����editor�µ�data�ļ�
        var dataArray = AssetDatabase.FindAssets("ItemDataList_SO");
        if (dataArray.Length > 1) 
        {
            //��ȡ�ļ���Ӧ��·��
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            dataBase = AssetDatabase.LoadAssetAtPath<ItemDataList_SO>(path);
        }

        itemList = dataBase.itemDetailsList;

        //�ദ�������޷�����
        EditorUtility.SetDirty(dataBase);
    }

    /// <summary>
    /// ���ɱ���
    /// </summary>
    private void GenerateListView() 
    {
        //ÿ�����ɱ���ʱ����һ�ݱ���ģ��
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();
        //ÿ�����ɺ�ִ�еĻص�
        Action<VisualElement, int> bindItem = (e, i) => 
        {
            if (i < itemList.Count) 
            {
                var data = itemList[i];
                //��sample�������ã����ö�Ӧ��ͼƬ
                if(data.itemIcon !=null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = data.itemIcon.texture;
                if (!string.IsNullOrEmpty(data.itemName))
                    e.Q<Label>("Name").text = data.itemName;
                else Debug.Log("Ϊ�գ�");
            }
        };

        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;
        itemListView.onSelectionChange += OnListSelectionChange;

        itemDetailsSection.visible = false;
    }

    /// <summary>
    /// �������ѡ���ȡ��Ӧ����Ʒ obj��ΪlistView��itemSource
    /// </summary>
    /// <param name="obj"></param>
    private void OnListSelectionChange(IEnumerable<object> obj)
    {
        activeItem = (ItemDetails)obj.First();
        itemDetailsSection.visible = true;
        GetItemDetails();
    }

    /// <summary>
    /// ���������Ʒ��ϸ
    /// </summary>
    private void GetItemDetails() 
    {
        itemDetailsSection.MarkDirtyRepaint();  //�ദ��

        //����itemID
        var integerField = itemDetailsSection.Q<IntegerField>("ItemId");

        integerField.value = activeItem.itemID;
        integerField.RegisterValueChangedCallback((evt) =>
        {
            activeItem.itemID = evt.newValue;
        });

        //item����
        var nameFiled = itemDetailsSection.Q<TextField>("ItemName");
        nameFiled.value = activeItem.itemName;
        nameFiled.RegisterValueChangedCallback((evt) =>
        {
            activeItem.itemName = evt.newValue;
            itemListView.Rebuild();     //���ֱ��޸�ʱ��������һ���б�
        });

        //����itemͼ��
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