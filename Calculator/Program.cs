using System;
using System.IO;
using System.Text;
using Xunit;

namespace Calculator.Tests;

public class ProgramTests
{
    [Fact]
    public void Soma_Should_Correctly_Add_Two_Positive_Numbers()
    {
        // Arrange
        float num1 = 10;
        float num2 = 20;

        // Act
        using (var consoleOutput = new StringWriter())
        {
            Console.SetOut(consoleOutput);

            // Create a mock for Console.ReadLine()
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(num1.ToString());
            stringBuilder.AppendLine(num2.ToString());
            var stringReader = new StringReader(stringBuilder.ToString());
            Console.SetIn(stringReader);

            Program.Soma();

            // Assert
            Assert.Contains($"Soma {num1 + num2}", consoleOutput.ToString());
        }
    }

    [Fact]
    public void Soma_Should_Handle_Negative_Numbers()
    {
        // Arrange
        float num1 = -10;
        float num2 = -5;

        // Act
        using (var consoleOutput = new StringWriter())
        {
            Console.SetOut(consoleOutput);

            // Create a mock for Console.ReadLine()
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(num1.ToString());
            stringBuilder.AppendLine(num2.ToString());
            var stringReader = new StringReader(stringBuilder.ToString());
            Console.SetIn(stringReader);

            Program.Soma();

            // Assert
            Assert.Contains($"Soma {num1 + num2}", consoleOutput.ToString());
        }
    }

    [Fact]
    public void Soma_Should_Handle_Zero()
    {
        // Arrange
        float num1 = 0;
        float num2 = 5;

        // Act
        using (var consoleOutput = new StringWriter())
        {
            Console.SetOut(consoleOutput);

            // Create a mock for Console.ReadLine()
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(num1.ToString());
            stringBuilder.AppendLine(num2.ToString());
            var stringReader = new StringReader(stringBuilder.ToString());
            Console.SetIn(stringReader);

            Program.Soma();

            // Assert
            Assert.Contains($"Soma {num1 + num2}", consoleOutput.ToString());
        }
    }

    [Fact]
    public void Soma_Should_Handle_Decimal_Numbers()
    {
        // Arrange
        float num1 = 2.5f;
        float num2 = 3.5f;

        // Act
        using (var consoleOutput = new StringWriter())
        {
            Console.SetOut(consoleOutput);

            // Create a mock for Console.ReadLine()
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(num1.ToString());
            stringBuilder.AppendLine(num2.ToString());
            var stringReader = new StringReader(stringBuilder.ToString());
            Console.SetIn(stringReader);

            Program.Soma();

            // Assert
            Assert.Contains($"Soma {num1 + num2}", consoleOutput.ToString());
        }
    }
}
