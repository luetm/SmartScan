using System;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ZXing.Mobile;

namespace ShowScan
{
    [Activity(Label = "ShowScan", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            MobileBarcodeScanner.Initialize(Application);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.ScanButton);

            button.Click += async (s, e) =>
            {
                FindViewById<EditText>(Resource.Id.ProductName).Text = string.Empty;
                FindViewById<EditText>(Resource.Id.Description).Text = string.Empty;
                FindViewById<EditText>(Resource.Id.Stock).Text = string.Empty;
                FindViewById<EditText>(Resource.Id.Price).Text = string.Empty;

                var scanner = new MobileBarcodeScanner();
                var result = await scanner.Scan();
                var code = result.Text;

                var path = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDocuments, "Articles.csv");
                var csvLines = File.ReadAllLines(path);
                var article = csvLines
                    .Select(l => Article.ParseFromCsv(ApplicationContext, l))
                    .Where(a => a != null)
                    .FirstOrDefault(x => x.Code == code);

                if (article == null)
                {
                    var toast = Toast.MakeText(ApplicationContext, "No article found.", ToastLength.Long);
                    toast.Show();
                    
                    return;
                }

                var notification = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
                Ringtone r = RingtoneManager.GetRingtone(ApplicationContext, notification);
                r.Play();

                FindViewById<EditText>(Resource.Id.ProductName).Text = article.Name;
                FindViewById<EditText>(Resource.Id.Description).Text = article.Description;
                FindViewById<EditText>(Resource.Id.Stock).Text = article.Stock;
                FindViewById<EditText>(Resource.Id.Price).Text = article.Price.ToString("N");
            };
        }
    }
}

