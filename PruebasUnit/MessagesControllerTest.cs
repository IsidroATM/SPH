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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PruebasUnit
{
    [TestFixture]
    public class MessagesControllerTests
    {
        private Mock<IUnitWork> _mockUnitWork;
        private MessagesController _controller;
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
            _mockHttpContext = new Mock<HttpContext>();

            // Configuración del User claim
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, UserId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);

            var tempData = new TempDataDictionary(_mockHttpContext.Object, Mock.Of<ITempDataProvider>());

            _controller = new MessagesController(_mockUnitWork.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object },
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
        public async Task Index_ReturnsViewResult_WithListOfMessages()
        {
            // Arrange
            int messengerId = 1;
            var sentMessages = new List<Message>
            {
                new Message { MessageId = 1, Content = "Hello", SenderId = 1, ReceiverId = 2, MessengerId = messengerId }
            };
            var receivedMessages = new List<Message>
            {
                new Message { MessageId = 2, Content = "Hi", SenderId = 2, ReceiverId = 1, MessengerId = messengerId }
            };

            _mockUnitWork.Setup(u => u.message.GetSentMessagesAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(sentMessages);
            _mockUnitWork.Setup(u => u.message.GetReceivedMessagesAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(receivedMessages);

            // Act
            var result = await _controller.Index(messengerId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as Tuple<IEnumerable<Message>, IEnumerable<Message>>;
            Assert.AreEqual(1, model.Item1.Count()); // Sent messages
            Assert.AreEqual(1, model.Item2.Count()); // Received messages
        }

        [Test]
        public async Task Create_Get_ReturnsViewResultWithMessage()
        {
            // Arrange
            int messengerId = 1;

            // Act
            var result = await _controller.Create(messengerId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var message = result.Model as Message;
            Assert.AreEqual(messengerId, message.MessengerId);
            Assert.AreEqual(int.Parse(UserId), message.SenderId); // Verifica que el ID del remitente sea el del usuario autenticado
        }

        [Test]
        public async Task Create_Post_AddsMessageAndRedirects()
        {
            // Arrange
            int messengerId = 1;
            string content = "Test message";

            _mockUnitWork.Setup(u => u.message.AgregarAsync(It.IsAny<Message>())).Verifiable();
            _mockUnitWork.Setup(u => u.GuardarAsync()).Verifiable();

            // Act
            var result = await _controller.Create(messengerId, null, content) as RedirectToActionResult;

            // Assert
            _mockUnitWork.Verify(u => u.message.AgregarAsync(It.IsAny<Message>()), Times.Once);
            _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual(messengerId, result.RouteValues["messengerId"]);
        }

        [Test]
        public async Task Edit_Get_ReturnsViewResultWithMessage()
        {
            // Arrange
            int messageId = 1;
            var message = new Message { MessageId = messageId, Content = "Test" };

            _mockUnitWork.Setup(u => u.message.ObtenerAsync(messageId)).ReturnsAsync(message);

            // Act
            var result = await _controller.Edit(messageId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(message, result.Model);
        }

        [Test]
        public async Task DeleteConfirmed_DeletesMessageAndReturnsJson()
        {
            // Arrange
            int messageId = 1;
            var message = new Message { MessageId = messageId, Content = "To be deleted" };

            _mockUnitWork.Setup(u => u.message.ObtenerAsync(messageId)).ReturnsAsync(message);
            _mockUnitWork.Setup(u => u.message.Eliminar(message)).Verifiable();
            _mockUnitWork.Setup(u => u.GuardarAsync()).Verifiable();

            // Act
            var result = await _controller.DeleteConfirmed(messageId) as JsonResult;

            // Assert
            _mockUnitWork.Verify(u => u.message.Eliminar(message), Times.Once);
            _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsTrue((bool)result.Value.GetType().GetProperty("success").GetValue(result.Value, null));
        }
    }
}
