using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventNotifierService.Common.Plugin
{
    public interface IConfigProvider<TConfig>
    {
        TConfig LoadConfigFromFile(string fileName);
        void SaveConfigToFile(TConfig config, string fileName);
    }
}
