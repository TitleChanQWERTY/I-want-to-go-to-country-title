using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class ItemController : MonoBehaviour
{
    [SerializeField] private Item _item;

    [Header("Settings")]
    //[SerializeField] private ItemTypeEnum _itemType;
    //[SerializeField] private InstrumentTypeEnum _instrumentType;
    //[SerializeField] private SeedTypeEnum _seedType;
    //[SerializeField] private TreeTypeEnum _treeType;
    //public int _level;
    //public int _durability;

    private bool _isUpdating = false;
    private int _id;
    [Header("Prefabs")]
    [SerializeField] private GameObject _objPrefab;
    private GameObject _obj;

    public bool IsUpdating
    {
        set => _isUpdating = value;
    }

    public Item Item
    {
        get => _item;
        set => _item = value;
    }

    [Obsolete]
    private void Start()
    {
        InitializeItem();
    }

    private void Update()
    {
        ApplyItemUpdate();
        ApplyItemBroke();
    }

    private void OnDestroy()
    {
        ApplyItemDisable();
    }

    private void ApplyItemBroke()
    {
        if (!_item.IsItemCountZero()) return;

        InventoryController.GetInstance().RemoveItem();
        Destroy(gameObject);
    }

    private void ApplyItemUpdate()
    {
        if (!_isUpdating) return;

        _obj = _item.Updating(_obj, _objPrefab);
    }

    public void InitializeItem()
    {
        _item.Init();
        //if (_item != null) return;

        //switch (_itemType)
        //{
        //    case ItemTypeEnum.None:
        //        break;
        //    case ItemTypeEnum.Instrument:
        //        _item = Instrument.CreateInstrument(_instrumentType, _level, _durability);
        //        break;
        //    case ItemTypeEnum.Seed:
        //        _item = new Seed(_seedType);
        //        break;
        //    case ItemTypeEnum.Tree:
        //        _item = new Tree(_treeType);
        //        break;
        //    case ItemTypeEnum.Fertilizers:
        //        _item = new Fertilizers(_level);
        //        break;
        //    //ToDo: Create Harvest class (https://trello.com/c/HGxBJlDN/58-create-harvest-class)
        //    case ItemTypeEnum.Harvest:
        //        _item = new Item();
        //        _item._price = 15;
        //        _item._id = (int)ItemIdsEnum.Harvest_Default;
        //        _item._isCanSold = true;
        //        break;
        //}
        //_item._type = _itemType;
    }

    public void ApplyItemDisable()
    {
        _obj = _item.StopUpdating();
    }
}
