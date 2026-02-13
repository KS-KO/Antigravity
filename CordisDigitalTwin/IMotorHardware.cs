namespace CordisDigitalTwin;

/// <summary>
/// P (Physical) Layer Interface
/// Cordis SUITE의 하드웨어 추상화 계층을 나타냅니다.
/// </summary>
public interface IMotorHardware
{
    double GetCurrentRPM();
    double GetTemperature();
    void SetVoltage(double voltage);
    bool IsFaulty { get; }
}
