using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.Lang;
using ZXing.Mobile;
using Environment = Android.OS.Environment;
using Exception = System.Exception;

namespace ShowScan
{
    [Activity(Label = "ShowScan", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private MediaPlayer _successSound;
        private MediaPlayer _errorSound;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            MobileBarcodeScanner.Initialize(Application);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);


            // Create Sounds
            _successSound = MediaPlayer.Create(this, Resource.Raw.success);
            _errorSound = MediaPlayer.Create(this, Resource.Raw.error);

            // Hook up scan button
            Button button = FindViewById<Button>(Resource.Id.ScanButton);
            button.Click += OnScan;
        }

        private async void OnScan(object sender, EventArgs e)
        {
            try
            {
                FindViewById<EditText>(Resource.Id.ProductName).Text = string.Empty;
                FindViewById<EditText>(Resource.Id.Description).Text = string.Empty;
                FindViewById<EditText>(Resource.Id.Stock).Text = string.Empty;
                FindViewById<EditText>(Resource.Id.Price).Text = string.Empty;

                var scanner = new MobileBarcodeScanner();
                var result = await scanner.Scan();
                var code = result.Text;

                var path = Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath, Environment.DirectoryDocuments, "Articles.csv");
                var csvLines = File.ReadAllLines(path);
                var article = csvLines
                    .Select(l => Article.ParseFromCsv(this, l))
                    .Where(a => a != null)
                    .FirstOrDefault(x => x.Code == code);

                if (article == null)
                {
                    var toast = Toast.MakeText(this, "No article found.", ToastLength.Long);
                    toast.Show();
                    _errorSound.Start();
                    
                    return;
                }


                _successSound.Start();
                
                FindViewById<EditText>(Resource.Id.ProductName).Text = article.Name;
                FindViewById<EditText>(Resource.Id.Description).Text = article.Description;
                FindViewById<EditText>(Resource.Id.Stock).Text = article.Stock;
                FindViewById<EditText>(Resource.Id.Price).Text = article.Price.ToString("N");
            }
            catch (Exception err)
            {
                var toast = Toast.MakeText(ApplicationContext, $"{err.GetType().Name}: {err.Message}", ToastLength.Long);
                toast.Show();
            }
        }
    }
}

