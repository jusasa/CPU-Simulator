namespace CpuSim.Core;

public class Memory
{
    private readonly byte[] _data = new byte[256];

    public byte Read(byte address) => _data[address];
    public void Write(byte address, byte value) => _data[address] = value;

    public void Load(byte[] program, byte startAddress = 0)
    {
        for (int i = 0; i < program.Length && (startAddress + i) < 256; i++)
        {
            _data[startAddress + i] = program[i];
        }
    }

    public byte[] GetData() => _data;
}
