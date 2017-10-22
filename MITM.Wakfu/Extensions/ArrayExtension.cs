using Rebirth.Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MITM.Wakfu.Extensions
{
    public static class ArrayExtension
    {
        public static T[] AddRange<T>(this T[] array1, T[] array2)
        {
            int array1OriginalLength = array1.Length;
            Array.Resize<T>(ref array1, array1OriginalLength + array2.Length);
            if (array2.Length > 0)
                Array.Copy(array2, 0, array1, array1OriginalLength, array2.Length);
            return array1;
        }
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                System.Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                System.Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
        public static string ToString<T>(this T[] source, string separator) where T : IConvertible
        {
            return string.Join(separator, source);
        }

        public static string GetDatas(this byte[] buff, bool isHexa)
        {
            if (isHexa)
                return BitConverter.ToString(buff).Replace("-", " ");
            else
                return buff.ToCSV(",");
        }
    }
}
