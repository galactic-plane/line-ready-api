using System;
using System.Collections.Generic;
using System.Reflection;

namespace LineReadyApi.Extentions
{
    public static class TypeInfoMemberExtensions
    {
        public static IEnumerable<ConstructorInfo> GetAllConstructors(this TypeInfo typeInfo)
        {
            return typeInfo.GetAll(ti => ti.DeclaredConstructors);
        }

        public static IEnumerable<EventInfo> GetAllEvents(this TypeInfo typeInfo)
        {
            return typeInfo.GetAll(ti => ti.DeclaredEvents);
        }

        public static IEnumerable<FieldInfo> GetAllFields(this TypeInfo typeInfo)
        {
            return typeInfo.GetAll(ti => ti.DeclaredFields);
        }

        public static IEnumerable<MemberInfo> GetAllMembers(this TypeInfo typeInfo)
        {
            return typeInfo.GetAll(ti => ti.DeclaredMembers);
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this TypeInfo typeInfo)
        {
            return typeInfo.GetAll(ti => ti.DeclaredMethods);
        }

        public static IEnumerable<TypeInfo> GetAllNestedTypes(this TypeInfo typeInfo)
        {
            return typeInfo.GetAll(ti => ti.DeclaredNestedTypes);
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(this TypeInfo typeInfo)
        {
            return typeInfo.GetAll(ti => ti.DeclaredProperties);
        }

        private static IEnumerable<T> GetAll<T>(this TypeInfo typeInfo, Func<TypeInfo, IEnumerable<T>> accessor)
        {
            while (typeInfo != null)
            {
                foreach (T t in accessor(typeInfo))
                {
                    yield return t;
                }

                typeInfo = typeInfo.BaseType?.GetTypeInfo();
            }
        }
    }
}