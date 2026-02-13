namespace CordisDigitalTwin;

public class MotorState
{
    public double TargetRPM { get; set; }
    public double CurrentRPM { get; set; }
    public double Temperature { get; set; }
    public bool IsRunning { get; set; }
    public double PowerConsumption { get; set; }
    public double AppliedVoltage { get; set; }
    public string Status { get; set; } = "Idle";

    public override string ToString()
    {
        return $"[Status: {Status}] RPM: {CurrentRPM:F1}/{TargetRPM:F1} | Temp: {Temperature:F1}°C | Power: {PowerConsumption:F1}W | Volt: {AppliedVoltage:F2}V";
    }
}
