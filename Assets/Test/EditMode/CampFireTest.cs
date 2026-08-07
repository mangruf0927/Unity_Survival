using NUnit.Framework;
using UnityEngine;

public class CampFireTest
{
    private GameObject campFireObject;
    private CampFire campFire;

    [SetUp]
    public void SetUp()
    {
        campFireObject = new GameObject("CampFireTest");
        campFire = campFireObject.AddComponent<CampFire>();

        CampFireData data = new()
        {
            MaxLevel = 5,
            MaxFuel = 100f,
            FuelAfterLevelUp = 20f,
            DecreaseAmount = 10,
            LevelUpDelay = 10f,
            WarningThreshold = 20f
        };

        campFire.SetUp(data);
    }

    [Test]
    public void 연료를_추가하면_현재연료가_증가한다()
    {
        campFire.AddFuel(30);
        Assert.AreEqual(30f, campFire.CurrentFuel);
    }

    [Test]
    public void 최대레벨에서_연료를_추가해도_최대연료를_넘지않는다()
    {
        CampFireSaveData data = new()
        {
            currentLevel = 5,
            currentFuel = 100f,
            isBurning = true
        };

        campFire.LoadSaveData(data);

        campFire.AddFuel(30);

        Assert.AreEqual(5, campFire.CurrentLevel);
        Assert.AreEqual(100f, campFire.CurrentFuel);
    }

    [Test]
    public void 최대연료에_도달하면_레벨업한다()
    {
        campFire.AddFuel(100);

        Assert.AreEqual(2, campFire.CurrentLevel);
        Assert.IsTrue(campFire.IsLevelingUp);
    }

    [TestCase(0)]
    [TestCase(-10)]
    public void 연료가_0이하면_추가되지않는다(float amount)
    {
        campFire.AddFuel(amount);

        Assert.AreEqual(0f, campFire.CurrentFuel);
    }

    [Test]
    public void 연료를_소모하면_설정된_양만큼_감소한다()
    {
        campFire.AddFuel(50);

        campFire.DecreaseFuelOnce();

        Assert.AreEqual(40f, campFire.CurrentFuel);
    }

    [Test]
    public void 연료가_0이되면_불이꺼진다()
    {
        campFire.AddFuel(10);

        campFire.DecreaseFuelOnce();

        Assert.AreEqual(0f, campFire.CurrentFuel);
        Assert.IsFalse(campFire.IsBurning);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(campFireObject);
    }
}
