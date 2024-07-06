using System;
using System.Globalization;

namespace IntegracaoVExpensesWeb.Business.Utils
{
	public static class StringExtensions
    {
        /// <summary>
        /// Capitaliza uma string
        /// </summary>
        /// <param name="value">string a ser capitalizada</param>
        /// <returns></returns>
        public static string Capitalize(this string value)
        {
            if (String.IsNullOrEmpty(value)) return value;

            return char.ToUpper(value[0]) + value.Substring(1).ToLower();
        }

        /// <summary>
        /// Converte uma string em DateTime
        /// </summary>
        /// <param name="value">string a ser convertida</param>
        /// <param name="dateFormat">Formato de data que a string está</param>
        /// <returns>DateTime da string informada</returns>
        public static DateTime ToDateTime(this string value, string dateFormat)
        {
            DateTime data;
            DateTime.TryParseExact(value, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out data);
            return data;
        }
    }
}