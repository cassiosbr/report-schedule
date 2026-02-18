using Microsoft.AspNetCore.Mvc;
using ReportSchedule.Controllers;

namespace ReportSchedule.Tests.Controllers;

public class HealthControllerTests
{
    [Fact]
    public void Get_ReturnsExpectedStatus()
    {
        // Arrange
        var controller = new HealthController();

        // Act
        var result = controller.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var value = result.Value;
        
        // Verifica se o objeto retornado tem a propriedade status = "ok"
        var statusProperty = value?.GetType().GetProperty("status");
        Assert.NotNull(statusProperty);
        Assert.Equal("ok", statusProperty.GetValue(value));
    }
}
