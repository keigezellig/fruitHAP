namespace FruitHAP.Common.PhysicalInterfaces
{
    public interface IPhysicalInterfaceFactory
    {
        IPhysicalInterface GetPhysicalInterface(string connectionString);
    }
}
