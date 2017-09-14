using IQToolkit.Data;
using IQToolkit.Data.Mapping;
using IQToolkit.Data.SQLite;
using System;

namespace Test
{
    class Program
    {
        public static void Main(string[] args)
        {
            new TestRunner(args, System.Reflection.Assembly.GetEntryAssembly()).RunTests();
        }

        private static DbEntityProvider CreateNorthwindProvider()
        {
            return new SQLiteQueryProvider("Northwind.db3", new AttributeMapping(typeof(Test.NorthwindWithAttributes)));
        }

        public class NorthwindTranslationTests 
            : Test.NorthwindTranslationTests
        {
            protected override DbEntityProvider CreateProvider()
            {
                return CreateNorthwindProvider();
            }
        }

        public class NorthwindExecutionTests 
            : Test.NorthwindExecutionTests
        {
            protected override DbEntityProvider CreateProvider()
            {
                return CreateNorthwindProvider();
            }
        }

        public class NorthwindCUDTests 
            : Test.NorthwindCUDTests
        {
            protected override DbEntityProvider CreateProvider()
            {
                return CreateNorthwindProvider();
            }
        }
    }
}
