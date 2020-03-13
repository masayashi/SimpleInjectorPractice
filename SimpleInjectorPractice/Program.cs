using SimpleInjector;
using System;
using System.Threading.Tasks;

namespace SimpleInjectorPractice
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 登録
                var testModuleType = "Hoge"; // or "Foo"
                var container = CreateDiContainer(testModuleType);
                container.Verify();
                
                // DI使用。Test, Singletonは Aggregate の要求に応じてコンストラクタでDI解決して生成される
                // Sigletonははじめの一度のみ、Testは要求ごとに別インスタンスになる。
                container.GetInstance<AggregateModule>().Run();
                container.GetInstance<AggregateModule>().Run();
                container.GetInstance<AggregateModule>().Run();

                Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString(), "Catch exception.");
            }
        }

        static Container CreateDiContainer(string testModule)
        {
            var container = new Container();
            // シングルトンとして登録
            container.Register<SingletonModule>(Lifestyle.Singleton);
            // Registerに引数なしのときはTransient(要求ごとにインスタンス生成)
            if(testModule == "Hoge")
            {
                container.Register<ITestModule, TestHogeModule>();
            }
            else
            {
                container.Register<ITestModule, TestFooModule>();
            }
            container.Register<AggregateModule>();
            return container;
        }
    }

    interface ITestModule
    {
        string GetName();
    }

    // Hoge
    class TestHogeModule : ITestModule
    {
        public TestHogeModule()
        {
            Console.WriteLine($"{GetName()} construct.");
        }

        public string GetName()
        {
            return $"[TestHoge]";
        }
    }

    // Foo
    class TestFooModule : ITestModule
    {
        public TestFooModule()
        {
            Console.WriteLine($"{GetName()} construct.");
        }

        public string GetName()
        {
            return $"[TestFoo]";
        }
    }
    
    class SingletonModule
    {
        static int _createdCount = 0;
        public SingletonModule()
        {
            _createdCount++;
            Console.WriteLine($"[{GetType().Name}] construct {_createdCount}.");
        }
        public string GetName()
        {
            return $"[Sigleton:{_createdCount}]";
        }
    }

    class AggregateModule
    {
        ITestModule _test;
        SingletonModule _singleton;
        public AggregateModule(ITestModule test, SingletonModule s)
        {
            _test = test;
            _singleton = s;
        }
        public void Run()
        {
            Console.WriteLine($"[Aggregate] {_test.GetName()} {_singleton.GetName()}");
        }
    }
}
