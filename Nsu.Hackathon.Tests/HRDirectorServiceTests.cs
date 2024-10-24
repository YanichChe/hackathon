using Hackathon.services.impl;
using Xunit;
using Assert = Xunit.Assert;

namespace Nsu.Hackathon.Tests;

public class HRDirectorServiceTests
{
    private readonly HRDirectorService _service;

    public HRDirectorServiceTests()
    {
        _service = new HRDirectorService();
    }

    [Fact]
    public void CalculateHarmonicity_SameNumbers_ReturnsSameValue()
    {
        // Arrange
        var pairs = new List<int> { 5, 5 };

        // Act
        double result = _service.CalculateHarmonicity(pairs);

        // Assert
        Assert.Equal(5.0, result); // Среднее гармоническое одинаковых чисел должно быть равно этому числу
    }

    [Fact]
    public void CalculateHarmonicity_TwoAndSix_ReturnsThree()
    {
        // Arrange
        var pairs = new List<int> { 2, 6 };

        // Act
        double result = _service.CalculateHarmonicity(pairs);

        // Assert
        Assert.Equal(3.0, result); // Среднее гармоническое для 2 и 6 должно быть 3
    }

    [Fact]
    public void CalculateHarmonicity_PredefinedPreferences_ReturnsExpectedValue()
    {
        // Arrange
        var pairs = new List<int> { 5, 10, 15, 20 };

        // Act
        double result = _service.CalculateHarmonicity(pairs);

        // Assert
        Assert.Equal(48.0 / 5.0, result); // Ожидаемое значение для заранее определенных предпочтений
    }
}