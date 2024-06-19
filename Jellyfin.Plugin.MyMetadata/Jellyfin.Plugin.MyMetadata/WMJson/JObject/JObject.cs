namespace WMJson {
    using System;
    using System.Globalization;

    public enum JObjectType {
        Null,
        True,
        False,
        Int8,
        UInt8,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        Float,
        Double,
        Decimal,
        String,
        Map,
        Array,
    }
    public abstract class JObject {
        public JObjectType ValueType { get; private set; }
        public object Value { get; protected set; }
        public JObject(JObjectType valueType, object value = null) {
            ValueType = valueType;
            Value = value;
        }
        public virtual JObject this[string key] {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public override string ToString() => Value == null ? "null" : Value.ToString();
        public virtual void Add(JObject obj) => throw new NotImplementedException();
        public static explicit operator bool(JObject v) => v.ValueType == JObjectType.True;
        public static explicit operator sbyte(JObject v) => Convert.ToSByte(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator byte(JObject v) => Convert.ToByte(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator short(JObject v) => Convert.ToInt16(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator ushort(JObject v) => Convert.ToUInt16(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator int(JObject v) => Convert.ToInt32(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator uint(JObject v) => Convert.ToUInt32(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator long(JObject v) => Convert.ToInt64(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator ulong(JObject v) => Convert.ToUInt64(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator float(JObject v) => Convert.ToSingle(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator double(JObject v) => Convert.ToDouble(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator decimal(JObject v) => Convert.ToDecimal(v.Value, CultureInfo.InvariantCulture);
        public static explicit operator string(JObject v) => v.Value.ToString();


        public static explicit operator JObject(bool v) => v ? JValue.True : JValue.False;
        public static explicit operator JObject(sbyte v) => new JValue(v);
        public static explicit operator JObject(byte v) => new JValue(v);
        public static explicit operator JObject(short v) => new JValue(v);
        public static explicit operator JObject(ushort v) => new JValue(v);
        public static explicit operator JObject(int v) => new JValue(v);
        public static explicit operator JObject(uint v) => new JValue(v);
        public static explicit operator JObject(long v) => new JValue(v);
        public static explicit operator JObject(ulong v) => new JValue(v);
        public static explicit operator JObject(float v) => new JValue(v);
        public static explicit operator JObject(double v) => new JValue(v);
        public static explicit operator JObject(decimal v) => new JValue(v);
        public static explicit operator JObject(string v) => new JValue(v);
    }
}
