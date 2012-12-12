using System;
using DBStorageLib;
using DBStorageLib.Attributes;
using DBStorageLib.SQLiteMembers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SQLiteRelationTest1
{
    [TestClass]
    public class SQLiteRelationTest1Steps
    {
        [DBStorageParams("data source=reltest.db")]
        public class User : SQLiteStorageItem
        {
            [DBColumn]
            public string Name;
            [DBColumn]
            private Guid _relatedUserID;
            public User RelatedUser
            {
                get
                {
                    return (User)this.Storage.Items[_relatedUserID];
                }
                set
                {
                    _relatedUserID = value.ID;
                }
            }
            
            public User(string name)
            {
                this.Name = name;
            }
            private User() : base(null) { }
        }

        [TestMethod]
        public void TestMethod1()
        {
            Core.Init();
            // 
            SQLiteStorage storage = (SQLiteStorage)Core.GetStorage(typeof(User));
            User user1, user2, user3;

            if (storage.Items.Count < 3)
            {
                user1 = new User("User1");
                user2 = new User("User2");
                user3 = new User("User3");

                user1.RelatedUser = user2;
                user2.RelatedUser = user3;
                user3.RelatedUser = user1;

                Assert.AreEqual(user1.RelatedUser.Name, "User2");
                Assert.AreEqual(user2.RelatedUser.Name, "User3");
                Assert.AreEqual(user3.RelatedUser.Name, "User1");
            }
        }
    }
}
