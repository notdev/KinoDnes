using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace KinoDnesApi
{
    public static class StringNormalizer
    {
        private static string RemoveDiacritics(string originalString)
        {
            var normalizedString = originalString.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        // Remove whitespace and diacritics from string
        public static string StandardizeString(string str)
        {
            var standardizedName = str.ToLower();
            standardizedName = Regex.Replace(standardizedName, @"\s+", "");
            standardizedName = RemoveDiacritics(standardizedName);

            return standardizedName;
        }
    }
}