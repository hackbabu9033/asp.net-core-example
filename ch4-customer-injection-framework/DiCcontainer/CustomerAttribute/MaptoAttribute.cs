using System;
using System.Collections.Generic;
using System.Text;

namespace DiCcontainer.CustomerAttribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MaptoAttribute : Attribute
    {
        public LifeCycle LifeCycle { set; get; }

        public Type ServiceType { set; get; }

        public MaptoAttribute(LifeCycle lifeCycle, Type type)
        {
            LifeCycle = lifeCycle;
            ServiceType = type;
        }
    }
}
