DBStorageLib
============

Basic ORM features, like storing objects in an automatically generated table, adding/deleting, with focus on different DBMS.
Is used by .NET labs in my university

How-to use :
-----
1. Add "DBStorageParams" attribute to your class and define connection string.
2. Derive your class from XXXStorageItem (for example - SQLiteStorageItem).
3. Add "DBColumn" attribute to any public/private field/property you want to save in database.
4. Declare private parameterless constructor that calls "base(null)".
5. Call "Core.Init" before any use, and "Core.Close" after last use of your instances"

Example
-------
    // in EmploymentRecord.cs
    [DBStorageParams("data source=database.db")]
    public class EmploymentRecord : SQLiteStorageItem
    {
        [DBColumn]
        private Guid _positionID;     // will save with "_positionID" column name 
        [DBColumn("YourCustomName")]  // will save with "YourCustomName" column name
        private Guid _personID;
        public Position Position { /* code here */ }
        public Person Person { /* code here */ }
        [DBColumn]
        public Decimal EmploymentValue;
        [DBColumn]
        public DateTime HireDate;
        [DBColumn]
        public DateTime FireDate;

        public EmploymentRecord(Position position, Person person, Decimal employmentValue,
                                DateTime hireDate, DateTime fireDate)
        {
            this.Position = position;
            this.Person = person;
            this.EmploymentValue = employmentValue;
            this.HireDate = hireDate;
            this.FireDate = fireDate;
        }
        
        // For internal library's usage
        private EmploymentRecord() : base(null) { }
    }
    
    // in Program.cs or anywhere
    static void Main(string[] args)
    {
        DBStorageLib.Core.Init();
        
        // Your code
        
        DBStorageLib.Core.Close();
    }
    

