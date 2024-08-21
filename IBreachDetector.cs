namespace TypewiseAlertSystem
{
    public interface IBreachDetector
    {
        BreachType InferBreach(double value, double lowerLimit, double upperLimit);
    }
}
