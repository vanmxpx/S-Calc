using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace S_Calc
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Random rnd = new Random();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            EditText mainTextField = FindViewById<EditText>(Resource.Id.editMainText);
            Button butt = FindViewById<Button>(Resource.Id.button);

            butt.Click += delegate 
            {
                mainTextField.SetTextColor(new Android.Graphics.Color(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255)));
            };

        }
    }
}

