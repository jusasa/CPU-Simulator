using System;
using System.Collections.Generic;

namespace CpuSim.Core;

public class Alu
{
    // Booth Simulation State
    public byte M { get; set; }
    public byte AC { get; set; }
    public byte Q { get; set; }
    public byte Q_minus_1 { get; set; }
    public int Step { get; set; }
    public string LastAction { get; set; } = "Initialized";

    public void ResetBooth(byte multiplicand, byte multiplier)
    {
        M = multiplicand;
        Q = multiplier;
        AC = 0;
        Q_minus_1 = 0;
        Step = 0;
        LastAction = "Booth's Setup Complete";
    }

    public bool BoothStep()
    {
        if (Step >= 8) return false;

        byte q0 = (byte)(Q & 1);
        
        // 1. Comparison & Arithmetic
        if (q0 == 1 && Q_minus_1 == 0)
        {
            AC = (byte)(AC - M);
            LastAction = $"10 detected: AC = AC - M ({AC:X2})";
        }
        else if (q0 == 0 && Q_minus_1 == 1)
        {
            AC = (byte)(AC + M);
            LastAction = $"01 detected: AC = AC + M ({AC:X2})";
        }
        else
        {
            LastAction = $"{(q0 == 0 ? "00" : "11")} detected: No arithmetic";
        }

        // 2. Combined Arithmetic Right Shift (AC and Q)
        byte q_lsb = (byte)(Q & 1);
        byte ac_lsb = (byte)(AC & 1);
        
        Q_minus_1 = q_lsb;

        // Shift Q
        Q = (byte)(Q >> 1);
        Q |= (byte)(ac_lsb << 7);
        
        // Shift AC (Arithmetic Shift Right - Preserve Sign)
        if ((AC & 0x80) != 0)
        {
            AC = (byte)((AC >> 1) | 0x80);
        }
        else
        {
            AC = (byte)(AC >> 1);
        }

        LastAction += " -> Shifted Right";
        Step++;
        return true;
    }

    // Adder Simulation State
    public byte AdderA { get; set; }
    public byte AdderB { get; set; }
    public byte AdderSum { get; set; }
    public byte AdderCarry { get; set; }
    public int AdderStep { get; set; }

    public void ResetAdder(byte a, byte b)
    {
        AdderA = a;
        AdderB = b;
        AdderSum = 0;
        AdderCarry = 0;
        AdderStep = 0;
    }

    public bool AdderStepBit(out byte bitA, out byte bitB, out byte cin, out byte sum, out byte cout)
    {
        if (AdderStep >= 8)
        {
            bitA = bitB = cin = sum = cout = 0;
            return false;
        }

        bitA = (byte)((AdderA >> AdderStep) & 1);
        bitB = (byte)((AdderB >> AdderStep) & 1);
        cin = AdderCarry;
        
        sum = (byte)(bitA ^ bitB ^ cin);
        cout = (byte)((bitA & bitB) | (cin & (bitA ^ bitB)));

        AdderSum |= (byte)(sum << AdderStep);
        AdderCarry = cout;
        AdderStep++;
        return true;
    }

    // Shift-Add (Unsigned) Multiplication State
    public byte ShiftAddM { get; set; }
    public byte ShiftAddQ { get; set; }
    public ushort ShiftAddProduct { get; set; }
    public int ShiftAddStep { get; set; }
    public string ShiftAddAction { get; set; } = "Initialized";

    public void ResetShiftAdd(byte multiplicand, byte multiplier)
    {
        ShiftAddM = multiplicand;
        ShiftAddQ = multiplier;
        ShiftAddProduct = 0;
        ShiftAddStep = 0;
        ShiftAddAction = "Shift-Add Setup Complete";
    }

    public bool ShiftAddStepIteration()
    {
        if (ShiftAddStep >= 8) return false;

        // Check the LSB of Q
        byte q0 = (byte)((ShiftAddQ >> ShiftAddStep) & 1);
        
        if (q0 == 1)
        {
            ushort addVal = (ushort)(ShiftAddM << ShiftAddStep);
            ShiftAddProduct += addVal;
            ShiftAddAction = $"Bit {ShiftAddStep} is 1: Add {ShiftAddM} << {ShiftAddStep} ({addVal:X2})";
        }
        else
        {
            ShiftAddAction = $"Bit {ShiftAddStep} is 0: No addition";
        }

        ShiftAddStep++;
        return true;
    }
}
