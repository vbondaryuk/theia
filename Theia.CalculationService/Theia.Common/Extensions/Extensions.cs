using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Theia.Common.Extensions
{
    public static class Extensions
    {
        public static List<T> OfType<T>(this IEnumerable<T> list, Type type)
        {
            return list.Where(x => x.GetType() == type).ToList();
        }

        public static bool IsList(object value)
        {
            return value is IList
                || IsGenericList(value);
        }

        public static bool IsGenericList(object value)
        {
            var type = value.GetType();
            return type.IsGenericType
                && typeof(List<>) == type.GetGenericTypeDefinition();
        }
    }
}