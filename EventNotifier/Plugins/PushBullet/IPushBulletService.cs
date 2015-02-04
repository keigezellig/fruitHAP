using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventNotifier.Plugins.PushBullet
{
    public interface IPushBulletService
    {
        void Initialize(string accessToken, string pushBulletUri);
        void PostNote(string title, string body, string channel);        
    }
}
