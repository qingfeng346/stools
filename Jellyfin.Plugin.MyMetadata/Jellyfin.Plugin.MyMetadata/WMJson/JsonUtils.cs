namespace WMJson {
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Linq;
    internal static class JsonUtils {
        private readonly static Type ObjectType = typeof(object);
        private readonly static Type BoolType = typeof(bool);
        private readonly static Type Int8Type = typeof(sbyte);
        private readonly static Type UInt8Type = typeof(byte);
        private readonly static Type Int16Type = typeof(short);
        private readonly static Type UInt16Type = typeof(ushort);
        private readonly static Type Int32Type = typeof(int);
        private readonly static Type UInt32Type = typeof(uint);
        private readonly static Type Int64Type = typeof(long);
        private readonly static Type UInt64Type = typeof(ulong);
        private readonly static Type FloatType = typeof(float);
        private readonly static Type DoubleType = typeof(double);
        private readonly static Type DecimalType = typeof(decimal);
        private readonly static Type StringType = typeof(string);
        private readonly static Type IDictionaryType = typeof(IDictionary<,>);
        private readonly static Type IEnumerableType = typeof(IEnumerable<>);
        private readonly static Type JObjectType = typeof(JObject);
        private readonly static Type JValueType = typeof(JValue);
        private readonly static Type JArrayType = typeof(JArray);
        private readonly static Type JMapType = typeof(JMap);
        private readonly static IJsonContract NullContract = new JsonNullContract();
        public readonly static IJsonContract StringContract = new JsonStringContract();
        private static Dictionary<Type, IJsonContract> TypeContract = new Dictionary<Type, IJsonContract>();
        public static void ClearCache() {
            TypeContract.Clear();
        }
        static bool IsNullableType(Type t) {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        static bool IsDictionary(Type type, out Type keyType, out Type valueType) {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == IDictionaryType) {
                keyType = type.GetGenericArguments()[0];
                valueType = type.GetGenericArguments()[1];
                return true;
            }
            foreach (Type i in type.GetInterfaces()) {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == IDictionaryType) {
                    keyType = i.GetGenericArguments()[0];
                    valueType = i.GetGenericArguments()[1];
                    return true;
                }
            }
            keyType = null;
            valueType = null;
            return false;
        }
        static bool IsEnumerable(Type type, out Type valueType) {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == IEnumerableType) {
                valueType = type.GetGenericArguments()[0];
                return true;
            }
            foreach (Type i in type.GetInterfaces()) {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == IEnumerableType) {
                    valueType = i.GetGenericArguments()[0];
                    return true;
                }
            }
            valueType = null;
            return false;
        }
        public static bool IsAttribute<T>(MemberInfo memberInfo) where T : Attribute {
            return Attribute.GetCustomAttributes(memberInfo, typeof(T)).Length > 0;
        }
        public static Func<object> GetCreateFunc(Type type) {
            if (type.IsValueType) {
                return () => Activator.CreateInstance(type);
            }
            var constructorInfo = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).SingleOrDefault(x => x.GetParameters().Length == 0);
            if (constructorInfo == null) {
                throw new InvalidOperationException("Unable to find default constructor for " + type.FullName);
            }
            return () => constructorInfo.Invoke(null);
        }
        public static IJsonContract GetContract(Type type) {
            if (type == null)
                return NullContract;
            type = IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;
            if (TypeContract.TryGetValue(type, out var contract)) {
                return contract;
            } else {
                if (type == ObjectType || type == JObjectType)
                    return TypeContract[type] = new JsonJObjectContract();
                else if (type.IsEnum)
                    return TypeContract[type] = new JsonEnumContract(type);
                else if (type == BoolType)
                    return TypeContract[type] = new JsonBoolContract();
                else if (type == Int8Type)
                    return TypeContract[type] = new JsonInt8Contract();
                else if (type == UInt8Type)
                    return TypeContract[type] = new JsonUInt8Contract();
                else if (type == Int16Type)
                    return TypeContract[type] = new JsonInt16Contract();
                else if (type == UInt16Type)
                    return TypeContract[type] = new JsonUInt16Contract();
                else if (type == Int32Type)
                    return TypeContract[type] = new JsonInt32Contract();
                else if (type == UInt32Type)
                    return TypeContract[type] = new JsonUInt32Contract();
                else if (type == Int64Type)
                    return TypeContract[type] = new JsonInt64Contract();
                else if (type == UInt64Type)
                    return TypeContract[type] = new JsonUInt64Contract();
                else if (type == FloatType)
                    return TypeContract[type] = new JsonFloatContract();
                else if (type == DoubleType)
                    return TypeContract[type] = new JsonDoubleContract();
                else if (type == DecimalType)
                    return TypeContract[type] = new JsonDecimalContract();
                else if (type == StringType)
                    return TypeContract[type] = StringContract;
                else if (type == JValueType)
                    return TypeContract[type] = new JsonJValuetContract();
                else if (type == JArrayType)
                    return TypeContract[type] = new JsonJArrayContract();
                else if (type == JMapType)
                    return TypeContract[type] = new JsonJMapContract();
                else if (IsDictionary(type, out var keyType, out var valueType))
                    return TypeContract[type] = new JsonMapContract(type, keyType, valueType);
                else if (type.IsArray)
                    return TypeContract[type] = new JsonArrayContract(type, type.GetElementType());
                else if (IsEnumerable(type, out var listValueType))
                    return TypeContract[type] = new JsonEnumerableContract(type, listValueType);
                else
                    return TypeContract[type] = new JsonObjectContract(type);
            }
        }
    }
}
