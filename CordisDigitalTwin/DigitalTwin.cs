namespace CordisDigitalTwin;

public class DigitalTwin
{
    private readonly MotorState _physicalState;
    public MotorState VirtualState { get; private set; }

    public DigitalTwin(MotorState physicalState)
    {
        _physicalState = physicalState;
        VirtualState = new MotorState();
    }

    public void Sync()
    {
        // Simple mirroring for now
        VirtualState.CurrentRPM = _physicalState.CurrentRPM;
        VirtualState.Temperature = _physicalState.Temperature;
        VirtualState.Status = _physicalState.Status;
        VirtualState.IsRunning = _physicalState.IsRunning;
    }

    public string GetHealthReport()
    {
        if (VirtualState.Temperature > 80)
            return "CRITICAL: Overheating detected in virtual model!";
        if (VirtualState.Temperature > 60)
            return "WARNING: Elevated temperature trend.";
        
        return "System Healthy (Predicted)";
    }
}
