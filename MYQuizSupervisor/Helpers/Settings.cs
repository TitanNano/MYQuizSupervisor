// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MYQuizSupervisor.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string ClientIdKey = "client_id";
        private static readonly string ClientIdDefault = string.Empty;

        #endregion


        public static string ClientId
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(ClientIdKey, ClientIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(ClientIdKey, value);
            }
        }

    }
}