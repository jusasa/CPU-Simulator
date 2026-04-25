using System;
using System.Collections.Generic;

namespace CpuSim.Core;

public class Cpu
{
    public Memory Memory { get; } = new Memory();
    public Dictionary<RegisterType, byte> Registers { get; } = new()
    {
        { RegisterType.ACC, 0 }, { RegisterType.R0, 0 }, { RegisterType.R1, 0 }, 
        { RegisterType.R2, 0 }, { RegisterType.R3, 0 }, { RegisterType.PC, 0 }, 
        { RegisterType.SP, 0xFF }, { RegisterType.IR, 0 }, { RegisterType.MAR, 0 }, 
        { RegisterType.MBR, 0 }
    };

    public bool Halted { get; private set; }

    public IEnumerable<CpuMicroStep> ExecuteInstruction()
    {
        if (Halted) yield break;

        foreach (var step in FetchNextByte("Fetch Opcode")) yield return step;
        Registers[RegisterType.IR] = Registers[RegisterType.MBR];
        yield return new CpuMicroStep("Decode: MBR -> IR", "MBR", "IR", Registers[RegisterType.IR]);

        Opcode opcode = (Opcode)Registers[RegisterType.IR];
        foreach (var step in ExecuteOpcode(opcode)) yield return step;
    }

    private IEnumerable<CpuMicroStep> FetchNextByte(string label)
    {
        Registers[RegisterType.MAR] = Registers[RegisterType.PC];
        yield return new CpuMicroStep($"{label}: PC -> MAR", "PC", "MAR", Registers[RegisterType.MAR]);
        Registers[RegisterType.MBR] = Memory.Read(Registers[RegisterType.MAR]);
        yield return new CpuMicroStep($"{label}: Memory[MAR] -> MBR", "MEM", "MBR", Registers[RegisterType.MBR]);
        Registers[RegisterType.PC]++;
        yield return new CpuMicroStep($"{label}: PC++", "PC", "PC", Registers[RegisterType.PC]);
    }

    private IEnumerable<CpuMicroStep> ExecuteOpcode(Opcode opcode)
    {
        switch (opcode)
        {
            case Opcode.HLT:
                Halted = true;
                yield return new CpuMicroStep("Execute: HLT", null, null, 0);
                break;
            case Opcode.ADD:
            case Opcode.SUB:
            case Opcode.MUL:
            case Opcode.DIV:
                foreach (var step in ExecuteArithmetic(opcode)) yield return step;
                break;
            case Opcode.JMP:
                foreach (var step in FetchNextByte("Fetch JMP Address")) yield return step;
                Registers[RegisterType.PC] = Registers[RegisterType.MBR];
                yield return new CpuMicroStep("Execute: JMP (MBR -> PC)", "MBR", "PC", Registers[RegisterType.PC]);
                break;
            case Opcode.LOD:
                foreach (var step in FetchNextByte("Fetch LOD Reg")) yield return step;
                byte lr = Registers[RegisterType.MBR];
                foreach (var step in FetchNextByte("Fetch LOD Addr")) yield return step;
                byte la = Registers[RegisterType.MBR];
                Registers[RegisterType.MAR] = la;
                yield return new CpuMicroStep($"Execute: LOD MAR = {la:X2}", "MBR", "MAR", la);
                Registers[RegisterType.MBR] = Memory.Read(la);
                yield return new CpuMicroStep("Execute: LOD Mem[MAR] -> MBR", "MEM", "MBR", Registers[RegisterType.MBR]);
                Registers[(RegisterType)lr] = Registers[RegisterType.MBR];
                yield return new CpuMicroStep($"Execute: LOD MBR -> {(RegisterType)lr}", "MBR", ((RegisterType)lr).ToString(), Registers[RegisterType.MBR]);
                break;
            case Opcode.STR:
                foreach (var step in FetchNextByte("Fetch STR Addr")) yield return step;
                byte sa = Registers[RegisterType.MBR];
                foreach (var step in FetchNextByte("Fetch STR Reg")) yield return step;
                byte sr = Registers[RegisterType.MBR];
                Registers[RegisterType.MAR] = sa;
                yield return new CpuMicroStep($"Execute: STR MAR = {sa:X2}", null, "MAR", sa);
                Registers[RegisterType.MBR] = Registers[(RegisterType)sr];
                yield return new CpuMicroStep($"Execute: STR {(RegisterType)sr} -> MBR", ((RegisterType)sr).ToString(), "MBR", Registers[RegisterType.MBR]);
                Memory.Write(sa, Registers[RegisterType.MBR]);
                yield return new CpuMicroStep("Execute: STR MBR -> Mem[MAR]", "MBR", "MEM", Registers[RegisterType.MBR]);
                break;
            case Opcode.MOV:
                foreach (var step in FetchNextByte("Fetch MOV Dst")) yield return step;
                byte d = Registers[RegisterType.MBR];
                foreach (var step in FetchNextByte("Fetch MOV Src")) yield return step;
                byte s = Registers[RegisterType.MBR];
                Registers[(RegisterType)d] = Registers[(RegisterType)s];
                yield return new CpuMicroStep($"Execute: MOV {(RegisterType)d} = {(RegisterType)s}", ((RegisterType)s).ToString(), ((RegisterType)d).ToString(), Registers[(RegisterType)d]);
                break;
            default:
                Halted = true;
                yield return new CpuMicroStep("Unknown Opcode", null, null, 0);
                break;
        }
    }

    private IEnumerable<CpuMicroStep> ExecuteArithmetic(Opcode op)
    {
        foreach (var step in FetchNextByte("Fetch Op1")) yield return step;
        byte d = Registers[RegisterType.MBR];
        foreach (var step in FetchNextByte("Fetch Op2")) yield return step;
        byte s = Registers[RegisterType.MBR];

        byte v1 = Registers[(RegisterType)d];
        byte v2 = Registers[(RegisterType)s];
        
        yield return new CpuMicroStep($"ALU Setup: Load Latch A", ((RegisterType)d).ToString(), "ALU", v1);
        yield return new CpuMicroStep($"ALU Setup: Load Latch B", ((RegisterType)s).ToString(), "ALU", v2);

        int result = op switch {
            Opcode.ADD => v1 + v2,
            Opcode.SUB => v1 - v2,
            Opcode.MUL => v1 * v2,
            Opcode.DIV => v2 != 0 ? v1 / v2 : 0,
            _ => 0
        };

        Registers[(RegisterType)d] = (byte)result;
        yield return new CpuMicroStep($"ALU Result: {op} completed", "ALU", ((RegisterType)d).ToString(), (byte)result);
    }
}

public record AluBitOperation(string Op, int Step, byte M, byte AC, byte Q, byte Q_Extra, string Action, byte Result = 0);

public record CpuMicroStep(string Description, string? Source = null, string? Destination = null, byte Value = 0) 
{ 
    public AluBitOperation? AluBitInfo { get; init; } 
}
