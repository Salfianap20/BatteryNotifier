using System.Media;

namespace AdvancedBatteryNotifier
{
    public static class AudioHelper
    {
        public static void Play(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    SoundPlayer player = new SoundPlayer(path);
                    player.Play();
                }
            }
            catch
            {
            }
        }
    }
}