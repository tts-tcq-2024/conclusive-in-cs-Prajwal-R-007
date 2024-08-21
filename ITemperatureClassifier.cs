namespace TypewiseAlertSystem
{
    public interface ITemperatureClassifier
    {
        BreachType ClassifyTemperatureBreach(CoolingType coolingType, double temperatureInC);
    }
}
