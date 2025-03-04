using System;
using System.Text;

namespace WDE.DatabaseEditors.Extensions
{
    public static class StringExtensions
    {
        public static string AddComment(this string str, string? comment)
        {
            if (string.IsNullOrEmpty(comment))
                return str;
            var existingComment = str.IndexOf(" // ", StringComparison.Ordinal);
            if (existingComment == -1)
                return str + " // " + comment;
            else
                return str.Substring(0, existingComment + 4) + comment;
        }
        
        public static string? GetComment(this string? str, bool canBeNull)
        {
            if (string.IsNullOrEmpty(str))
                return canBeNull ? null : "";

            int indexOf = str.IndexOf(" // ", StringComparison.Ordinal);
            if (indexOf == -1)
                return canBeNull ? null : "";

            return str.Substring(indexOf + 4);
        }
    }
}