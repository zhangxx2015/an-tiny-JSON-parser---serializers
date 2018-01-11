using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// An tiny JSON parser & serializers, by QQ:20437023 liaisonme@hotmail.com
/// MIT License Copyright (c) 2017 20437023@qq.com
/// 
/// 一个微型的 JSON 解析器 & 序列化器
/// </summary>
public static class Json {
    private static readonly Type StringType = string.Empty.GetType();
    private static readonly Type IntType = int.MaxValue.GetType();
    private static readonly Type SingleType = float.MaxValue.GetType();
    private static readonly Type DoubleType = double.MaxValue.GetType();
    private static readonly Type BoolType = bool.Parse("true").GetType();
    private static readonly Type StringArrayType = new string[] { }.GetType();
    private static readonly Type IntArrayType = new int[] { }.GetType();
    private static readonly Type SingleArrayType = new float[] { }.GetType();
    private static readonly Type BoolArrayType = new bool[] { }.GetType();
    private static readonly Type ArrayType = typeof(Array);
    private static readonly Dictionary<Type, Func<object, string>> Serializer = new Dictionary<Type, Func<object, string>>() {
        { StringType,obj => string.Format("\"{0}\"", obj.ToString()) },
        { IntType,obj => obj.ToString() },
        { SingleType,obj => obj.ToString() },
        { DoubleType,obj => obj.ToString() },
        { BoolType,obj => obj.ToString().ToLower() },
        { StringArrayType,arr => string.Format("[\"{0}\"]", string.Join("\",\"", arr as string[])) },
        { IntArrayType,arr => string.Format("[{0}]", string.Join(",", arr as Int32[])) },
        { SingleArrayType,arr => string.Format("[{0}]", string.Join(",", arr as float[])) },
        { BoolArrayType,arr => string.Format("[{0}]", string.Join(",", arr as bool[]).ToLower()) },
    };
    private static readonly Type[] BaseTypes = Serializer.Keys.ToArray();
    private const System.Reflection.BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly;
    private static readonly char[] AlphaTab = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    /// <summary>
    /// serializeobject
    /// </summary>
    public static string ToJson(object obj) {
        if (null == obj) return "null";
        var type = obj.GetType();
        if (BaseTypes.Contains(type)) return Serializer[type](obj);
        if (type.BaseType != ArrayType) {
            var membs = type.GetProperties(BindingFlags);
            var props = new object[membs.Length];
            for (var i = 0; i < membs.Length; i++) 
                  props[i] = string.Format("\"{0}\":{1}",membs[i].Name,ToJson(membs[i].GetValue(obj)));
            return string.Format("{{{0}}}",string.Join(",",props));
        }
        var items = obj as Array;
        var jsons = new string[items.Length];
        for (var i = 0; i < items.Length; i++)
            jsons[i] = ToJson(items.GetValue(i));
        return string.Format("[{0}]", string.Join(",", jsons));
    }
    /// <summary>
    /// deserializeobject
    /// </summary>
    public static object FromJson(string json) {
        var chars = json.ToList();
        return deserializeObject(ref chars);
    }
    private static object deserializeObject(ref List<char> chars) {
        if (null == chars) return null;
        var json = new string(chars.ToArray()).Trim();
        if (string.IsNullOrEmpty(json)) return null;
        if (chars.Count < 1) return null;
        var jsonLength = json.Length;
        if (jsonLength >= 4) {
            if (0 == json.IndexOf("null", StringComparison.Ordinal)) {
                chars.RemoveRange(0,4);
                return null;
            }
            if (0 == json.IndexOf("true", StringComparison.Ordinal)) {
                chars.RemoveRange(0, 4);
                return true;
            }
        }
        if (jsonLength >= 5)
            if (0 == json.IndexOf("false", StringComparison.Ordinal)) {
                chars.RemoveRange(0, 5);
                return false;
            }
        var ch = chars.FirstOrDefault();
        if ('[' == ch) {
            chars.RemoveAt(0);
            var array = new List<object>();
            while (chars.Count > 0) {
                var val = deserializeObject(ref chars);
                array.Add(val);
                if (',' == chars[0]) {
                    chars.RemoveAt(0);
                    continue;
                }
                if (']' != chars[0]) continue;
                chars.RemoveAt(0);
                break;
            }
            return array;
        }
        if ('{' == ch) {
            chars.RemoveAt(0);
            var dict = new Dictionary<string, object>();
            while (chars.Count > 0) {
                var key = deserializeObject(ref chars);
                //if (key.GetType() != typeof(string)) throw new Exception("illegal key");
                if (key.GetType() != StringType) throw new Exception("illegal key");
                if (':' == chars[0]) 
                    chars.RemoveAt(0);
                var val = deserializeObject(ref chars);
                dict.Add(key.ToString(), val);
                if (',' == chars[0]) {
                    chars.RemoveAt(0);
                    continue;
                }
                if ('}' != chars[0]) continue;
                chars.RemoveAt(0);
                return dict;
            }
        }
        if (AlphaTab.Contains(ch)) {
            var nums = string.Empty;
            while (chars.Count > 0) {
                if (AlphaTab.Contains(chars[0]) || '.' == chars[0]) {
                    nums += chars[0];
                    chars.RemoveAt(0);
                    continue;
                }
                break;
            }
            var decimalPoints = nums.Count(c => '.' == c);
            if (0 == decimalPoints) return int.Parse(nums);
            if (1 == decimalPoints) return float.Parse(nums);
            throw new Exception("illegal digit");
        }
        if ('"' != ch) return null;
        chars.RemoveAt(0);
        var strs = string.Empty;
        if (chars.Count <= 0) return strs;
        do {
            if ('"' == chars[0]) {
                if ('\\' == strs.LastOrDefault()) {
                    strs += chars[0];
                    chars.RemoveAt(0);
                    continue;
                }
                chars.RemoveAt(0);
                break;
            }
            strs += chars[0];
            chars.RemoveAt(0);
        } while (chars.Count > 0);
        return strs;
    }
}
