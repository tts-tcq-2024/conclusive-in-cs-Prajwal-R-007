using System;

namespace TypewiseAlertSystem
{
    public class TypewiseAlert : IBreachDetector, ITemperatureClassifier, IAlertSender
    {
        public BreachType InferBreach(double value, double lowerLimit, double upperLimit)
        {
            if (value < lowerLimit) return BreachType.TOO_LOW;
            if (value > upperLimit) return BreachType.TOO_HIGH;
            return BreachType.NORMAL;
        }

        public BreachType ClassifyTemperatureBreach(CoolingType coolingType, double temperatureInC)
        {
            var limits = GetTemperatureLimits(coolingType);
            return InferBreach(temperatureInC, limits.lowerLimit, limits.upperLimit);
        }

        private (double lowerLimit, double upperLimit) GetTemperatureLimits(CoolingType coolingType)
        {
            return coolingType switch
            {
                CoolingType.PASSIVE_COOLING => (0, 35),
                CoolingType.HI_ACTIVE_COOLING => (0, 45),
                CoolingType.MED_ACTIVE_COOLING => (0, 40),
                _ => throw new ArgumentException("Invalid cooling type", nameof(coolingType))
            };
        }

        public void SendAlert(AlertTarget alertTarget, BreachType breachType)
        {
            if (alertTarget == AlertTarget.TO_CONTROLLER)
            {
                SendToController(breachType);
            }
            else if (alertTarget == AlertTarget.TO_EMAIL)
            {
                SendToEmail(breachType);
            }
        }

        private void SendToController(BreachType breachType)
        {
            const ushort header = 0xfeed;
            Console.WriteLine($"{header} : {breachType}");
        }

        private void SendToEmail(BreachType breachType)
        {
            string recipient = "a.b@c.com";
            string message = GetEmailMessage(breachType);
            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine($"To: {recipient}\n{message}");
            }
        }

        private string GetEmailMessage(BreachType breachType)
        {
            return breachType switch
            {
                BreachType.TOO_LOW => "Hi, the temperature is too low",
                BreachType.TOO_HIGH => "Hi, the temperature is too high",
                _ => string.Empty
            };
        }

        public void CheckAndAlert(AlertTarget alertTarget, BatteryCharacter batteryChar, double temperatureInC)
        {
            BreachType breachType = ClassifyTemperatureBreach(batteryChar.CoolingType, temperatureInC);
            SendAlert(alertTarget, breachType);
        }
    }
}
