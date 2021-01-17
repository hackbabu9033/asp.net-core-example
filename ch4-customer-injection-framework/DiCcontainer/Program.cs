using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using static DiCcontainer.ContainerBuilder;

namespace DiCcontainer
{
    class Program
    {
        static void Main(string[] args)
        {
            //var container = new Container();
            //container.Register<IFoo, Foo>();
            //container.Register<IBar, Bar>(LifeCycle.Transient);
            //var BarService = container.GetService<IBar>();
            //var BarService2 = container.GetService<IBar>();
            //var FooService = container.GetService<IFoo>();
            //var FooService2 = container.GetService<IFoo>();

            #region test containerBuilder
            var service = new ServiceCollection();
            service.AddTransient<IFoo, Foo>()
                .AddScoped<IBar>(_ => new Bar())
                .AddSingleton<IBaz>(new Baz());

            var factory = new ContainerProviderFactory();
            var temp = Assembly.GetEntryAssembly();
            var builder = factory.CreateBuilder(service).Register(Assembly.GetEntryAssembly());

            var container = factory.CreateServiceProvider(builder);

            GetServices(container);
            GetServices(container);
            Console.WriteLine($"\nroot container is disposed");
            (container as IDisposable)?.Dispose();
            #endregion
        }

        static void GetServices(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                Console.WriteLine("\n service scope is created");
                var child = scope.ServiceProvider;
                child.GetService<IFoo>();
                child.GetService<IBar>();
                child.GetService<IBaz>();
                child.GetService<IQux>();

                child.GetService<IFoo>();
                child.GetService<IBar>();
                child.GetService<IBaz>();
                child.GetService<IQux>();
                Console.WriteLine("\n service scope is disposed");
            }
        }
    }
  
}
