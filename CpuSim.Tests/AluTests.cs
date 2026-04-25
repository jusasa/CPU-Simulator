using CpuSim.Core;
using Xunit;

namespace CpuSim.Tests;

public class AluTests
{
    [Theory]
    [InlineData(7, 3, 21)]      // 7 * 3 = 21
    [InlineData(10, 2, 20)]     // 10 * 2 = 20
    [InlineData(127, 2, 254)]   // Max 7-bit * 2
    [InlineData(0, 5, 0)]       // 0 * 5 = 0
    [InlineData(7, -3, -21)]    // Positive * Negative
    [InlineData(-7, 3, -21)]    // Negative * Positive
    [InlineData(-7, -3, 21)]    // Negative * Negative
    [InlineData(-127, 1, -127)] // Signed edge case
    public void BoothMultiply_CorrectResult(int mInt, int qInt, int expected)
    {
        byte m = (byte)(sbyte)mInt;
        byte q = (byte)(sbyte)qInt;
        var alu = new Alu();
        alu.ResetBooth(m, q);
        
        while (alu.BoothStep()) { }
        
        // Combine AC and Q into a 16-bit signed integer
        short result = unchecked((short)((alu.AC << 8) | alu.Q));
        Assert.Equal(expected, (int)result);
    }

    [Fact]
    public void Adder_CorrectResult()
    {
        var alu = new Alu();
        alu.ResetAdder(100, 50);
        
        while (alu.AdderStepBit(out _, out _, out _, out _, out _)) { }
        
        Assert.Equal(150, alu.AdderSum);
        Assert.Equal(0, alu.AdderCarry);
    }

    [Fact]
    public void Adder_WithCarry()
    {
        var alu = new Alu();
        alu.ResetAdder(200, 100); // 300 = 256 + 44
        
        while (alu.AdderStepBit(out _, out _, out _, out _, out _)) { }
        
        Assert.Equal(44, alu.AdderSum);
        Assert.Equal(1, alu.AdderCarry);
    }

    [Theory]
    [InlineData(10, 5, 50)]
    [InlineData(12, 12, 144)]
    [InlineData(255, 1, 255)]
    [InlineData(255, 2, 510)]
    [InlineData(0, 100, 0)]
    public void ShiftAddMultiply_CorrectResult(byte m, byte q, int expected)
    {
        var alu = new Alu();
        alu.ResetShiftAdd(m, q);
        
        while (alu.ShiftAddStepIteration()) { }
        
        Assert.Equal(expected, (int)alu.ShiftAddProduct);
    }
}
