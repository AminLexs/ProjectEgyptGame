/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Utility
{
    using System.Reflection;

    public static class ReflectionUtility
    {
        public static object GetNonPublicField<T>(T objectInstance, string fieldName)
        {
            return objectInstance.GetType()
                .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(objectInstance);
        }

        public static void SetNonPublicField<T>(T objectInstance, string fieldName, object newValue)
        {
            objectInstance.GetType()
                .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(objectInstance, newValue);
        }

        public static void ObjectCopy(object copyFrom, object copyTo)
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var copyFromFields = copyFrom.GetType().GetFields(bindingFlags);
            var copyToType = copyTo.GetType();
            for (int i = 0; i < copyFromFields.Length; ++i) {
                var copyToField = copyToType.GetField(copyFromFields[i].Name, bindingFlags);
                if (copyToField == null || !copyFromFields[i].GetType().IsAssignableFrom(copyToField.GetType())) {
                    continue;
                }
                // The field exists and is of the same type. Assign the value of copyFrom to copyTo.
                copyToField.SetValue(copyTo, copyFromFields[i].GetValue(copyFrom));
            }
        }
    }
}

