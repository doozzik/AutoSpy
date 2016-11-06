using Rocket.API;

namespace AutoSpy
{
    public class AutoSpyConfiguration : IRocketPluginConfiguration
    {
        public int DelayInMinutes;
        
        public void LoadDefaults()
        {
            DelayInMinutes = 30;
        }
    }
}
