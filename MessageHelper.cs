using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace vocab_tester
{
    public static class MessageHelper
    {
        public static class MessageBoxQuestion
        {
            public enum MessageBoxResult
            {
                Positive, Negative, Ignore, Cancel, Closed
            };

            private static MessageBoxResult yesNoDialogResult;

            public static async Task<MessageBoxResult> Show(Context context, String title, String message, String positiveMessage, String negativeMessage)
            {
                yesNoDialogResult = MessageBoxResult.Closed;
                var alert = new AlertDialog.Builder(context)
                   .SetTitle(title).SetMessage(message)
                   .SetCancelable(false)
                   .SetIcon(Android.Resource.Drawable.IcDialogInfo);

                var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

                alert.SetPositiveButton("Tak", (senderAlert, args) =>
                {
                    yesNoDialogResult = MessageBoxResult.Positive;
                    waitHandle.Set();
                });

                alert.SetNegativeButton("Nie", (senderAlert, args) =>
                {
                    yesNoDialogResult = MessageBoxResult.Negative;
                    waitHandle.Set();
                });

                alert.Show();
                await Task.Run(() => waitHandle.WaitOne());
                return yesNoDialogResult;
            }
        }
    }
}