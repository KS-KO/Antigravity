namespace CordisDigitalTwin;

public class ControlLogic
{
    private readonly IMotorHardware _hardware;
    public double TargetRPM { get; set; }

    public ControlLogic(IMotorHardware hardware)
    {
        _hardware = hardware;
    }

    /// <summary>
    /// L (Logic) Layer Update
    /// Cordis SUITE에서 생성된 제어 알고리즘이 실행되는 위치입니다.
    /// </summary>
    public void Update()
    {
        double current = _hardware.GetCurrentRPM();
        
        // Error calculation
        double error = TargetRPM - current;
        
        // Control Algorithm (Simplified P-control)
        // 로직 계층은 하드웨어의 전압을 제어하여 목표 RPM에 도달하게 합니다.
        double outputVoltage = error * 0.05; 
        
        // 전압 제한 (0V ~ 100V)
        outputVoltage = Math.Clamp(outputVoltage, 0, 100);
        
        _hardware.SetVoltage(outputVoltage);

        // Safety Logic (PLD의 핵심: 독립적인 레이어 보호)
        if (_hardware.GetTemperature() > 95 || _hardware.IsFaulty)
        {
            _hardware.SetVoltage(0);
            TargetRPM = 0;
            Console.WriteLine("[LOGIC] EMERGENCY_STOP: Overheating or Fault detected!");
        }
    }
}
