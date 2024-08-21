namespace TypewiseAlertSystem
{
    public interface IAlertSender
    {
        void SendAlert(AlertTarget alertTarget, BreachType breachType);
    }
}
