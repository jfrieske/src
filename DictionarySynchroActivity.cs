using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace vocab_tester
{    
    [Activity(Label = "DictionarySynchroActivity")]
    public class DictionarySynchroActivity : Activity
    {
        private string database_version_remote = "";
        private string database_file_id_remote = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_dictionary_synchro);

            FindViewById<TextView>(Resource.Id.textCurrentAction).Visibility = ViewStates.Gone;
            FindViewById<ProgressBar>(Resource.Id.progressWaiting).Visibility = ViewStates.Gone;
            FindViewById<TextView>(Resource.Id.textError).Visibility = ViewStates.Gone;
            FindViewById<Button>(Resource.Id.btnDownload).Visibility = ViewStates.Gone;

            FindViewById<Button>(Resource.Id.btnClose).Click += BtnClose_Click;
            FindViewById<Button>(Resource.Id.btnCheckVersion).Click += btnCheckVersion_Click;
            FindViewById<Button>(Resource.Id.btnDownload).Click += btnDownload_Click;
                       
        }

        private async void btnCheckVersion_Click(object sender, EventArgs e)
        {
            if (!IsOnline())
            {
                ShowError("Brak połączenia z internetem");
                return;
            }

            ShowMessageAndStart("Sprawdzam wersję bazy na serwerze");
            if (! await DownloadVersion())
            {
                return;
            }
            if (1==1)
            {
                ShowMessageAndStop(string.Format("Na serwerze jest nowa wersja numer: {0}", database_version_remote));
                FindViewById<Button>(Resource.Id.btnCheckVersion).Visibility = ViewStates.Gone;
                FindViewById<Button>(Resource.Id.btnDownload).Visibility = ViewStates.Visible;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (!IsOnline())
            {
                ShowError("Brak połączenia z internetem");
                return;
            }
        }
       
        private void BtnClose_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private bool IsOnline()
        {
            var cm = (ConnectivityManager)GetSystemService(ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }

        private async Task<Boolean> DownloadVersion()
        {
            try
            {
                string page = "http://drive.google.com/uc?export=download&id=1Em6g-Yvgb-eDboBBtK_phXe0OCqDNjZO";
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(page))
                using (HttpContent content = response.Content)
                {
                    System.IO.Stream xml_stream = await content.ReadAsStreamAsync();

                    XmlSerializer ser = new XmlSerializer(typeof(DictionaryXML_info.Info));
                    DictionaryXML_info.Info info;
                    using (XmlReader reader = XmlReader.Create(xml_stream))
                    {
                        info = (DictionaryXML_info.Info)ser.Deserialize(reader);
                    }

                    database_version_remote = info.version.number;
                    database_file_id_remote = info.file.id;
                    return true;
                }

            }
            catch (Exception e)
            {
                ShowError(e.Message);
                return false;
            }
        }

        private void ShowError(string msg)
        {
            FindViewById<TextView>(Resource.Id.textCurrentAction).Visibility = ViewStates.Gone;
            FindViewById<ProgressBar>(Resource.Id.progressWaiting).Visibility = ViewStates.Gone;
            FindViewById<TextView>(Resource.Id.textError).Visibility = ViewStates.Visible;
            FindViewById<TextView>(Resource.Id.textError).Text = msg;
        }

        private void ShowMessageAndStop(string msg)
        {
            ShowMessage(msg);
            FindViewById<ProgressBar>(Resource.Id.progressWaiting).Visibility = ViewStates.Gone;            
        }

        private void ShowMessageAndStart(string msg)
        {
            ShowMessage(msg);
            FindViewById<ProgressBar>(Resource.Id.progressWaiting).Visibility = ViewStates.Visible;
        }

        private void ShowMessage(string msg)
        {
            FindViewById<TextView>(Resource.Id.textError).Visibility = ViewStates.Gone;
            FindViewById<TextView>(Resource.Id.textCurrentAction).Visibility = ViewStates.Visible;
            FindViewById<TextView>(Resource.Id.textCurrentAction).Text = msg;
        }

    }
}