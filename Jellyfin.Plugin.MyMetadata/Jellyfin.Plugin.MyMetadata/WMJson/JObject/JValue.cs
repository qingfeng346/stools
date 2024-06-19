namespace WMJson {
    public class JValue : JObject {
        public static readonly JObject Null = new JValue();
        public static readonly JObject True = new JValue(true);
        public static readonly JObject False = new JValue(false);
        public JValue() : base(JObjectType.Null) { }
        public JValue(bool value) : base(value ? JObjectType.True : JObjectType.False, value) { }
        public JValue(sbyte value) : base(JObjectType.Int8, value) { }
        public JValue(byte value) : base(JObjectType.UInt8, value) { }
        public JValue(short value) : base(JObjectType.Int16, value) { }
        public JValue(ushort value) : base(JObjectType.UInt16, value) { }
        public JValue(int value) : base(JObjectType.Int32, value) { }
        public JValue(uint value) : base(JObjectType.UInt32, value) { }
        public JValue(long value) : base(JObjectType.Int64, value) { }
        public JValue(ulong value) : base(JObjectType.UInt64, value) { }
        public JValue(float value) : base(JObjectType.Float, value) { }
        public JValue(double value) : base(JObjectType.Double, value) { }
        public JValue(decimal value) : base(JObjectType.Decimal, value) { }
        public JValue(string value) : base(JObjectType.String, value) { }
    }
}
