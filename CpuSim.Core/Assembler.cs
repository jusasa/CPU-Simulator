using System;
using System.Collections.Generic;
using System.Linq;

namespace CpuSim.Core;

public class Assembler
{
    public byte[] Assemble(string code)
    {
        var result = new List<byte>();
        var lines = code.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var cleanLine = line.Split(';')[0].Trim(); // Remove comments
            if (string.IsNullOrWhiteSpace(cleanLine)) continue;

            var parts = cleanLine.Split(new[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var mnemonic = parts[0].ToUpper();
            var operands = parts.Skip(1).ToList();

            switch (mnemonic)
            {
                case "ADD":
                    result.AddRange(HandleArithmetic(Opcode.ADD, operands));
                    break;
                case "SUB":
                    result.AddRange(HandleArithmetic(Opcode.SUB, operands));
                    break;
                case "MUL":
                    result.AddRange(HandleArithmetic(Opcode.MUL, operands));
                    break;
                case "DIV":
                    result.AddRange(HandleArithmetic(Opcode.DIV, operands));
                    break;
                case "JMP":
                    result.Add((byte)Opcode.JMP);
                    result.Add(ParseByte(operands[0]));
                    break;
                case "LOD":
                    result.Add((byte)Opcode.LOD);
                    result.Add((byte)ParseRegister(operands[0]));
                    result.Add(ParseByte(operands[1]));
                    break;
                case "STR":
                    result.Add((byte)Opcode.STR);
                    result.Add(ParseByte(operands[0]));
                    result.Add((byte)ParseRegister(operands[1]));
                    break;
                case "MOV":
                    result.Add((byte)Opcode.MOV);
                    result.Add((byte)ParseRegister(operands[0]));
                    result.Add((byte)ParseRegister(operands[1]));
                    break;
                case "HLT":
                    result.Add((byte)Opcode.HLT);
                    break;
                default:
                    throw new Exception("Unknown mnemonic or invalid format: " + mnemonic);
            }
        }

        return result.ToArray();
    }

    private IEnumerable<byte> HandleArithmetic(Opcode op, List<string> operands)
    {
        if (operands.Count == 2)
        {
            yield return (byte)op;
            yield return (byte)ParseRegister(operands[0]);
            yield return (byte)ParseRegister(operands[1]);
        }
        else
        {
            throw new Exception($"Instruction {op} requires exactly 2 operands.");
        }
    }

    private RegisterType ParseRegister(string reg)
    {
        if (Enum.TryParse<RegisterType>(reg.ToUpper(), out var result))
        {
            return result;
        }
        throw new Exception("Unknown register: " + reg);
    }

    private byte ParseByte(string val)
    {
        if (val.StartsWith("0X", StringComparison.OrdinalIgnoreCase))
        {
            return Convert.ToByte(val[2..], 16);
        }
        return byte.Parse(val);
    }
}
