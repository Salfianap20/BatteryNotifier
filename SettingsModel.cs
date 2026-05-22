namespace AdvancedBatteryNotifier
{
    public class SettingsModel
    {
        public int LowBatteryLimit { get; set; } = 30;

        public int FullBatteryLimit { get; set; } = 100;

        public int LowBatteryIntervalSeconds { get; set; } = 15;

        public int FullBatteryIntervalSeconds { get; set; } = 60;

        public string AlarmPath { get; set; } = "";
    }
}