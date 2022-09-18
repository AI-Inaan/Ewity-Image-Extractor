using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace EwityToExcel
{
    internal class Program
    {

        #region json class

     
        public class Base
        {
            public double price { get; set; }
        }

        public class Brand
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class Category
        {
            public int id { get; set; }
            public string name { get; set; }
            public object parent { get; set; }
        }

        public class Config
        {
            public bool is_default { get; set; }
        }

        public class Datum
        {
            public int id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public Brand brand { get; set; }
            public Category category { get; set; }
            public bool is_serialized { get; set; }
            public bool is_inventory_tracked { get; set; }
            public List<object> attribute_types { get; set; }
            public List<Variant> variants { get; set; }
            public List<Taxis> taxes { get; set; }
            public int updated_at { get; set; }
        }

        public class Images
        {
            public int id { get; set; }
            public string url { get; set; }
            public object width { get; set; }
            public object height { get; set; }
            public string placeholder { get; set; }
        }

        public class Pricing
        {
            public Base @base { get; set; }
        }

        public class Root
        {
            public List<Datum> data { get; set; }
        }

        public class Scales
        {
            public int pcs { get; set; }
        }

        public class Stock
        {
            public int count { get; set; }
            public int updated_at { get; set; }
        }

        public class Taxis
        {
            public int id { get; set; }
            public string code { get; set; }
            public string name { get; set; }
            public double rate { get; set; }
            public Config config { get; set; }
        }

        public class Units
        {
            public Scales scales { get; set; }
            public string @default { get; set; }
            public string @base { get; set; }
        }

        public class Variant
        {
            public int id { get; set; }
            public string sku { get; set; }
            public object barcode { get; set; }
            public object attributes { get; set; }
            public Pricing pricing { get; set; }
            public Units units { get; set; }
            public Stock stock { get; set; }
            public List<Images> images { get; set; }
            public int updated_at { get; set; }
        }
        #endregion
        
        public class Items
        {
            public string name;
            public string img;
            public string count;
            public string price;


            public static Items FromCsv(string csvLine)
            {
                string[] values = csvLine.Split(',');
                Items dailyValues = new Items();
                dailyValues.name = Convert.ToString(values[0]);
                dailyValues.price = Convert.ToString(values[1]);
                dailyValues.count = Convert.ToString(values[2]);
                dailyValues.img = Convert.ToString(values[3]);
                return dailyValues;
            }
        }

        static void Main(string[] args)
        {
            ReadJson();
            ReadCSV();
        }


        /// <summary>
        /// read csv file and download each image in the sheet
        /// </summary>
        public static void ReadCSV()
        {
            List<Items> values = File.ReadAllLines("data.csv")
                                          .Skip(1)
                                          .Select(v => Items.FromCsv(v))
                                          .ToList();

            foreach (var item in values)
            {
                Console.WriteLine("Downloading " + item.name);
                string filename = Path.Combine("img",item.img.Split('/').Last().Replace(".png", "").Replace(".jpg","").Replace(".", "")) + ".png";
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(item.img);
                System.Drawing.Bitmap bitmap; bitmap = new System.Drawing.Bitmap(stream);

                if (bitmap != null)
                {
                    bitmap.Save(filename, ImageFormat.Png);
                }

                item.img = filename;
                stream.Flush();
                stream.Close();
                client.Dispose();
            }

            SaveToCsv(values, "_data.csv");
        }

        /// <summary>
        /// read output from ewity and then convert it to a simple CSV file containing all the url for the images
        /// </summary>
        public static void ReadJson()
        {
            Console.WriteLine("Reading Ewity Json");
            string json = System.IO.File.ReadAllText("ewitydata.json");
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);

            List<Items> items = new List<Items>();

            foreach (var item in myDeserializedClass.data)
            {
                string name = item.name;
                string img;
                if (item.variants[0].images.Count != 0)
                {
                    img = item.variants[0].images[0].url;
                }
                else
                {
                    img = "null";
                }
                var count = item.variants[0].stock.count;
                var price = GetPrice(item.variants[0].pricing.@base.price);

                items.Add(new Items
                {
                    name = name.Replace(",", ";"),
                    img = img,
                    count = count.ToString(),
                    price = price.ToString()
                });
            }

            SaveToCsv(items, "data.csv");
        }

        /// <summary>
        /// export data extracted from ewity to csv
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="path"></param>
        public static void SaveToCsv(List<Items> reportData, string path)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < reportData.Count; i++)
            {
                sb.Append(reportData[i].name + ',');
                sb.Append(reportData[i].price + ',');
                sb.Append(reportData[i].count + ',');
                sb.Append(reportData[i].img + ',');

                //Append new line character.
                sb.Append("\r\n");

            }

            File.WriteAllText(path, sb.ToString());
        }

        /// <summary>
        /// calculate final price of 6% gst
        /// </summary>
        /// <param name="_price">uncalculated price</param>
        /// <returns></returns>
        public static double GetPrice(double _price)
        {
            var price = _price;

            price = price * 0.06;

            price = price + _price;

            return Math.Round(price, 0, MidpointRounding.AwayFromZero);
        }
    }
}
