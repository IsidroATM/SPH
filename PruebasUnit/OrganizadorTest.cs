using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SPH.Controllers;
using SPH.Models;
using SPH.Repositories.Interfaces;
using SPH.Persistence;
using SPH.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PruebasUnit
{
    [TestFixture]
    public class OrganizadorTest
    {
        private Mock<IUnitWork> _mockUnitWork;
        private OrganizersController _controller;
        private Mock<HttpContext> _mockHttpContext;
        private SPHDbContext _context;
        private const string UserId = "1";

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SPHDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
                .Options;

            _context = new SPHDbContext(options);
            _mockUnitWork = new Mock<IUnitWork>();
            _controller = new OrganizersController(_mockUnitWork.Object, null);
            _mockHttpContext = new Mock<HttpContext>();

            // Setup the User claim
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, UserId)
    };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);

            var tempData = new TempDataDictionary(_mockHttpContext.Object, Mock.Of<ITempDataProvider>());

            _controller = new OrganizersController(_mockUnitWork.Object, _context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                },
                TempData = tempData
            };
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewResult_WithListOfTasks()
        {
            // Arrange
            var tasks = new List<Organizer>
            {
                new Organizer { TaskId = 1, Nombre = "Tarea 1", FechaCreacion = DateTime.Now, UserId = 1 },
                new Organizer { TaskId = 2, Nombre = "Tarea 2", FechaCreacion = DateTime.Now, UserId = 1 }
            };

            _mockUnitWork.Setup(u => u.organizer.GetUserTasksAsync(It.IsAny<int>())).ReturnsAsync(tasks);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
            var model = result.Model as IEnumerable<Organizer>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count());
            _mockUnitWork.Verify(u => u.organizer.GetUserTasksAsync(1), Times.Once);
        }

        [Test]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var task = new Organizer { TaskId = 1, Nombre = "New Task", UserId = 1 };

            _mockUnitWork.Setup(u => u.organizer.AgregarAsync(It.IsAny<Organizer>())).Returns(Task.CompletedTask);
            _mockUnitWork.Setup(u => u.GuardarAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(task) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.AreEqual("Index", result.ActionName, "Should redirect to Index");
            _mockUnitWork.Verify(u => u.organizer.AgregarAsync(It.IsAny<Organizer>()), Times.Once);
            _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);
            Assert.AreEqual("Tarea creada correctamente.", _controller.TempData[DS.Successful]);
        }

        [Test]
        public async Task Create_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var task = new Organizer(); // Invalid model
            _controller.ModelState.AddModelError("NombreTarea", "Required");

            // Act
            var result = await _controller.Create(task) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsNotNull(result.Model, "Model should not be null");
            Assert.AreEqual(task, result.Model, "Returned model should be the same as input");
            Assert.AreEqual("Error al guardar la tarea, intente de nuevo.", _controller.TempData[DS.Error]);
        }

        [Test]
        public async Task Edit_Get_ReturnsViewResult_WithTask()
        {
            // Arrange
            var task = new Organizer { TaskId = 1, Nombre = "Tarea 1", FechaCreacion = DateTime.Now, UserId = 1 };

            _mockUnitWork.Setup(u => u.organizer.ObtenerAsync(1)).ReturnsAsync(task);

            // Act
            var result = await _controller.Edit(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ViewResult>(result);
            var model = result.Model as Organizer;
            Assert.AreEqual(task.TaskId, model.TaskId);
        }

        [Test]
        public async Task Delete_ExistingTask_ReturnsJsonSuccess()
        {
            // Arrange
            var taskId = 2005; // ID de la tarea que se va a eliminar
            var task = new Organizer { TaskId = taskId, Nombre = "Tarea de prueba" };
            _mockUnitWork.Setup(u => u.organizer.ObtenerAsync(It.IsAny<int>())).ReturnsAsync(task);

            // Configura el mock para simular la eliminación de la tarea
            _mockUnitWork.Setup(u => u.organizer.Eliminar(It.IsAny<Organizer>())); // Esto asegura que el método de eliminación está configurado

            // Asegúrate de que GuardarAsync se configure para no devolver null
            _mockUnitWork.Setup(u => u.GuardarAsync()).Returns(Task.CompletedTask); // Esto simula el guardado

            // Act
            var result = await _controller.Delete(taskId) as JsonResult;
            Assert.IsNotNull(result, "El resultado no debería ser nulo.");
            Assert.IsNotNull(result.Value, "El valor del JSON no debería ser nulo.");

            // Verifica que el resultado es un objeto anónimo con las propiedades success y message
            var jsonDictionary = result.Value as object;
            Assert.IsNotNull(jsonDictionary, "El resultado JSON debería ser un objeto anónimo.");

            // Verifica que el objeto anónimo tiene las propiedades success y message
            var success = jsonDictionary.GetType().GetProperty("success").GetValue(jsonDictionary);
            var message = jsonDictionary.GetType().GetProperty("message").GetValue(jsonDictionary);

            Assert.IsTrue((bool)success, "La propiedad success debería ser true.");
            Assert.AreEqual("Tarea eliminada correctamente.", message, "El mensaje es incorrecto.");

            // Verificar que los métodos correctos fueron llamados
            _mockUnitWork.Verify(u => u.organizer.ObtenerAsync(taskId), Times.Once);
            _mockUnitWork.Verify(u => u.organizer.Eliminar(task), Times.Once);
            _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);
        }



        [Test]
        public async Task Delete_NonExistingTask_ReturnsJsonFailure()
        {
            // Arrange
            int taskId = 999; // ID no existente
            _mockUnitWork.Setup(u => u.organizer.ObtenerAsync(It.IsAny<int>())).ReturnsAsync((Organizer)null);

            // Act
            var result = await _controller.Delete(taskId) as JsonResult;
            Assert.IsNotNull(result, "El resultado no debería ser nulo.");
            Assert.IsNotNull(result.Value, "El valor del JSON no debería ser nulo.");

            // Verifica que el resultado es un objeto anónimo con las propiedades success y message
            var jsonDictionary = result.Value as object;
            Assert.IsNotNull(jsonDictionary, "El resultado JSON debería ser un objeto anónimo.");

            // Verifica que el objeto anónimo tiene las propiedades success y message
            var success = jsonDictionary.GetType().GetProperty("success").GetValue(jsonDictionary);
            var message = jsonDictionary.GetType().GetProperty("message").GetValue(jsonDictionary);

            Assert.IsFalse((bool)success, "La propiedad success debería ser false.");
            Assert.AreEqual("Error al eliminar la tarea.", message, "El mensaje es incorrecto.");

            // Verifica que no se llamó al método Eliminar ni a GuardarAsync
            _mockUnitWork.Verify(u => u.organizer.Eliminar(It.IsAny<Organizer>()), Times.Never);
            _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Never);
        }


    }
}