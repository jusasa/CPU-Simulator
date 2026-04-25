# 8-bit CPU & ALU Algorithm Simulator

A web-based educational simulator built with Blazor WebAssembly and C# that visualizes the internal workings of a CPU and fundamental computer arithmetic algorithms.

## 🚀 Key Features

### 1. CPU Simulator
*   **Instruction-Level Simulation**: Execute assembly code with a custom 2-operand instruction set (`ADD`, `SUB`, `MUL`, `DIV`, `LOD`, `STR`, `JMP`, `MOV`, `HLT`).
*   **Visual Data Path**: Real-time visualization of data movement between registers (PC, SP, IR, MAR, MBR, ALU) via a central 8-bit system bus.
*   **Editable RAM**: Interactive 256-byte memory grid with support for Hex and Decimal inputs.
*   **Micro-Step Execution**: Step through the physical hardware cycles (`PC -> MAR -> MEM -> MBR -> PC++`) of every instruction.

### 2. 8-Bit Ripple Carry Adder
*   **Bit-by-Bit Simulation**: Visualize the ripple carry process including Carry-In and Carry-Out for each bit.
*   **Signed Support**: Supports 8-bit signed integers (-128 to 127) using 2's complement.

### 3. Booth's Multiplier
*   **Signed Multiplication**: Implements the professional Booth's algorithm for 8x8 signed multiplication.
*   **16-Bit Output**: Correctly calculates and displays the full 16-bit signed product.
*   **State Visualization**: Step-by-step execution table showing the $[AC | Q | Q_{-1}]$ state and specific arithmetic/shift actions.

### 4. Shift-Add Multiplier
*   **Unsigned Multiplication**: Demonstrates the classic shift-and-add algorithm for 8x8 unsigned integers.
*   **Incremental Progress**: Watch the partial products accumulate step-by-step.

## 🛠️ Technology Stack
*   **Frontend**: Blazor WebAssembly (C#)
*   **Core Logic**: C# Class Library
*   **Testing**: xUnit for ALU and Booth logic verification
*   **Styling**: Bootstrap for a responsive, modern UI

## 📋 How to Run Locally

### Prerequisites
*   [.NET 9 SDK](https://dotnet.microsoft.com/download)

### Steps
1.  **Clone the repository**:
    ```bash
    git clone https://github.com/YourUsername/CpuSimulator.git
    cd CpuSimulator
    ```
2.  **Run the project**:
    ```bash
    dotnet run --project CpuSim.Web
    ```
3.  **Open in Browser**:
    Navigate to the URL provided in the terminal (usually `https://localhost:5001` or `http://localhost:5000`).

## 🧪 Running Tests
To verify the arithmetic logic:
```bash
dotnet test
```

## 📜 License
This project is licensed under the MIT License.
