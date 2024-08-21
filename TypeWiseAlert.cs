using System;

namespace TypewiseAlertSystem
{
    public static class TypewiseAlert
    {
        public static BreachType InferBreach(double value, double lowerLimit, double upperLimit)
        {
            if (value < lowerLimit)
                return BreachType.TOO_LOW;
            if (value > upperLimit)
                return BreachType.TOO_HIGH;
            return BreachType.NORMAL;
        }

        public static BreachType ClassifyTemperatureBreach(CoolingType coolingType, double temperatureInC)
        {
            var limits = GetTemperatureLimits(coolingType);
            return InferBreach(temperatureInC, limits.lowerLimit, limits.upperLimit);
        }

        private static (double lowerLimit, double upperLimit) GetTemperatureLimits(CoolingType coolingType)
        {
            return coolingType switch
            {
                CoolingType.PASSIVE_COOLING => (0, 35),
                CoolingType.HI_ACTIVE_COOLING => (0, 45),
                CoolingType.MED_ACTIVE_COOLING => (0, 40),
                _ => throw new ArgumentException("Invalid cooling type", nameof(coolingType))
            };
        }

        public static void CheckAndAlert(AlertTarget alertTarget, BatteryCharacter batteryChar, double temperatureInC)
        {
            BreachType breachType = ClassifyTemperatureBreach(batteryChar.CoolingType, temperatureInC);
            if (alertTarget == AlertTarget.TO_CONTROLLER)
            {
                SendToController(breachType);
            }
            else if (alertTarget == AlertTarget.TO_EMAIL)
            {
                SendToEmail(breachType);
            }
        }

        private static void SendToController(BreachType breachType)
        {
            const ushort header = 0xfeed;
            Console.WriteLine($"{header} : {breachType}");
        }

        private static void SendToEmail(BreachType breachType)
        {
            string recipient = "a.b@c.com";
            switch (breachType)
            {
                case BreachType.TOO_LOW:
                    Console.WriteLine($"To: {recipient}\nHi, the temperature is too low\n");
                    break;
                case BreachType.TOO_HIGH:
                    Console.WriteLine($"To: {recipient}\nHi, the temperature is too high\n");
                    break;
                case BreachType.NORMAL:
                    break;
            }
        }
    }
}
