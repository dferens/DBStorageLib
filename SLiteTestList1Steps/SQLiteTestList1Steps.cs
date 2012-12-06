using System;
using DBStorageLib.Attributes;
using DBStorageLib.SQLiteMembers;
using DBStorageLib.BaseMembers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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

    [DBStorageParams("data source=..\\..\test1.db")]
    public class User : SQLiteStorageItem
    {
        [DBColumn]
        public string Name;
        [DBColumn]
        public int Age { get; set; }

        public User(string name, int age)
        {
            this.Name = name;
            this.Age = age;
            Save();
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
            this.vasa = new TestClass("Vasilii", 1, Guid.NewGuid());
            this.kristina = new TestClass("Kristina", 2, Guid.NewGuid());
            this.vitja = new TestClass("Vitja", 3, Guid.NewGuid());
            TestClass vasa2 = (TestClass)TestClass.Items[vasa.ID];

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
            Assert.AreEqual(TestClass.Items.Count, 3);
            Assert.AreEqual(((TestClass)TestClass.Items[2]).StringValue, "Vitja");
            DBStorage storage = TestClass.Items[0].Storage;
            storage.SaveToDisk();
            storage.LoadFromDisk();
            Assert.AreEqual(TestClass.Items.Count, 3);
        }

        [TestMethod]
        public void TestMethod3()
        {
            TestClass x = new TestClass(default(string), default(short), default(Guid),
                                        default(int), default(long), default(decimal), 
                                        default(double), default(float));
            DBStorage storage = x.Storage;
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
            DBStorage storage = DBStorage.GetStorage(typeof(TestClass));
            List<DBStorageItem> toDelete = new List<DBStorageItem>();
            foreach (DBStorageItem item in DBStorageItem.Items.Values)
            {
                toDelete.Add(item);
            }
            foreach (var item in toDelete)
            {
                item.Delete();
            }
            storage.SaveToDisk();
            storage.LoadFromDisk();
            Assert.AreEqual(TestClass.Items.Count, 0);
        }

        [TestMethod]
        public void TestMethod5()
        {
            var storage = DBStorage.GetStorage(typeof(User));
            User u1 = User.Items[0] as User;
            User u2 = User.Items[1] as User;
            User u3 = User.Items[2] as User;
            Assert.AreEqual(u1.Age, 91);
            Assert.AreEqual(u2.Age, 92);
            Assert.AreEqual(u3.Age, 93);
            Assert.AreEqual(u1.Name, "User1Name");
            Assert.AreEqual(u2.Name, "User2Name");
            Assert.AreEqual(u3.Name, "User3Name");
        }
    }
}
