using System;
using System.Collections.Generic;
using DBStorageLib;
using DBStorageLib.Attributes;
using DBStorageLib.SQLiteMembers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SLiteTestList1Steps
{
    [DBStorageParams("data source=:memory:")]
    public class TestClass : SQLiteStorageItem
    {
        [DBColumn]
        public string StringValue { get; set; }
        [DBColumn]
        public short ShortValue { get; set; }
        [DBColumn("AgeColumn")]
        public int IntValue { get; set; }
        [DBColumn]
        public long LongValue { get; set; }
        [DBColumn]
        public decimal DecimalValue { get; set; }
        [DBColumn]
        public double DoubleValue { get; set; }
        [DBColumn]
        public float FloatValue { get; set; }
        [DBColumn]
        public Guid GuidValue { get; set; }

        public TestClass(string name, short shortVal, Guid guidVal, int age = short.MaxValue,
                         long longVal = int.MaxValue, decimal decimalVal = decimal.One,
                        double doubleVal = 2.3, float floatVal = 2.3F)
        {
            this.StringValue = name;
            this.ShortValue = shortVal;
            this.IntValue = age;
            this.LongValue = longVal;
            this.DecimalValue = decimalVal;
            this.DoubleValue = doubleVal;
            this.FloatValue = floatVal;
            this.GuidValue = guidVal;
            Save();
        }
        public TestClass()
            : base(null)
        { }
    }

    [DBStorageParams("data source=test1.db")]
    public class User : SQLiteStorageItem
    {
        [DBColumn]
        public string Name;
        [DBColumn]
        public int Age { get; set; }
        [DBColumn]
        private Guid _someObjectID;

        public User(string name, int age)
        {
            this.Name = name;
            this.Age = age;
            _someObjectID = Guid.NewGuid();
        }

        public User() : base(null) { }
    }

    [TestClass]
    public class Tests
    {
        public TestClass vasa;
        public TestClass kristina;
        public TestClass vitja;
        
        [TestMethod]
        public void TestMethod1()
        {
            Core.Init();

            this.vasa = new TestClass("Vasilii", 1, Guid.NewGuid());
            this.kristina = new TestClass("Kristina", 2, Guid.NewGuid());
            this.vitja = new TestClass("Vitja", 3, Guid.NewGuid());
            SQLiteStorage storage = (SQLiteStorage)Core.GetStorage(typeof(TestClass));
            TestClass vasa2 = (TestClass)storage.Items[vasa.ID];

            Assert.AreSame(vasa, vasa2);
            Assert.AreEqual(vasa.StringValue, "Vasilii");
            Assert.AreEqual(vasa.ShortValue, 1);
            Assert.AreEqual(vasa.IntValue, short.MaxValue);
            Assert.AreEqual(vasa.LongValue, int.MaxValue);
            Assert.AreEqual(vasa.DecimalValue, decimal.One);
            Assert.AreEqual(vasa.DoubleValue, 2.3);
            Assert.AreEqual(vasa.FloatValue, 2.3F);
            Assert.AreNotEqual(vasa.GuidValue, null);
        }

        [TestMethod]
        public void TestMethod2()
        {
            SQLiteStorage storage = (SQLiteStorage)Core.GetStorage(typeof(TestClass));
            Assert.AreEqual(storage.Items.Count, 3);
            Assert.AreEqual(((TestClass)storage.Items[2]).StringValue, "Vitja");
            storage.SaveToDisk();
            storage.LoadFromDisk();
            Assert.AreEqual(storage.Items.Count, 3);
        }

        [TestMethod]
        public void TestMethod3()
        {
            TestClass x = new TestClass(default(string), default(short), default(Guid),
                                        default(int), default(long), default(decimal), 
                                        default(double), default(float));
            SQLiteStorage storage = (SQLiteStorage)Core.GetStorage(typeof(TestClass));
            storage.SaveToDisk();
            storage.LoadFromDisk();
            Assert.AreEqual(x.StringValue, default(string));
            Assert.AreEqual(x.ShortValue, default(short));
            Assert.AreEqual(x.IntValue, default(int));
            Assert.AreEqual(x.LongValue, default(long));
            Assert.AreEqual(x.DecimalValue, default(decimal));
            Assert.AreEqual(x.DoubleValue, default(double));
            Assert.AreEqual(x.FloatValue, default(float));
            Assert.AreNotEqual(x.GuidValue, null);
        }

        [TestMethod]
        public void TestMethod4()
        {
            SQLiteStorage storage = (SQLiteStorage)Core.GetStorage(typeof(TestClass));
            List<SQLiteStorageItem> toDelete = new List<SQLiteStorageItem>();
            foreach (SQLiteStorageItem item in storage.Items.Values)
            {
                toDelete.Add(item);
            }
            foreach (var item in toDelete)
            {
                item.Delete();
            }
            storage.SaveToDisk();
            storage.LoadFromDisk();
            Assert.AreEqual(storage.Items.Count, 0);
        }

        [TestMethod]
        public void TestMethod5()
        {
            SQLiteStorage storage = (SQLiteStorage)Core.GetStorage(typeof(User));

            Assert.AreEqual(storage.ColumnBindings.Count, 3);

            Core.Close();
        }

    }
}
