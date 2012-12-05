using System.IO;
using DBStorageLib.Attributes;
using DBStorageLib.SQLiteMembers;
using NUnit.Framework;

namespace NUnitSQLiteTest1
{
    [DBStorageParams("data source=:memory:")]
    public class TestClass : SQLiteStorageItem
    {
        [DBColumn]
        public string StringValue   { get; set; }
        [DBColumn]
        public short ShortValue     { get; set; }
        [DBColumn("AgeColumn")]
        public int IntValue         { get; set; }
        [DBColumn]
        public long LongValue       { get; set; }
        [DBColumn]
        public decimal DecimalValue { get; set; }
        [DBColumn]
        public double DoubleValue   { get; set; }
        [DBColumn]
        public float FloatValue     { get; set; }
        [DBColumn]
        public byte[] ByteArrValue  { get; set; }

        public TestClass(string name, short shortVal, int age, long longVal, decimal decimalVal,
                    double doubleVal, float floatVal, byte[] byteArrVal)
        {
            this.StringValue = name;
            this.ShortValue = shortVal;
            this.IntValue = age;
            this.LongValue = longVal;
            this.DecimalValue = decimalVal;
            this.DoubleValue = doubleVal;
            this.FloatValue = floatVal;
            this.ByteArrValue = byteArrVal;
            Save();
        }
        public TestClass()
            : base(null) 
        { }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestMethod1()
        {
            TestClass vasa = new TestClass("Vasilii", 1, short.MaxValue, int.MaxValue, new decimal(2.3), 2.3, 2.3F, new byte[] { 0, 1 });
            TestClass vasa2 = (TestClass)TestClass.Items[vasa.ID];

            Assert.AreSame(vasa, vasa2);
            Assert.AreEqual(vasa.StringValue, "Vasilii");
            Assert.AreEqual(vasa.ShortValue, 1);
            Assert.AreEqual(vasa.IntValue, short.MaxValue);
            Assert.AreEqual(vasa.LongValue, int.MaxValue);
            Assert.AreEqual(vasa.DecimalValue, new decimal(2.3));
            Assert.AreEqual(vasa.DoubleValue, 2.3);
            Assert.AreEqual(vasa.FloatValue, 2.3F);
            Assert.AreEqual(vasa.ByteArrValue, new byte[] { 0, 1 });
        }

        [Test]
        public void TestMethod2()
        {
            TestClass vasa = (TestClass)(TestClass.Items[0]);

            Assert.AreEqual(vasa.StringValue, "Vasilii");
            Assert.AreEqual(vasa.ShortValue, 1);
            Assert.AreEqual(vasa.IntValue, short.MaxValue);
            Assert.AreEqual(vasa.LongValue, int.MaxValue);
            Assert.AreEqual(vasa.DecimalValue, new decimal(2.3));
            Assert.AreEqual(vasa.DoubleValue, 2.3);
            Assert.AreEqual(vasa.FloatValue, 2.3F);
            Assert.AreEqual(vasa.ByteArrValue, new byte[] { 0, 1 });
        }

        [Test]
        public void TestMethod3()
        {
            TestClass kristina = new TestClass("Kristina", 0, 0, 0, 0, 0, 0F, null);
            long id = kristina.ID;
            kristina.Delete();

            Assert.IsFalse(TestClass.Items.ContainsKey(id));
            Assert.IsFalse(TestClass.Items.ContainsValue(kristina));
        }
    }
}
