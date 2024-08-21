using System;
using Xunit;
using TypewiseAlertSystem;

namespace TypewiseAlertSystem.Tests
{
    public class TypewiseAlertTests
    {
        [Theory]
        [InlineData(25, 0, 35, TypewiseAlert.BreachType.NORMAL)]
        [InlineData(-5, 0, 35, TypewiseAlert.BreachType.TOO_LOW)]
        [InlineData(40, 0, 35, TypewiseAlert.BreachType.TOO_HIGH)]
        public void InferBreach_ShouldReturnCorrectBreachType(double value, double lowerLimit, double upperLimit, TypewiseAlert.BreachType expectedBreachType)
        {
            // Act
            var result = TypewiseAlert.inferBreach(value, lowerLimit, upperLimit);

            // Assert
            Assert.Equal(expectedBreachType, result);
        }

        [Theory]
        [InlineData(TypewiseAlert.CoolingType.PASSIVE_COOLING, 20, TypewiseAlert.BreachType.NORMAL)]
        [InlineData(TypewiseAlert.CoolingType.PASSIVE_COOLING, -1, TypewiseAlert.BreachType.TOO_LOW)]
        [InlineData(TypewiseAlert.CoolingType.PASSIVE_COOLING, 36, TypewiseAlert.BreachType.TOO_HIGH)]
        [InlineData(TypewiseAlert.CoolingType.HI_ACTIVE_COOLING, 20, TypewiseAlert.BreachType.NORMAL)]
        [InlineData(TypewiseAlert.CoolingType.HI_ACTIVE_COOLING, 46, TypewiseAlert.BreachType.TOO_HIGH)]
        public void ClassifyTemperatureBreach_ShouldReturnCorrectBreachType(TypewiseAlert.CoolingType coolingType, double temperatureInC, TypewiseAlert.BreachType expectedBreachType)
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
            var consoleOutput = new ConsoleOutput();
            var batteryChar = new TypewiseAlert.BatteryCharacter
            {
                coolingType = TypewiseAlert.CoolingType.PASSIVE_COOLING,
                brand = "TestBrand"
            };

            // Act
            TypewiseAlert.checkAndAlert(TypewiseAlert.AlertTarget.TO_CONTROLLER, batteryChar, 20);

            // Assert
            Assert.Contains("0xfeed : NORMAL", consoleOutput.GetOutput());
        }

        [Fact]
        public void CheckAndAlert_ShouldSendToEmail()
        {
            // Arrange
            var consoleOutput = new ConsoleOutput();
            var batteryChar = new TypewiseAlert.BatteryCharacter
            {
                coolingType = TypewiseAlert.CoolingType.PASSIVE_COOLING,
                brand = "TestBrand"
            };

            // Act
            TypewiseAlert.checkAndAlert(TypewiseAlert.AlertTarget.TO_EMAIL, batteryChar, -1);

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

