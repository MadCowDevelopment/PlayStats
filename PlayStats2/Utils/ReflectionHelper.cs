using System;
using System.Reflection;

namespace PlayStats.Utils
{
    public static class ReflectionHelper
    {
        public static void InvokeGenericMethod(this object instance, Type typeArgument, string methodName, params object[] parameters)
        {
            var method = CreateGenericMethodInfo(instance, typeArgument, methodName);
            method.Invoke(instance, parameters);
        }

        public static MethodInfo CreateGenericMethodInfo(object instance, Type typeArgument, string methodName)
        {
            var currentType = instance.GetType();
            MethodInfo method;
            do
            {
                method = currentType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                currentType = currentType.BaseType;
            }
            while (method == null && currentType != null);

            if (method == null)
            {
                throw new InvalidOperationException($"Method '{methodName}' does not exist in type {instance.GetType()}.");
            }

            return method.MakeGenericMethod(typeArgument);
        }
    }
}
