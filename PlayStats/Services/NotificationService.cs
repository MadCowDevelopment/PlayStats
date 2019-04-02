using System;
using System.Windows.Markup;
using MaterialDesignThemes.Wpf;

namespace PlayStats.Services
{
    public interface INotificationService
    {
        void Queue(string message);
    }

    public class NotificationService : INotificationService
    {
        internal static SnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(5));

        public void Queue(string message)
        {
            MessageQueue.Enqueue(message);
        }
    }

    /// <summary>
    /// Provides shorthand to initialise a new <see cref="SnackbarMessageQueue"/> for a <see cref="Snackbar"/> in the UI.
    /// </summary>
    [MarkupExtensionReturnType(typeof(SnackbarMessageQueue))]
    public class MessageQueueExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return NotificationService.MessageQueue;
        }
    }
}
