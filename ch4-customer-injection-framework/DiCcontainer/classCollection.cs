using System;
using System.Collections.Generic;
using System.Text;

namespace DiCcontainer
{
    public interface IFoo { public int GenId { set; get; } }
    
    public interface IBar { public int GenId { set; get; } }

    public interface IQux { }

    public interface IBaz { }
    

    public interface IMouse { public int GenId { set; get; } }
    

    public interface ISport<T1, T2> { public int GenId { set; get; } }
    

    public class Foo : Base,IFoo 
    {
        public static int Id { set; get; } = 0;
        public int GenId { set; get; }

        public Foo()
        {
            Id++;
            GenId = Id;
        }

    }
    public class Bar : Base, IBar 
    {
        public static int Id { set; get; } = 0;
        public int GenId { set; get; }
        public Bar()
        {
            Id++;
            GenId = Id;
        }
    }
    public class Mouse : Base, IMouse
    {
        public static int Id { set; get; } = 0;
        public int GenId { set; get; }

        public Mouse()
        {
            Id++;
            GenId = Id;
        }
    }

    public class Sport<T1,T2> : Base, ISport<T1, T2> 
    {
        public static int Id { set; get; } = 0;

        public T1 item1;
        public T2 item2;
        public int GenId { set; get; }

        public Sport(T1 item1,T2 item2)
        {
            Id++;
            GenId = Id;
            this.item1 = item1;
            this.item2 = item2;
        }

    }

    public class Qux : Base, IQux
    {

    }

    public class Baz : Base, IBaz
    {

    }

    public class Base : IDisposable
    {
        public Base() => Console.WriteLine($"instance of {GetType().Name} is created");

        public void Dispose()
        {
            Console.WriteLine($"instance of {GetType().Name} is disposed");
        }
    }
}
