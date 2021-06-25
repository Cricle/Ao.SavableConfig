using Ao.SavableConfig.Binder.Visitors;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace Ao.SavableConfig.Binder
{
    public class ProxyCreator : ObjectNamedCreator
    {
        public ProxyCreator(ProxyHelper proxyHelper,
            Type type,
            INameTransfer nameTransfer,
            INamedCreator namedCreator,
            IPropertyVisitor propertyVisitor)
            : base(type, nameTransfer, namedCreator, propertyVisitor)
        {
            ProxyHelper = proxyHelper ?? throw new ArgumentNullException(nameof(proxyHelper));
            BuildType(type);
        }
        public ProxyHelper ProxyHelper { get; }
        protected override void BuildType(Type type)
        {
            ProxyHelper.BuildProx(type);
        }
        protected override object CreateInstance(Type type, IConfiguration configuration)
        {
            var inst = ProxyHelper.CreateProxy(type, configuration, NameTransfer);
            return inst;
        }
        protected override ObjectNamedCreator CreateCreator(PropertyInfo info, INameTransfer transfer)
        {
            return new ProxyCreator(ProxyHelper,
                        info.PropertyType,
                        transfer,
                        NamedCreator,
                        PropertyVisitor);
        }
    }
}
