using Android.App;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace vocab_tester
{
    [Activity(Label = "DictionarySynchroActivity")]
    public class DictionarySynchroActivity : Activity
    {
        private DictionaryXMLHelper.DB_info db_remote_info;
        private string db_local_info = "";

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
            try
            {
                if (!IsOnline())
                {
                    throw new Exception("Brak połączenia z internetem");
                }

                ShowMessageAndStart("Sprawdzam wersję bazy na serwerze");
                FindViewById<Button>(Resource.Id.btnCheckVersion).Visibility = ViewStates.Gone;
                DictionaryXMLHelper dictionaryXMLHelper = new DictionaryXMLHelper();
                db_remote_info = await dictionaryXMLHelper.DownloadVersion();

                DictionaryDBHelper dictionaryDBHelper = new DictionaryDBHelper();
                db_local_info = dictionaryDBHelper.GetVersion();

                if (db_local_info == db_remote_info.version)
                {
                    ShowMessageAndStop(string.Format("Posiadasz aktualną wersję bazy danych."));
                    FindViewById<Button>(Resource.Id.btnCheckVersion).Visibility = ViewStates.Gone;
                    return;
                }

                ShowMessageAndStop(string.Format("Na serwerze jest nowa wersja numer: {0}", db_remote_info.version));
                FindViewById<Button>(Resource.Id.btnCheckVersion).Visibility = ViewStates.Gone;
                FindViewById<Button>(Resource.Id.btnDownload).Visibility = ViewStates.Visible;
            }
            catch (Exception ex)
            {
                FindViewById<Button>(Resource.Id.btnCheckVersion).Visibility = ViewStates.Visible;
                ShowError(ex.Message);
            }
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsOnline())
                {
                    throw new Exception("Brak połączenia z internetem");
                }

                ShowMessageAndStart("Pobieram bazę danych");
                FindViewById<Button>(Resource.Id.btnDownload).Visibility = ViewStates.Gone;
                DictionaryXMLHelper dictionaryXMLHelper = new DictionaryXMLHelper();
                db_remote_info = await dictionaryXMLHelper.DownloadVersion();

                DictionaryDBHelper dictionaryDBHelper = new DictionaryDBHelper();
                dictionaryDBHelper.SetVersion(db_remote_info.version);                

                ShowMessageAndStop(string.Format("Baza została zsynchronizowana"));
                FindViewById<Button>(Resource.Id.btnCheckVersion).Visibility = ViewStates.Gone;
                FindViewById<Button>(Resource.Id.btnDownload).Visibility = ViewStates.Gone;
            }
            catch (Exception ex)
            {
                FindViewById<Button>(Resource.Id.btnDownload).Visibility = ViewStates.Visible;
                ShowError(ex.Message);
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

        private void ShowError(string msg)
        {
            FindViewById<TextView>(Resource.Id.textCurrentAction).Visibility = ViewStates.Gone;
            FindViewById<ProgressBar>(Resource.Id.progressWaiting).Visibility = ViewStates.Gone;
            FindViewById<TextView>(Resource.Id.textError).Visibility = ViewStates.Visible;
            FindViewById<TextView>(Resource.Id.textError).Text = msg;
        }

        private void ShowMessageAndStart(string msg)
        {
            ShowMessage(msg);
            FindViewById<ProgressBar>(Resource.Id.progressWaiting).Visibility = ViewStates.Visible;
        }

        private void ShowMessageAndStop(string msg)
        {
            ShowMessage(msg);
            FindViewById<ProgressBar>(Resource.Id.progressWaiting).Visibility = ViewStates.Gone;            
        }        

        private void ShowMessage(string msg)
        {
            FindViewById<TextView>(Resource.Id.textError).Visibility = ViewStates.Gone;
            FindViewById<TextView>(Resource.Id.textCurrentAction).Visibility = ViewStates.Visible;
            FindViewById<TextView>(Resource.Id.textCurrentAction).Text = msg;
        }

    }
}