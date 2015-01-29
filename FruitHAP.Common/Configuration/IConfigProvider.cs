namespace FruitHAP.Common.Configuration
{
    public interface IConfigProvider<TConfig>
    {
        TConfig LoadConfigFromFile(string fileName);
        void SaveConfigToFile(TConfig config, string fileName);
    }
}
