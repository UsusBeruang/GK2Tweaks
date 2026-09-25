using System;
using System.Reflection;

namespace GK2Tweaks.Compatibility
{
    internal static class ReflectionContract
    {
        private const BindingFlags InstanceMembers =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private const BindingFlags StaticMembers =
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static PropertyInfo RequireProperty(
            Type owner,
            string name,
            bool isStatic)
        {
            BindingFlags flags = isStatic ? StaticMembers : InstanceMembers;
            PropertyInfo property = owner.GetProperty(name, flags);
            MethodInfo getter = property?.GetGetMethod(nonPublic: true);

            if (property == null
                || getter == null
                || getter.IsStatic != isStatic)
            {
                throw new MissingMemberException(owner.FullName, name);
            }

            return property;
        }

        public static void RequireProperty(
            Type owner,
            string name,
            Type valueType,
            bool isStatic)
        {
            PropertyInfo property = RequireProperty(owner, name, isStatic);

            if (property.PropertyType != valueType)
            {
                throw new MissingMemberException(
                    owner.FullName,
                    $"{name} : {valueType.FullName}");
            }
        }

        public static void RequireWritableProperty(
            Type owner,
            string name,
            Type valueType,
            bool isStatic)
        {
            PropertyInfo property = RequireProperty(owner, name, isStatic);
            MethodInfo setter = property.GetSetMethod(nonPublic: true);

            if (property.PropertyType != valueType
                || setter == null
                || setter.IsStatic != isStatic)
            {
                throw new MissingMemberException(
                    owner.FullName,
                    $"{name} : {valueType.FullName}");
            }
        }

        public static void RequireField(
            Type owner,
            string name,
            Type valueType,
            bool isStatic)
        {
            BindingFlags flags = isStatic ? StaticMembers : InstanceMembers;
            FieldInfo field = owner.GetField(name, flags);

            if (field == null
                || field.FieldType != valueType
                || field.IsStatic != isStatic)
            {
                throw new MissingFieldException(
                    owner.FullName,
                    $"{name} : {valueType.FullName}");
            }
        }

        public static void RequireMethod(
            Type owner,
            string name,
            Type returnType,
            bool isStatic,
            params Type[] parameterTypes)
        {
            BindingFlags flags = isStatic ? StaticMembers : InstanceMembers;
            MethodInfo method = owner.GetMethod(
                name,
                flags,
                binder: null,
                types: parameterTypes,
                modifiers: null);

            if (method == null
                || method.ReturnType != returnType
                || method.IsStatic != isStatic)
            {
                throw new MissingMethodException(owner.FullName, name);
            }
        }

        public static void RequireConstructor(Type owner, params Type[] parameterTypes)
        {
            ConstructorInfo constructor = owner.GetConstructor(
                InstanceMembers,
                binder: null,
                types: parameterTypes,
                modifiers: null);

            if (constructor == null)
                throw new MissingMethodException(owner.FullName, ".ctor");
        }
    }
}
