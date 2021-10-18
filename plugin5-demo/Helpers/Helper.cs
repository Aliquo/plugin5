using Aliquo.Windows;
using System.Threading.Tasks;

namespace plugin5_demo.Helpers
{
    internal class Helper
    {
        /// <summary>Muestra o añade una notificación al usuario</summary>
        internal static void SendNotification(IHost host, string title, string message, Aliquo.Core.NotificationType type = Aliquo.Core.NotificationType.Information, Aliquo.Core.NotificationHideStyle style = Aliquo.Core.NotificationHideStyle.AutoHide, bool add = true)
        {
            Aliquo.Core.Models.Notification notification = new Aliquo.Core.Models.Notification
            {
                Title = title,
                Type = type,
                HideStyle = style,
                Message = message
            };

            if (add)
                Task.Factory.StartNew(async () => await host.Management.AddNotificationAsync(host.Environment.IdUser, notification));
            else
                host.Management.Views.ShowNotification(notification);
        }
    }
}
