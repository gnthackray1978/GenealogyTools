using System;
using System.Reflection;

namespace FTMContextNet.Domain.ExtensionMethods
{
    public static class ObjectExtender
    {
        public static TReturn CallPrivateMethod<TReturn>(
            this object instance,
            string methodName,
            params object[] parameters)
        {
            Type type = instance.GetType();
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo method = type.GetMethod(methodName, bindingAttr);

            return (TReturn)method.Invoke(instance, parameters);
        }
    }
}
