using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using DynamicExpressionParser = System.Linq.Dynamic.Core.DynamicExpressionParser;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace JBBS.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 手机号码校验
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns></returns>
        public static bool IsMobile(this string mobile)
        {
            const string pattern = @"^1[3-9]\d{9}$";
            return Regex.IsMatch(mobile, pattern);
        }

        public static bool IsNullOrWhiteSpace(this string? input)
        {
            return string.IsNullOrWhiteSpace(input);
        }

        public static bool IsMatch(this string? input, string pattern)
        {
            return input != null && Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// 移除字符串中的空字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveWhitespace(this string input)
        {
            return Regex.Replace(input, @"\s+", "");
        }

        /// <summary>
        /// 字符串切片
        /// </summary>
        /// <param name="input"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Slice(this string input, int startIndex, int length)
        {
            var n = input.Length - startIndex;
            if (n >= length) return input.Substring(startIndex, length);
            if (n > 0) return input.Substring(startIndex, n);
            return string.Empty;
        }

        public static long? ToNullable(this long number)
        {
            return number > 0 ? number : null;
        }

        /// <summary>
        /// 单词变成单数形式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Single(this string name)
        {
            var plural1 = new Regex("(?<keep>[^aeiou])ies$");
            var plural2 = new Regex("(?<keep>[aeiou]y)s$");
            var plural3 = new Regex("(?<keep>[sxzh])es$");
            var plural4 = new Regex("(?<keep>[^sxzhyu])s$");

            if (plural1.IsMatch(name)) return plural1.Replace(name, "${keep}y");
            if (plural2.IsMatch(name)) return plural2.Replace(name, "${keep}");
            if (plural3.IsMatch(name)) return plural3.Replace(name, "${keep}");
            if (plural4.IsMatch(name)) return plural4.Replace(name, "${keep}");

            return name;
        }

        /// <summary>
        /// 单词变成复数形式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Plural(this string name)
        {
            var plural1 = new Regex("(?<keep>[^aeiou])y$");
            var plural2 = new Regex("(?<keep>[aeiou]y)$");
            var plural3 = new Regex("(?<keep>[sxzh])$");
            var plural4 = new Regex("(?<keep>[^sxzhy])$");

            if (plural1.IsMatch(name)) return plural1.Replace(name, "${keep}ies");
            if (plural2.IsMatch(name)) return plural2.Replace(name, "${keep}s");
            if (plural3.IsMatch(name)) return plural3.Replace(name, "${keep}es");
            if (plural4.IsMatch(name)) return plural4.Replace(name, "${keep}s");

            return name;
        }

        public static int ParseInt(this string? input, int defaultValue = 0)
        {
            return int.TryParse(input, out var value) ? value : defaultValue;
        }

        public static long ParseLong(this string? input, long defaultValue = 0)
        {
            return long.TryParse(input, out var value) ? value : defaultValue;
        }

        public static long[] ParseLongs(this string input)
        {
            return input.RemoveWhitespace().Split(",").Select(x => long.Parse(x!)).ToArray();
        }

        /// <summary>
        /// 将数字转换为大写字母，数字0~25，依次对应A~Z
        /// </summary>
        /// <param name="number">0~25</param>
        /// <returns></returns>
        public static char ToUpperChar(this int number)
        {
            return (char)(65 + number);
        }

        /// <summary>
        /// 将数字转换为小写字母，数字0~25，依次对应a~z
        /// </summary>
        /// <param name="number">0~25</param>
        /// <returns></returns>
        public static char ToLowerChar(this int number)
        {
            return (char)(97 + number);
        }

        /// <summary>
        /// 反序列化难度等级数量字典
        /// </summary>
        /// <param name="quantitiesInRankString"></param>
        /// <returns></returns>
        public static Dictionary<int, int> ToRankQuantityDictionary(this string quantitiesInRankString)
        {
            var result = new Dictionary<int, int>();
            foreach (Match match in Regex.Matches(quantitiesInRankString.RemoveWhitespace(), "[0-9]+:[0-9]+"))
            {
                var temp = match.Value.Split(":").Select(int.Parse).ToArray();
                result[temp[0]] = temp[1];
            }

            return result;
        }

        /// <summary>
        /// 身份证号校验
        /// </summary>
        /// <param name="code"></param>
        /// <param name="error"></param>
        /// <param name="minAge"></param>
        /// <param name="maxAge"></param>
        /// <returns></returns>
        public static bool VerifyIdCardCode(this string code, out string error, int minAge = 0, int maxAge = 150)
        {
            error = "";

            string?[] provinces =
            {
                null, null, null, null, null, null, null, null, null, null, null, "北京", "天津", "河北", "山西", "内蒙古", null, null, null, null, null, "辽宁", "吉林", "黑龙江", null, null, null, null, null, null, null, "上海", "江苏", "浙江", "安微", "福建", "江西", "山东", null, null, null, "河南", "湖北", "湖南", "广东", "广西", "海南", null, null, null, "重庆", "四川", "贵州", "云南", "西藏", null, null, null, null, null, null, "陕西", "甘肃", "青海", "宁夏", "新疆", null, null, null, null, null, "台湾", null, null, null, null, null, null, null, null, null, "香港",
                "澳门", null, null, null, null, null, null, null, null, "国外"
            };
            var sum = 0;
            code = code.ToLower();
            if (!Regex.IsMatch(code, @"^\d{17}(\d|x)$"))
            {
                error = "长度错误";
                return false;
            }

            if (provinces[int.Parse(code.Substring(0, 2))] == null)
            {
                error = "地区错误";
                return false;
            }

            if (!DateTime.TryParse(code.Substring(6, 4) + "-" + code.Substring(10, 2) + "-" + code.Substring(12, 2), out var birthdate))
            {
                error = "生日错误";
                return false;
            }

            var age = birthdate.GetAgeByBirthdate();
            if (age >= maxAge)
            {
                error = $"年龄已满{maxAge}周岁";
                return false;
            }

            if (age < minAge)
            {
                error = $"年龄未满{minAge}周岁";
                return false;
            }

            code = code.Replace("x", "a");
            for (var i = 17; i >= 0; i--)
            {
                sum += (int)Math.Pow(2, i) % 11 * int.Parse(code[17 - i].ToString(), NumberStyles.HexNumber);
            }

            if (sum % 11 != 1)
            {
                error = "校验码错误";
                return false;
            }

            return true;
        }

        public static string ToLowerCase(this string str)
        {
            return string.Join("_", Regex.Matches(str, "(^[a-z0-9]+|[A-Z]+(?![a-z0-9])|[A-Z][a-z0-9]+)").Select(m => m.Value.ToLower()));
        }

        public static string ToUpperCase(this string str)
        {
            return string.Join("_", Regex.Matches(str, "(^[a-z0-9]+|[A-Z]+(?![a-z0-9])|[A-Z][a-z0-9]+)").Select(m => m.Value.ToUpper()));
        }

        public static string ToCamelCase(this string str)
        {
            return Regex.Replace(str, @"([A-Z])([A-Z]+|[a-z0-9_]+)($|[A-Z]\w*)", m => $"{m.Groups[1].Value.ToLower()}{m.Groups[2].Value.ToLower()}{m.Groups[3].Value}");
        }

        public static string Or(this string? input, string optional)
        {
            return string.IsNullOrWhiteSpace(input) ? optional : input;
        }

        /// <summary>
        /// 字符串数据替换
        /// </summary>
        /// <param name="input">包含待替换字符的字符串模板（例如："{arg1.Name},{arg2.Value}"）</param>
        /// <param name="data">用于替换字符串中的待替换字符的数据（例如：{["arg1"]=new{Name="Hello"},["arg2"]=new{Value=6}}）</param>
        /// <returns></returns>
        public static string Replace(this string input, IReadOnlyDictionary<string, object> data)
        {
            var items = data.Select(x => new { p = Expression.Parameter(x.Value.GetType(), x.Key), a = x.Value }).ToArray();
            var parameters = items.Select(x => x.p).ToArray();
            var args = items.Select(x => x.a).ToArray();

            return Regex.Replace(input, @"{(?<exp>[^}]+)}", match =>
            {
                var e = DynamicExpressionParser.ParseLambda(parameters, null, match.Groups["exp"].Value);
                return e.Compile().DynamicInvoke(args)?.ToString() ?? "";
            });
        }

        public static bool TryReplace(this string input, IReadOnlyDictionary<string, object> data, out string result)
        {
            try
            {
                result = input.Replace(data);
                return true;
            }
            catch (Exception e)
            {
                result = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 修剪对象所有字符串类型（包括string的IList类型）的属性
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T TrimProperties<T>(this T obj)
        {
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(typeof(T)))
            {
                if (pd.PropertyType == typeof(string) && !pd.IsReadOnly)
                {
                    pd.SetValue(obj, (pd.GetValue(obj) as string)?.Trim() ?? "");
                }
                else
                {
                    var list = pd.GetValue(obj) as IList<string>;
                    if (list is null) continue;
                    for (var i = 0; i < list.Count; i++)
                    {
                        list[i] = list[i].Trim();
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// 删除字符串中所有html标签
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveHtmlTag(this string input)
        {
            //删除脚本   
            input = Regex.Replace(input, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            input = Regex.Replace(input, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"-->", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"<!--.*", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"<a\s*[^>]*>", "", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"</a>", "", RegexOptions.IgnoreCase);
            return input;
        }

        /// <summary>
        /// 计算公式字符串的值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Object? StringCompute(this string str) {
            DataTable dt = new DataTable();
            try
            {
                var result = dt.Compute(str, null);
                return result;
            }
            catch {
                return null;
            }
        }
    }
}
