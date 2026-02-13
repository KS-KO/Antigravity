using CordisDigitalTwin;
using System.Diagnostics;

Console.WriteLine("==================================================");
Console.WriteLine("  Cordis SUITE PLD (Physical, Logic, Data) Model");
Console.WriteLine("==================================================");

// 1. Physical Layer (P) Initialization
var motorState = new MotorState();
var physicalHardware = new PhysicalMotor(motorState);

// 2. Logic Layer (L) Initialization (Uses P-Layer Interface)
var controlLogic = new ControlLogic(physicalHardware);

// 3. Data Layer (D) / Digital Twin Initialization (Observes P-Layer State)
var digitalTwin = new DigitalTwin(motorState);

Console.WriteLine("Initializing Simulation with PLD Architecture...");

double deltaTime = 0.5;
int tick = 0;

// Set initial target
controlLogic.TargetRPM = 1500;

while (tick < 100)
{
    // A. Logic Update (L Layer) - 제어 결정을 내림
    controlLogic.Update();

    // B. Physical Update (P Layer) - 현실 세계 시뮬레이션
    physicalHardware.Update(deltaTime);

    // C. Data/Twin Sync (D Layer) - 가상 모델 업데이트
    digitalTwin.Sync();

    // D. Monitoring & UI
    if (tick % 10 == 0)
    {
        Console.WriteLine($"\n--- [TIME SYNC: TICK {tick}] ---");
        Console.WriteLine($"[PHYSICAL (P)] {motorState}");
        Console.WriteLine($"[TWIN MVB (D)] {digitalTwin.VirtualState}");
        Console.WriteLine($"[ANALYSIS    ] {digitalTwin.GetHealthReport()}");

        // Dynamic Scenario Demo
        if (tick == 30)
        {
            Console.WriteLine("\n>>> COMMAND: Increase Speed to 3500 RPM (Stress Test)");
            controlLogic.TargetRPM = 3500;
        }
        if (tick == 70)
        {
            Console.WriteLine("\n>>> COMMAND: Return to 1000 RPM (System Cooling)");
            controlLogic.TargetRPM = 1000;
        }
    }

    tick++;
}

Console.WriteLine("\nSimulation completed successfully.");
Console.WriteLine("PLD 분리 원칙에 따라 로직 수정 없이 하드웨어 교체가 가능합니다.");
