/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-05-14 16:35:12
* 版 本 号：1.0.0
* 功能说明：实现创建显示属性+函数的工厂类
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2011-12-02 17:51:00
* 版 本 号：2.3.0
* 修改内容：代码整理
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2019-11-23 17:51:00
* 版 本 号：1.0.6
* 修改内容：支持嵌套调用
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Factory
{
    public static class ObFunc
    {

        #region 基础方法

        public static ObProperty Avg(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.Avg;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.Avg,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty Count(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.Count;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.Count,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty Max(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.Max;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.Max,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty Min(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.Min;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.Min,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty Sum(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.Sum;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.Sum,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty Replace(ObProperty obProperty, string oldValue, string newValue)
        {
            /*if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.Replace;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }*/
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.Replace,
                CustomParams = new object[] { obProperty, oldValue, newValue }
            };
            return iObProperty;
        }

        public static ObProperty SubString(ObProperty obProperty, int startIndex, int length)
        {
            /*if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.SubString;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }*/
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.SubString,
                CustomParams = new object[] { obProperty, startIndex+1, length }
            };
            return iObProperty;
        }

        public static ObProperty IndexOf(ObProperty obProperty, string value)
        {
            /*if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.Replace;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }*/
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.Replace,
                CustomParams = new object[] { obProperty, value }
            };
            return iObProperty;
        }

        public static ObProperty Custom(string func, params object[] parameters)
        {
            var property = parameters.FirstOrDefault(p => p is IObProperty);
            if (property == null)
                throw new Exception("自定义函数参数中必须有一个类型为IObProperty");
            var obProperty = new ObProperty((IObProperty)property)
            {
                DbFunc = DbFunc.Custom,
                FuncName = func,
                CustomParams = parameters
            };
            return obProperty;
        }

        public static ObProperty ToInt16(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.ToInt16;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.ToInt16,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty ToInt32(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.ToInt32;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.ToInt32,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty ToInt64(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.ToInt64;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.ToInt64,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty ToSingle(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.ToSingle;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.ToSingle,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty ToDouble(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.ToDouble;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.ToDouble,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty ToDecimal(ObProperty obProperty, int length, int precision)
        {
            /*if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.ToDecimal;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }*/
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.ToDecimal,
                CustomParams = new object[] { obProperty, length, precision }
            };
            return iObProperty;
        }

        public static ObProperty ToDateTime(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.ToDateTime;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.ToDateTime,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty ToString(ObProperty obProperty)
        {
            if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.ToString;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.ToString,
                CustomParams = new object[] { obProperty }
            };
            return iObProperty;
        }

        public static ObProperty ToString(ObProperty obProperty, int type)
        {
            /*if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.ToString;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }*/
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.ToString,
                CustomParams = new object[] { obProperty, type }
            };
            return iObProperty;
        }

        public static ObProperty ToString(ObProperty obProperty, string format)
        {
            /*if (obProperty.DbFunc == DbFunc.Null)
            {
                obProperty.DbFunc = DbFunc.ToDecimal;
                obProperty.FuncBrotherCount = obProperty.Brothers.Count;
                return obProperty;
            }*/
            var iObProperty = new ObProperty(obProperty)
            {
                DbFunc = DbFunc.Format,
                CustomParams = new object[] { obProperty, format }
            };
            return iObProperty;
        }

        #endregion

        #region 扩展方法

        public static ObProperty<TSource> Top<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            iObProperty.DbFunc = DbFunc.Null;
            iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
            return new ObProperty<TSource>(source, iObProperty);
        }

        public static ObProperty<TSource> Avg<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.Avg;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.Avg,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> Count<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.Count;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.Count,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> Max<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.Max;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.Max,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> Min<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.Min;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.Min,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> Sum<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.Sum;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.Sum,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> RowNumber<TSource>(this TSource source, Func<TSource, IObSort> keySelector)
            where TSource : ObTermBase
        {
            var iObSort = keySelector(source);
            var iObProperty = new ObProperty<TSource>(source, iObSort.List.First().ObProperty)
            {
                DbFunc = DbFunc.RowNumber,
                Sort = iObSort
            };
            return iObProperty;
        }

        public static ObProperty<TSource> RowNumber<TSource>(this TSource source, Func<TSource, IObGroup> keySelector, Func<TSource, IObSort> keySelector2)
            where TSource : ObTermBase
        {
            var iObSort = keySelector2(source);
            var iObGroup = keySelector(source);
            var iObProperty = new ObProperty<TSource>(source, iObSort.List.First().ObProperty)
            {
                DbFunc = DbFunc.RowNumber,
                Sort = iObSort,
                Group = iObGroup
            };
            return iObProperty;
        }

        public static ObProperty<TSource> Custom<TSource>(this TSource source, string func, Func<TSource, object[]> keySelector)
            where TSource : ObTermBase
        {
            var parameters = keySelector(source);
            var property = parameters.FirstOrDefault(p => p is IObProperty);
            if (property == null)
                throw new Exception("自定义函数参数中必须有一个类型为IObProperty");
            var obProperty = new ObProperty<TSource>(source, (IObProperty)property)
            {
                DbFunc = DbFunc.Custom,
                FuncName = func,
                CustomParams = parameters
            };
            return obProperty;
        }

        public static ObProperty<TSource> Custom<TSource, TKey>(this TSource source, string func, Func<TSource, TKey> keySelector)
            where TSource : ObTermBase
        {
            var key = keySelector(source);
            var parameters = new List<object>();
            if (key is ObProperty[] ps)
            {
                parameters.AddRange(ps);
            }
            else
            {
                foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    parameters.Add(k);
                }
            }
            var property = parameters.FirstOrDefault(p => p is IObProperty);
            if (property == null)
                throw new Exception("自定义函数参数中必须有一个类型为IObProperty");
            var obProperty = new ObProperty<TSource>(source, (IObProperty)property)
            {
                DbFunc = DbFunc.Custom,
                FuncName = func,
                CustomParams = parameters.ToArray()
            };
            return obProperty;
        }

        public static ObProperty<TSource> Replace<TSource>(this TSource source, Func<TSource, ObProperty> keySelector, string oldValue, string newValue)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            /*if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.Replace;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }*/
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.Replace,
                CustomParams = new object[] { iObProperty, oldValue, newValue }
            };
            return obProperty;
        }

        public static ObProperty<TSource> SubString<TSource>(this TSource source, Func<TSource, ObProperty> keySelector, int startIndex, int length)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            /*if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.SubString;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }*/
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.SubString,
                CustomParams = new object[] { iObProperty, startIndex+1, length }
            };
            return obProperty;
        }

        public static ObProperty<TSource> IndexOf<TSource>(this TSource source, Func<TSource, ObProperty> keySelector, string value)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            /*if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.Replace;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }*/
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.IndexOf,
                CustomParams = new object[] { iObProperty, value }
            };
            return obProperty;
        }

        public static ObProperty<TSource> ToInt16<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.ToInt16;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.ToInt16,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> ToInt32<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.ToInt32;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.ToInt32,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> ToInt64<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.ToInt64;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.ToInt64,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> ToSingle<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.ToSingle;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.ToSingle,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> ToDouble<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.ToDouble;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.ToSingle,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> ToDecimal<TSource>(this TSource source, Func<TSource, ObProperty> keySelector, int length, int precision)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            /*if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.ToDecimal;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }*/
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.ToDecimal,
                CustomParams = new object[] { iObProperty, length, precision }
            };
            return obProperty;
        }

        public static ObProperty<TSource> ToDateTime<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.ToDateTime;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.ToDateTime,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> ToString<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.ToString;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.ToString,
                CustomParams = new object[] { iObProperty }
            };
            return obProperty;
        }

        public static ObProperty<TSource> ToString<TSource>(this TSource source, Func<TSource, ObProperty> keySelector, int style)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            /*if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.ToString;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }*/
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.ToString,
                CustomParams = new object[] { iObProperty, style }
            };
            return obProperty;
        }

        public static ObProperty<TSource> ToString<TSource>(this TSource source, Func<TSource, ObProperty> keySelector, string format)
            where TSource : ObTermBase
        {
            var iObProperty = keySelector(source);
            /*if (iObProperty.DbFunc == DbFunc.Null)
            {
                iObProperty.DbFunc = DbFunc.ToDecimal;
                iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
                return new ObProperty<TSource>(source, iObProperty);
            }*/
            var obProperty = new ObProperty<TSource>(source, iObProperty)
            {
                DbFunc = DbFunc.Format,
                CustomParams = new object[] { iObProperty, format }
            };
            return obProperty;
        }

        #endregion

    }
}
