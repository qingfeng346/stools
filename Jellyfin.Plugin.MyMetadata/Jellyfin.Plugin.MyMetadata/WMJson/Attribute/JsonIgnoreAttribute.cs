namespace WMJson {
    using System;
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class JsonIgnoreAttribute : Attribute {
    }
}
