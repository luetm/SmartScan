using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ShowScan
{
    public class Article
    {
        public string Code { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Stock { get; set; }
        public decimal Price { get; set; }

        public static Article ParseFromCsv(Context context, string csvLine)
        {
            try
            {
                if (csvLine == null)
                    throw new ArgumentNullException(nameof(csvLine));

                var parts = csvLine.Split(';');
                if (parts.Length != 6)
                    return null;

                decimal price;
                if (!decimal.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                    return null;

                var result = new Article
                {
                    Code = parts[0],
                    Description1 = parts[1],
                    Description2 = parts[2],
                    Stock = parts[3],
                    Price = price
                };

                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}