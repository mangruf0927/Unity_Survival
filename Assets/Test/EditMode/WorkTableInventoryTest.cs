using NUnit.Framework;
using UnityEngine;

public class WorkTableInventoryTest
{
    private GameObject workTableObject;
    private WorkTableInventory workTable;
    private WorkTableItem item;

    [SetUp]
    public void SetUp()
    {
        workTableObject = new GameObject("WorkTableTest");
        workTable = workTableObject.AddComponent<WorkTableInventory>();
        item = new WorkTableItem
        {
            needIron = 5,
            needWood = 5,
            requiredLevel = 1,
            unlocksNextLevel = true,
            purchaseLimit = 1
        };
    }

    [Test]
    public void 나무를_추가하면_수량이_증가한다()
    {
        workTable.AddMaterial(MaterialType.WOOD, 10);

        Assert.AreEqual(10, workTable.Wood);
    }

    [Test]
    public void 철을_추가하면_수량이_증가한다()
    {
        workTable.AddMaterial(MaterialType.IRON, 10);

        Assert.AreEqual(10, workTable.Iron);
    }

    [Test]
    public void 재료가_부족하면_구매할수없다()
    {
        bool result = workTable.CanBuy(item);

        Assert.IsFalse(result);
    }

    [Test]
    public void 재료가_충분하면_구매할수있다()
    {
        workTable.AddMaterial(MaterialType.WOOD, 10);
        workTable.AddMaterial(MaterialType.IRON, 10);

        bool result = workTable.CanBuy(item);

        Assert.IsTrue(result);
    }

    [Test]
    public void 요구레벨을_충족하지못하면_구매할수없다()
    {
        item.requiredLevel = 2;

        workTable.AddMaterial(MaterialType.WOOD, 10);
        workTable.AddMaterial(MaterialType.IRON, 10);

        bool result = workTable.CanBuy(item);

        Assert.IsFalse(result);
        Assert.AreNotEqual(workTable.CurrentLevel, item.requiredLevel);
    }

    [Test]
    public void 업그레이드_아이템을_구매하면_레벨이_증가한다()
    {
        workTable.AddMaterial(MaterialType.WOOD, 10);
        workTable.AddMaterial(MaterialType.IRON, 10);

        bool result = workTable.BuyItem(item);

        Assert.IsTrue(result);
        Assert.AreEqual(2, workTable.CurrentLevel);
    }

    [Test]
    public void 구매횟수가_제한에_도달하면_다시_구매할수없다()
    {
        workTable.AddMaterial(MaterialType.WOOD, 10);
        workTable.AddMaterial(MaterialType.IRON, 10);
        workTable.BuyItem(item);

        bool result = workTable.CanBuy(item);

        Assert.IsFalse(result);
        Assert.IsTrue(workTable.IsSoldOut(item));
        Assert.AreEqual(1, workTable.GetPurchaseCount(item));
    }

    [Test]
    public void 업그레이드_아이템을_구매하면_레벨이_증가하고_재료가_차감된다()
    {
        workTable.AddMaterial(MaterialType.WOOD, 10);
        workTable.AddMaterial(MaterialType.IRON, 10);
        workTable.BuyItem(item);

        bool result = workTable.CanBuy(item);

        Assert.IsFalse(result);
        Assert.AreEqual(2, workTable.CurrentLevel);

        Assert.AreEqual(5, workTable.Wood);
        Assert.AreEqual(5, workTable.Iron);

        Assert.AreEqual(1, workTable.GetPurchaseCount(item));
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(workTableObject);
    }
}
