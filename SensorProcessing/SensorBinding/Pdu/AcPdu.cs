namespace SensorBinding.Pdu
{
    public class AcPdu
    {
        public uint DeviceId { get; set; }
        public byte UnitCode { get; set; }
        public AcCommand Command { get; set; }
        public byte Level { get; set; }
    }

    public enum AcCommand
    {
        Off = 0,
        On = 1,
        SetLevel = 2,
        GroupOff = 3,
        GroupOn = 4,
        SetGroupLevel = 5
    }
}
