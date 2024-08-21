using System;
using Xunit;
using TypewiseAlertSystem;

namespace TypewiseAlertSystem.Tests
{
    public class TypewiseAlertTests
    {
        [Theory]
        [InlineData(25, 0, 35, BreachType.NORMAL)]
        [InlineData(-5, 0, 35, BreachType.TOO_LOW)]
        [InlineData(40, 0, 35, BreachType.TOO_HIGH)]
        public void InferBreach_ShouldReturnCorrectBreachType(double value, double lowerLimit, double upperLimit, BreachType expectedBreachType)
        {
            // Act
            var result = TypewiseAlert.inferBreach(value, lowerLimit, upperLimit);

            // Assert
            Assert.Equal(expectedBreachType, result);
        }

        [Theory]
        [InlineData(CoolingType.PASSIVE_COOLING, 20, BreachType.NORMAL)]
        [InlineData(CoolingType.PASSIVE_COOLING, -1, BreachType.TOO_LOW)]
        [InlineData(CoolingType.PASSIVE_COOLING, 36, BreachType.TOO_HIGH)]
        [InlineData(CoolingType.HI_ACTIVE_COOLING, 20, BreachType.NORMAL)]
        [InlineData(CoolingType.HI_ACTIVE_COOLING, 46, BreachType.TOO_HIGH)]
        public void ClassifyTemperatureBreach_ShouldReturnCorrectBreachType(CoolingType coolingType, double temperatureInC, BreachType expectedBreachType)
        {
            // Act
            var result = TypewiseAlert.classifyTemperatureBreach(coolingType, temperatureInC);

            // Assert
            Assert.Equal(expectedBreachType, result);
        }

        [Fact]
        public void CheckAndAlert_ShouldSendToController()
        {
            // Arrange
            using var consoleOutput = new ConsoleOutput();
            var batteryChar = new BatteryCharacter
            {
                CoolingType = CoolingType.PASSIVE_COOLING,
                Brand = "TestBrand"
            };

            // Act
            TypewiseAlert.checkAndAlert(AlertTarget.TO_CONTROLLER, batteryChar, 20);

            // Assert
            Assert.Contains("0xfeed : NORMAL", consoleOutput.GetOutput());
        }

        [Fact]
        public void CheckAndAlert_ShouldSendToEmail()
        {
            // Arrange
            using var consoleOutput = new ConsoleOutput();
            var batteryChar = new BatteryCharacter
            {
                CoolingType = CoolingType.PASSIVE_COOLING,
                Brand = "TestBrand"
            };

            // Act
            TypewiseAlert.checkAndAlert(AlertTarget.TO_EMAIL, batteryChar, -1);

            // Assert
            Assert.Contains("To: a.b@c.com", consoleOutput.GetOutput());
            Assert.Contains("Hi, the temperature is too low", consoleOutput.GetOutput());
        }

        private class ConsoleOutput : IDisposable
        {
            private StringWriter _stringWriter;
            private TextWriter _originalOutput;

            public ConsoleOutput()
            {
                _originalOutput = Console.Out;
                _stringWriter = new StringWriter();
                Console.SetOut(_stringWriter);
            }

            public string GetOutput() => _stringWriter.ToString().Trim();

            public void Dispose()
            {
                Console.SetOut(_originalOutput);
                _stringWriter.Dispose();
            }
        }
    }
}
