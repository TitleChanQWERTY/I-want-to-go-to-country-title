using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private bool _isEverydayUpdating;
    [SerializeField] private List<Item> _itemsSells;
    private List<GoodsModel> _goods = new List<GoodsModel>();
    private List<GoodsModel> _goodsForDay = new List<GoodsModel>();
    private Dictionary<int, GoodsModel> _sellingItems;

    public bool IsEverydayUpdating
    {
        get => _isEverydayUpdating;
    }

    private void Start()
    {
        InitializeGoodsList();
        InitializeGoodsForDay();
    }

    private void InitializeGoodsList()
    {
        foreach (var itemSells in _itemsSells)
        {
            ItemController itemController = itemSells.Object;
            itemController.InitializeItem();
            GoodsModel goodsModel = new GoodsModel()
            {
                Goods = itemController.Item,
                Count = 99,
                Price = itemSells.Price
            };
            _goods.Add(goodsModel);
        }
    }

    private void AddValueToSellingItemDictionary(Item[] items)
    {
        foreach (var item in items)
        {
            if (item == null) continue;
            if (!item.IsCanSold) continue;

            if (_sellingItems.ContainsKey(item.Id))
            {
                _sellingItems[item.Id].Count += item.Amount;
            }
            else
            {
                GoodsModel goods = new GoodsModel()
                {
                    Goods = item,
                    Count = item.Amount,
                    Price = item.Price
                };
                _sellingItems.Add(item.Id, goods);
            }
        }
    }

    private void InitializeSellingGoods()
    {
        Item[] inventoryItems = InventoryController.GetInstance().ItemsArray;
        Item[] playerItems = InventoryController.GetInstance().PlayerItems;
        _sellingItems = null;
        _sellingItems = new Dictionary<int, GoodsModel>();

        AddValueToSellingItemDictionary(inventoryItems);
        AddValueToSellingItemDictionary(playerItems);
    }

    //TODO: add check on type of plant: https://trello.com/c/OLVCJVxh/57-check-on-type-of-plant
    public void InitializeGoodsForDay()
    {
        _goodsForDay.Clear();

        foreach (var g in _goods)
        {
            if(g.Goods.Type != ItemTypeEnum.Tree && g.Goods.Type != ItemTypeEnum.Seed)
            {
                _goodsForDay.Add(g);
                continue;
            }

            _goodsForDay.Clear();
            break;
        }
        if (_goodsForDay.Count > 0) return;

        while (_goodsForDay.Count() != 2) 
        { 
            int index = Random.Range(0, _goods.Count);
            if(_goods[index].Goods is Plant)
            {
                Plant plant = (Plant)_goods[index].Goods;
                int chance = Random.Range(1, 101);
                if (chance > (int)plant.PlantRare) continue;
            }
            var existGoods = _goodsForDay.Where(g => g.Goods.Id == _goods[index].Goods.Id);
            if (existGoods.Count() != 0) continue;

            _goodsForDay.Add(_goods[index]);
            if (_goods.Count == 1) break;
        }
    }

    public void LoadGoodsForDayToUI()
    {
        UIController.GetInstance().RedrawShop(_goodsForDay, this);
    }

    public void LoadGoodsForSellingToUI()
    {
        InitializeSellingGoods();
        UIController.GetInstance().RedrawShop(_sellingItems, this);
    }

    public void BuyItemFromShop(int index)
    {
        if (PlayerController.GetInstance().Money < _goodsForDay[index].Price) return;

        _goodsForDay[index].Count--;
        PlayerController.GetInstance().Money -= _goodsForDay[index].Price;
        Item soldGoods = _goodsForDay[index].Goods.Copy();
        soldGoods.Init();
        InventoryController.GetInstance().AddItemToMainInventory(soldGoods, 1);
        if (_goodsForDay[index].Count == 0) _goodsForDay[index] = null;

        LoadGoodsForDayToUI();
    }

    public void SellItemToShop(int key, int count)
    {
        if (count == 0) count = _sellingItems[key].Count;
        PlayerController.GetInstance().Money += _sellingItems[key].Price *
                                                (_sellingItems[key].Count >= count ? count : _sellingItems[key].Count);
        InventoryController.GetInstance().RemoveItem(_sellingItems[key].Goods, count);
        _sellingItems[key].Count -= count;

        LoadGoodsForSellingToUI();
    }
}
