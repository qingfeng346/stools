namespace WMJson {
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    public class JsonDeserializer {
        const char END_CHAR = (char)0;
        const char QUOTES = '"';            //引号
        const char LEFT_BRACE = '{';        //{
        const char RIGHT_BRACE = '}';       //}
        const char LEFT_BRACKET = '[';      //[
        const char RIGHT_BRACKET = ']';     //]
        const char COMMA = ',';             //,
        const string TRUE = "true";
        const string FALSE = "false";
        const string NULL = "null";

        private string m_Buffer;
        private StringBuilder m_Builder;
        private int m_Length;
        private int m_Index;
        static bool IsWhiteSpace(char c) {
            switch (c) {
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    return true;
                default:
                    return false;
            }
        }
        static bool IsWordBreak(char c) {
            switch (c) {
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                case ',':
                case ':':
                case '{':
                case '}':
                case '[':
                case ']':
                    case '\"':
                    return true;
                default:
                    return false;
            }
        }
        char EatWhiteSpace {
            get {
                char ch;
                while (IsWhiteSpace(ch = m_Buffer[m_Index])) {
                    if (++m_Index == m_Length) {
                        return END_CHAR;
                    }
                }
                ++m_Index;
                return ch;
            }
        }
        string NextWord {
            get {
                var start = m_Index;
                while (!IsWordBreak(m_Buffer[m_Index])) {
                    ++m_Index;
                    if (m_Index == m_Length) {
                        return m_Buffer.Substring(start, m_Index - start);
                    }
                }
                return m_Buffer.Substring(start, m_Index - start);
            }
        }
        string NextString {
            get {
                m_Builder.Length = 0;
                while (true) {
                    var ch = m_Buffer[m_Index++];
                    if (ch == QUOTES) {
                        return m_Builder.ToString();
                    }
                    switch (ch) {
                        case '\\':
                            ch = m_Buffer[m_Index++];
                            switch (ch) {
                                case '\'': m_Builder.Append('\''); break;
                                case '\"': m_Builder.Append('\"'); break;
                                case '\\': m_Builder.Append('\\'); break;
                                case 'a': m_Builder.Append('\a'); break;
                                case 'b': m_Builder.Append('\b'); break;
                                case 'f': m_Builder.Append('\f'); break;
                                case 'n': m_Builder.Append('\n'); break;
                                case 'r': m_Builder.Append('\r'); break;
                                case 't': m_Builder.Append('\t'); break;
                                case 'v': m_Builder.Append('\v'); break;
                                case '0': m_Builder.Append('\0'); break;
                                case '/': m_Builder.Append("/"); break;
                                case 'u': {
                                    var hex = new StringBuilder();
                                    for (int i = 0; i < 4; i++) {
                                        hex.Append(m_Buffer[m_Index++]);
                                    }
                                    m_Builder.Append((char)Convert.ToUInt16(hex.ToString(), 16));
                                    break;
                                }
                            }
                            break;
                        default:
                            m_Builder.Append(ch);
                            break;
                    }
                }
            }
        }
        public object Parse(string buffer, Type type, JsonDeserializeSetting setting) {
            this.m_Buffer = buffer;
            this.m_Length = buffer.Length;
            this.m_Builder = new StringBuilder();
            this.m_Index = 0;
            return Read(JsonUtils.GetContract(type), setting);
        }
        string ReadWord() {
            var ch = EatWhiteSpace;
            switch (ch) {
                case QUOTES:
                    return NextString;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    --m_Index;
                    return NextWord;
                case LEFT_BRACE:
                case LEFT_BRACKET:
                    throw new Exception("起始char错误");
                default:
                    --m_Index;
                    return NextWord;
            }
        }
        object Read(IJsonContract contract, JsonDeserializeSetting setting) {
            if (contract is JsonPrimitiveContract) {
                return contract.Parse(ReadWord());
            } else if (contract is JsonJObjectContract) {
                return ReadJObject(setting);
            } else if (contract is JsonEnumerableContract enumerableContract) {
                return ReadEnumerable(enumerableContract, setting);
            } else if (contract is JsonArrayContract arrayContract) {
                if (arrayContract.Rank > 1) {
                    return ReadMultidimensionalArray(arrayContract, setting, arrayContract.Rank);
                } else {
                    return ReadArray(arrayContract, setting);
                }
            } else if (contract is JsonMapContract mapContract) {
                return ReadMap(mapContract, setting);
            } else {
                return ReadObject((JsonObjectContract)contract, setting);
            }
        }
        bool CheckNullOrChar(string message, char c) {
            var startChat = EatWhiteSpace;
            if (startChat == 'n') {
                --m_Index;
                if (NextWord == NULL) return true;
                throw new Exception($"预期是{message},起始char错误 : {startChat}");
            } else if (startChat != c)
                throw new Exception($"预期是{message},起始char错误 : {startChat}");
            return false;
        }
        object ReadObject(JsonObjectContract contract, JsonDeserializeSetting setting) {
            if (CheckNullOrChar("Object", LEFT_BRACE)) return null;
            var instance = contract.Create();
            while (true) {
                switch (EatWhiteSpace) {
                    case RIGHT_BRACE: return instance;
                    case COMMA: continue;
                    case END_CHAR:
                        throw new Exception("Json解析, 未找到 map 结尾 [}]");
                    case QUOTES: {
                        var key = NextString;
                        if (EatWhiteSpace != ':') {
                            throw new Exception($"Json解析, key值后必须跟 [:] 赋值 : {key} {m_Index}");
                        }
                        if (contract.TryGetVariable(key, out var variable) && variable.CanWrite) {
                            variable.SetValue(instance, Read(variable.contract, setting));
                        } else {
                            ReadJObject(setting);
                        }
                        break;
                    }
                    default: {
                        throw new Exception("Json解析, key值 未知符号 ");
                    }
                }
            }
        }
        object ReadMap(JsonMapContract contract, JsonDeserializeSetting setting) {
            if (CheckNullOrChar("Map", LEFT_BRACE)) return null;
            IDictionary instance = (IDictionary)contract.Create();
            while (true) {
                switch (EatWhiteSpace) {
                    case RIGHT_BRACE: return instance;
                    case COMMA: continue;
                    case END_CHAR:
                        throw new Exception("Json解析, 未找到 map 结尾 [}]");
                    case QUOTES: {
                        var key = NextString;
                        if (EatWhiteSpace != ':') {
                            throw new Exception($"Json解析, key值后必须跟 [:] 赋值 : {key} {m_Index}");
                        }
                        instance[contract.keyContract.Parse(key)] = Read(contract.valueContract, setting);
                        break;
                    }
                    default: {
                        throw new Exception("Json解析, key值 未知符号 ");
                    }
                }
            }
        }
        object ReadEnumerable(JsonEnumerableContract contract, JsonDeserializeSetting setting) {
            if (CheckNullOrChar("Enumerable", LEFT_BRACKET)) return null;
            IList instance = (IList)contract.Create();
            var valueContract = contract.valueContract;
            while (true) {
                switch (EatWhiteSpace) {
                    case RIGHT_BRACKET: return instance;
                    case COMMA: continue;
                    case END_CHAR:
                        throw new Exception("Json解析, 未找到array结尾 ]");
                    default: {
                        --m_Index;
                        instance.Add(Read(valueContract, setting));
                        continue;
                    }
                }
            }
        }
        object ReadArray(JsonArrayContract contract, JsonDeserializeSetting setting) {
            if (CheckNullOrChar("Array", LEFT_BRACKET)) return null;
            var instance = new List<object>();
            var valueContract = contract.valueContract;
            while (true) {
                switch (EatWhiteSpace) {
                    case RIGHT_BRACKET: {
                        var result = Array.CreateInstance(contract.elementType, instance.Count);
                        ((IList)instance).CopyTo(result, 0);
                        return result;
                    }
                    case COMMA: continue;
                    case END_CHAR:
                        throw new Exception("Json解析, 未找到array结尾 ]");
                    default: {
                        --m_Index;
                        instance.Add(Read(valueContract, setting));
                        continue;
                    }
                }
            }
        }
        object ReadMultidimensionalArray(JsonArrayContract contract, JsonDeserializeSetting setting, int rank) {
            if (CheckNullOrChar("MultidimensionalArray", LEFT_BRACKET)) return null;
            var instance = new List<object>();
            while (true) {
                switch (EatWhiteSpace) {
                    case RIGHT_BRACKET: return CreateMultidimensionalArray(contract.elementType, instance, rank);
                    case COMMA: continue;
                    case END_CHAR:
                        throw new Exception("Json解析, 未找到array结尾 ]");
                    default: {
                        --m_Index;
                        instance.Add(ReadMultidimensionalArray_impl(contract, setting, rank - 1));
                        continue;
                    }
                }
            }
        }
        object ReadMultidimensionalArray_impl(JsonArrayContract contract, JsonDeserializeSetting setting, int rank) {
            if (CheckNullOrChar("MultidimensionalArray", LEFT_BRACKET)) return null;
            var instance = new List<object>();
            while (true) {
                switch (EatWhiteSpace) {
                    case RIGHT_BRACKET: return instance;
                    case COMMA: continue;
                    case END_CHAR:
                        throw new Exception("Json解析, 未找到array结尾 ]");
                    default: {
                        --m_Index;
                        if (rank > 1) {
                            instance.Add(ReadMultidimensionalArray_impl(contract, setting, rank - 1));
                        } else {
                            instance.Add(Read(contract.valueContract, setting));
                        }
                        continue;
                    }
                }
            }
        }
        object CreateMultidimensionalArray(Type elementType, List<object> list, int rank) {
            var indexs = new int[rank];
            object v = list;
            for (var i = 0; i < rank; ++i) {
                if (v is IList l && l.Count > 0) {
                    indexs[i] = l.Count;
                    v = l[0];
                } else {
                    indexs[i] = 0;
                }
            }
            var array = Array.CreateInstance(elementType, indexs);
            CopyFromJaggedToMultidimensionalArray(list, array, Array.Empty<int>());
            return array;
        }
        void CopyFromJaggedToMultidimensionalArray(IList values, Array multidimensionalArray, int[] indices) {
            int dimension = indices.Length;
            if (dimension == multidimensionalArray.Rank) {
                multidimensionalArray.SetValue(JaggedArrayGetValue(values, indices), indices);
                return;
            }
            int dimensionLength = multidimensionalArray.GetLength(dimension);
            IList list = (IList)JaggedArrayGetValue(values, indices);
            int currentValuesLength = list.Count;
            if (currentValuesLength != dimensionLength) {
                throw new Exception("Cannot deserialize non-cubical array as multidimensional array.");
            }
            int[] newIndices = new int[dimension + 1];
            for (int i = 0; i < dimension; i++) {
                newIndices[i] = indices[i];
            }
            for (int i = 0; i < multidimensionalArray.GetLength(dimension); i++) {
                newIndices[dimension] = i;
                CopyFromJaggedToMultidimensionalArray(values, multidimensionalArray, newIndices);
            }
        }
        private static object JaggedArrayGetValue(IList values, int[] indices) {
            IList currentList = values;
            for (int i = 0; i < indices.Length; i++) {
                int index = indices[i];
                if (i == indices.Length - 1) {
                    return currentList[index];
                } else {
                    currentList = (IList)currentList[index];
                }
            }
            return currentList;
        }
        JObject ReadJObject(JsonDeserializeSetting setting) {
            var ch = EatWhiteSpace;
            switch (ch) {
                case QUOTES:
                    return new JValue(NextString);
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    --m_Index;
                    return ReadJNumber(setting);
                case LEFT_BRACE:
                    return ReadJMap(setting);
                case LEFT_BRACKET:
                    return ReadJArray(setting);
                default:
                    --m_Index;
                    var word = NextWord;
                    switch (word) {
                        case TRUE: return JValue.True;
                        case FALSE: return JValue.False;
                        case NULL: return JValue.Null;
                        default: throw new Exception("Json解析, 未知标识符 : " + word);
                    }
            }
        }
        JObject ReadJNumber(JsonDeserializeSetting setting) {
            var number = NextWord;
            if (number.IndexOf('.') >= 0) {
                return new JValue(double.Parse(number));
            } else if (long.TryParse(number, out var l)){
                return new JValue(l);
            } else {
                return new JValue(ulong.Parse(number));
            }
        }
        JMap ReadJMap(JsonDeserializeSetting setting) {
            var map = new JMap();
            while (true) {
                var ch = EatWhiteSpace;
                switch (ch) {
                    case RIGHT_BRACE: return map;
                    case COMMA: continue;
                    case END_CHAR:
                        throw new Exception("Json解析, 未找到 map 结尾 [}]");
                    case QUOTES: {
                        var key = NextString;
                        if (EatWhiteSpace != ':') {
                            throw new Exception("Json解析, key值后必须跟 [:] 赋值");
                        }
                        map[key] = ReadJObject(setting);
                        break;
                    }
                    default: {
                        throw new Exception("Json解析, key值 未知符号 : " + ch);
                    }
                }
            }
        }
        JArray ReadJArray(JsonDeserializeSetting setting) {
            var array = new JArray();
            while (true) {
                var ch = EatWhiteSpace;
                switch (ch) {
                    case RIGHT_BRACKET: return array;
                    case COMMA: continue;
                    case END_CHAR:
                        throw new Exception("Json解析, 未找到array结尾 ]");
                    default: {
                        --m_Index;
                        array.Add(ReadJObject(setting));
                        continue;
                    }
                }
            }
        }
    }
}
