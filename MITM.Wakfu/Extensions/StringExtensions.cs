using System;
using System.Collections.Generic;
using System.Text;

namespace MITM.Wakfu.Extensions
{
    public static class StringExtensions
    {
        public static string ConvertStringArrayToString(this byte[] array)
        {
            // Concatenate all the elements into a StringBuilder.
            StringBuilder builder = new StringBuilder();
            foreach (byte value in array)
            {
                builder.Append(value.ToString());
                builder.Append(".");
            }
            return builder.ToString();
        }
    }
}
