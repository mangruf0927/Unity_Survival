using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DataTableTest
{
    private DataTable<TestData> table;
    private DataTable<ValidatableTestData> validatableTable;

    private class TestData : IGameData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    private class ValidatableTestData : IGameData, IValidatable
    {
        public int Id { get; set; }
        public bool IsValid { get; set; }
        public bool Validate() { return IsValid; }
    }

    [SetUp]
    public void SetUp()
    {
        table = new DataTable<TestData>();
        validatableTable = new DataTable<ValidatableTestData>();
    }

    [Test]
    public void 데이터를_로드하면_ID로_조회할수있다()
    {
        // Arrange
        TestData data = new()
        {
            Id = 1001,
            Name = "Wood"
        };

        // Act
        table.Load(new[] { data });

        // Assert
        Assert.AreSame(data, table.Get(1001));
    }

    [Test]
    public void 중복_ID가_있으면_첫번째_데이터만_등록된다()
    {
        // Arrange
        TestData first = new()
        {
            Id = 1001,
            Name = "first"
        };

        TestData second = new()
        {
            Id = 1001,
            Name = "second"
        };

        LogAssert.Expect(LogType.Error, "Duplicate id: TestData - 1001");

        // Act
        table.Load(new[] { first, second });

        // Assert
        Assert.AreSame(first, table.Get(1001));
        Assert.AreEqual(1, table.All.Count);
    }

    [Test]
    public void 없는_ID를_Get하면_null을_반환한다()
    {
        LogAssert.Expect(LogType.Error, "Not found id: TestData - 9999");

        // Act
        TestData result = table.Get(9999);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public void 검증에_실패한_데이터는_등록되지않는다()
    {
        // Arrange
        ValidatableTestData invalidData = new()
        {
            Id = 1001,
            IsValid = false
        };

        LogAssert.Expect(LogType.Error, "Invalid data skipped: ValidatableTestData - Id: 1001");

        // Act
        validatableTable.Load(new[] { invalidData });

        // Assert
        Assert.IsFalse(validatableTable.TryGet(1001, out _));
        Assert.AreEqual(0, validatableTable.All.Count);
    }

    [Test]
    public void ID가_존재하면_true와_데이터를_반환한다()
    {
        // Arrange
        TestData data = new()
        {
            Id = 1001,
            Name = "Wood"
        };

        table.Load(new[] { data });

        // Act
        bool result = table.TryGet(1001, out TestData found);

        // Assert
        Assert.IsTrue(result);
        Assert.AreSame(data, found);
    }

    [Test]
    public void ID가_존재하지않으면_false를_반환한다()
    {
        // Act
        bool result = table.TryGet(9999, out TestData found);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(found);
    }
}