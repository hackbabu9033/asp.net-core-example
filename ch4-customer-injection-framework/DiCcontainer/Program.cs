using System;

namespace DiCcontainer
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container();
            container.Register<IFoo, Foo>();
            container.Register<IBar, Bar>(LifeCycle.Transient);
            var BarService = container.GetService<IBar>();
            var BarService2 = container.GetService<IBar>();
            var FooService = container.GetService<IFoo>();
            var FooService2 = container.GetService<IFoo>();
        }
    }
}
