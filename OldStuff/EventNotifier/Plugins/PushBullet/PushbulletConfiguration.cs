using System;

namespace EventNotifier.Plugins.PushBullet
{
    [Serializable]
    public class PushbulletConfiguration
    {
        public string PushbulletUri { get; set; }
        public string ApiKey { get; set; }
        public string Channel { get; set; }
    }
}
