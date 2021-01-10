using System;
using System.Collections.Generic;
using System.Text;

namespace DiCcontainer
{
    public class RegistryService
    {
        public Type ServiceType { get; set; }
        public LifeCycle LifeCycle { get; set; }
        public Func<LifeCycle, Type[], object> ServiceFac { set; get; }
        public RegistryService Next { set; get; }

        public RegistryService(Type type, LifeCycle lifeCycle,Func<LifeCycle, Type[], object> serviceFac)
        {
            ServiceType = type;
            LifeCycle = lifeCycle;
            ServiceFac = serviceFac;
        }

    }
}
