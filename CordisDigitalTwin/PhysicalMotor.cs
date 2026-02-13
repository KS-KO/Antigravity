namespace CordisDigitalTwin;

public class PhysicalMotor : IMotorHardware
{
    public MotorState State { get; private set; }
    private readonly Random _random = new();

    public PhysicalMotor(MotorState state)
    {
        State = state;
    }

    // P Layer Implementation
    public double GetCurrentRPM() => State.CurrentRPM;
    public double GetTemperature() => State.Temperature;
    public void SetVoltage(double voltage) => State.AppliedVoltage = voltage;
    public bool IsFaulty => State.Temperature > 100;

    public void Update(double deltaTime)
    {
        // Physics simulation: Voltage drives RPM increase
        if (State.AppliedVoltage > 0)
        {
            double acceleration = State.AppliedVoltage * 50.0; // Simple physics constant
            State.CurrentRPM += (acceleration - (State.CurrentRPM * 0.1)) * deltaTime;
            State.IsRunning = true;
        }
        else
        {
            // Natural friction deceleration
            State.CurrentRPM *= Math.Max(0, 1.0 - (0.5 * deltaTime));
            State.IsRunning = State.CurrentRPM > 1.0;
        }

        // Temperature depends on RPM and duration
        double targetTemp = 25.0 + (State.CurrentRPM / 100.0) * 3.0;
        State.Temperature += (targetTemp - State.Temperature) * 0.05 * deltaTime;

        // Power Consumption based on voltage and load
        State.PowerConsumption = Math.Abs(State.AppliedVoltage * 10.0) + (_random.NextDouble() * 2.0);

        // Status assignment
        if (IsFaulty) State.Status = "FAULT";
        else if (State.IsRunning) State.Status = State.CurrentRPM > 100 ? "Running" : "Starting";
        else State.Status = "Idle";

        // Add some noise
        State.CurrentRPM += (_random.NextDouble() - 0.5) * 1.5;
        if (State.CurrentRPM < 0) State.CurrentRPM = 0;
    }
}
