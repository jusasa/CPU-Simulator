namespace CpuSim.Core;

public enum RegisterType : byte
{
    ACC = 0,
    R0 = 1,
    R1 = 2,
    R2 = 3,
    R3 = 4,
    PC = 5,
    SP = 6,
    IR = 7,
    MAR = 8,
    MBR = 9
}

public enum Opcode : byte
{
    HLT = 0x00,
    
    // Arithmetic (2-operand only: dst, src -> dst = dst op src)
    ADD = 0x11,
    SUB = 0x21,
    MUL = 0x31,
    DIV = 0x41,
    
    // Jump
    JMP = 0x50, // JMP addr
    
    // Load/Store (2-operand only: reg, addr or addr, reg)
    LOD = 0x61, // LOD reg, addr -> reg = [addr]
    STR = 0x71, // STR addr, reg -> [addr] = reg
    
    // Move
    MOV = 0x80  // MOV dst, src
}
