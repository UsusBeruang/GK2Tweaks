using System;
using System.Reflection;

namespace GK2Tweaks.Compatibility
{
    internal static class SaveAnywhereCompatibility
    {
        private const BindingFlags InstanceFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private const BindingFlags StaticFlags =
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static void Validate()
        {
            RequireProperty(typeof(MainGame), "Instance", typeof(MainGame), isStatic: true);
            RequireProperty(typeof(MainGame), "gameState", typeof(GameState), isStatic: false);
            RequireProperty(typeof(MainGame), "GameSave", typeof(GameSave), isStatic: false);
            RequireProperty(typeof(MainGame), "SaveSlotData", typeof(SaveSlotData), isStatic: false);
            RequireProperty(typeof(MainGame), "PlayerController", typeof(PlayerController), isStatic: true);

            RequireField(typeof(GameSave), "worldData", typeof(WorldData), isStatic: false);
            RequireField(typeof(WorldData), "gdPointsData", typeof(GdPointsData), isStatic: false);

            RequireMethod(
                typeof(GdPointsData),
                "GetGDPointDataByInstanceId",
                typeof(GDPointData),
                isStatic: false,
                typeof(int));

            RequireConstructor(
                typeof(GDPointTeleportData),
                typeof(GDPointData),
                typeof(string),
                typeof(string),
                typeof(Action),
                typeof(bool),
                typeof(float));

            RequireMethod(
                typeof(PlayerController),
                "Teleport",
                typeof(bool),
                isStatic: true,
                typeof(TeleportDataBase));

            RequireMethod(
                typeof(SaveSystem),
                "Save",
                typeof(void),
                isStatic: true,
                typeof(SaveSlotData),
                typeof(GameSave),
                typeof(Action),
                typeof(Action),
                typeof(bool),
                typeof(Action<SaveSlotData, GameSave>));
        }

        private static void RequireProperty(
            Type type,
            string name,
            Type propertyType,
            bool isStatic)
        {
            BindingFlags flags = isStatic ? StaticFlags : InstanceFlags;
            PropertyInfo property = type.GetProperty(name, flags);

            if (property == null
                || property.PropertyType != propertyType
                || property.GetMethod == null
                || property.GetMethod.IsStatic != isStatic)
            {
                throw new MissingMemberException(
                    type.FullName,
                    $"{name} : {propertyType.FullName}");
            }
        }

        private static void RequireField(
            Type type,
            string name,
            Type fieldType,
            bool isStatic)
        {
            BindingFlags flags = isStatic ? StaticFlags : InstanceFlags;
            FieldInfo field = type.GetField(name, flags);

            if (field == null
                || field.FieldType != fieldType
                || field.IsStatic != isStatic)
            {
                throw new MissingFieldException(
                    type.FullName,
                    $"{name} : {fieldType.FullName}");
            }
        }

        private static void RequireMethod(
            Type type,
            string name,
            Type returnType,
            bool isStatic,
            params Type[] parameterTypes)
        {
            BindingFlags flags = isStatic ? StaticFlags : InstanceFlags;
            MethodInfo method = type.GetMethod(
                name,
                flags,
                binder: null,
                types: parameterTypes,
                modifiers: null);

            if (method == null
                || method.ReturnType != returnType
                || method.IsStatic != isStatic)
            {
                throw new MissingMethodException(
                    type.FullName,
                    $"{name}({string.Join(", ", (object[])parameterTypes)})");
            }
        }

        private static void RequireConstructor(Type type, params Type[] parameterTypes)
        {
            ConstructorInfo constructor = type.GetConstructor(
                InstanceFlags,
                binder: null,
                types: parameterTypes,
                modifiers: null);

            if (constructor == null)
            {
                throw new MissingMethodException(
                    type.FullName,
                    $".ctor({string.Join(", ", (object[])parameterTypes)})");
            }
        }
    }
}
