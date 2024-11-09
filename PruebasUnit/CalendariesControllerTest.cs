using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using SPH.Controllers;
using SPH.Repositories.Interfaces;
using SPH.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

[TestFixture]
public class CalendariesControllerTests
{
    private Mock<IUnitWork> _mockUnitWork;
    private CalendariesController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockUnitWork = new Mock<IUnitWork>();
        _controller = new CalendariesController(_mockUnitWork.Object);

        // Setup TempData
        var tempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
        );
        _controller.TempData = tempData;

        // Setup HttpContext with authenticated user
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1") // User ID is "1"
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Test]
    public async Task Create_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var calendar = new Calendar { EventId = 1, UserId = 1 };
        _mockUnitWork.Setup(u => u.calendar.AgregarAsync(It.IsAny<Calendar>())).Returns(Task.CompletedTask);
        _mockUnitWork.Setup(u => u.GuardarAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(calendar) as RedirectToActionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(nameof(CalendariesController.Index), result.ActionName);
        _mockUnitWork.Verify(u => u.calendar.AgregarAsync(It.IsAny<Calendar>()), Times.Once);
        _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);
    }

    [Test]
    public async Task Create_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var calendar = new Calendar(); // Assume invalid due to missing required fields
        _controller.ModelState.AddModelError("Error", "Invalid model");

        // Act
        var result = await _controller.Create(calendar) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(calendar, result.Model);
        Assert.IsFalse(result.ViewData.ModelState.IsValid);
    }

    [Test]
    public async Task Delete_NullId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.Delete((int?)null);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task Delete_InvalidId_ReturnsJsonWithSuccessFalse()
    {
        // Arrange
        int invalidId = 1;
        _mockUnitWork.Setup(u => u.calendar.ObtenerAsync(invalidId)).ReturnsAsync((Calendar)null);

        // Act
        var result = await _controller.Delete(invalidId) as JsonResult;

        // Assert
        var json = result.Value as Dictionary<string, object>;
        Assert.IsFalse((bool)json["success"]);
        Assert.AreEqual("Evento no encontrado.", json["message"]);
    }

    [Test]
    public async Task Delete_ValidId_CallsEliminarAndGuardarAsync()
    {
        // Arrange
        int validId = 1;
        var calendar = new Calendar { EventId = validId };
        _mockUnitWork.Setup(u => u.calendar.ObtenerAsync(validId)).ReturnsAsync(calendar);

        // Act
        var result = await _controller.Delete(validId) as JsonResult;

        // Assert
        _mockUnitWork.Verify(u => u.calendar.Eliminar(calendar), Times.Once);
        _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);

        var json = result.Value as Dictionary<string, object>;
        Assert.IsTrue((bool)json["success"]);
        Assert.AreEqual("Evento eliminado correctamente.", json["message"]);
    }

    [Test]
    public async Task Update_EventColor_SuccessfullyUpdated()
    {
        // Arrange
        var eventId = 1;
        var newColor = "#00FF00";
        var existingEvent = new Calendar
        {
            EventId = eventId,
            NombreEvento = "Evento Existente",
            DetalleEvento = "Detalle del evento existente",
            FechaIniEvento = DateTime.Now.Date,
            FechaFinEvento = DateTime.Now.Date.AddHours(2),
            HoraEvento = new TimeSpan(14, 0, 0),
            ColorEvento = "#FF0000",
            UserId = 1
        };
        _mockUnitWork.Setup(u => u.calendar.ObtenerAsync(eventId)).ReturnsAsync(existingEvent);
        _mockUnitWork.Setup(u => u.calendar.Actualizar(It.IsAny<Calendar>()));
        _mockUnitWork.Setup(u => u.GuardarAsync()).Returns(Task.CompletedTask);

        // Act
        existingEvent.ColorEvento = newColor;
        var result = await _controller.Edit(eventId, existingEvent) as RedirectToActionResult;

        // Assert
        Assert.IsNotNull(result);
        _mockUnitWork.Verify(u => u.calendar.Actualizar(It.Is<Calendar>(c => c.ColorEvento == newColor)), Times.Once);
        _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);
    }
    [Test]
    public async Task AddMultipleEventsOnSameDay_SuccessfullyAdded()
    {
        // Arrange
        var userId = 1;
        var today = DateTime.Now.Date;
        var events = new List<Calendar>
    {
        new Calendar {
            NombreEvento = "Evento 1",
            DetalleEvento = "Detalle 1",
            FechaIniEvento = today,
            FechaFinEvento = today.AddHours(2),
            HoraEvento = new TimeSpan(9, 0, 0),
            ColorEvento = "#FF0000",
            UserId = userId
        },
        new Calendar {
            NombreEvento = "Evento 2",
            DetalleEvento = "Detalle 2",
            FechaIniEvento = today,
            FechaFinEvento = today.AddHours(2),
            HoraEvento = new TimeSpan(14, 0, 0),
            ColorEvento = "#00FF00",
            UserId = userId
        }
    };

        _mockUnitWork.Setup(u => u.calendar.AgregarAsync(It.IsAny<Calendar>())).Returns(Task.CompletedTask);
        _mockUnitWork.Setup(u => u.GuardarAsync()).Returns(Task.CompletedTask);

        // Act
        foreach (var evt in events)
        {
            await _controller.Create(evt);
        }

        // Assert
        _mockUnitWork.Verify(u => u.calendar.AgregarAsync(It.IsAny<Calendar>()), Times.Exactly(2));
        _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Exactly(2));
    }
    
}