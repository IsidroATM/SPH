using NUnit.Framework;
using Moq;
using SPH.Controllers;
using SPH.Models;
using SPH.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace SPH.Tests.Controllers
{
    [TestFixture]
    public class ThemesControllerTests
    {
        private Mock<IUnitWork> _mockUnitWork;
        private ThemesController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUnitWork = new Mock<IUnitWork>();
            _controller = new ThemesController(_mockUnitWork.Object);
        }

        [Test]
        public async Task Index_ReturnsViewWithThemes()
        {
            // Arrange
            var expectedThemes = new List<Theme> { new Theme(), new Theme() };
            _mockUnitWork.Setup(uow => uow.theme.ObtenerTodosAsync(
                It.IsAny<Expression<Func<Theme, bool>>>(),
                It.IsAny<Func<IQueryable<Theme>, IOrderedQueryable<Theme>>>(),
                It.IsAny<string>(),
                It.IsAny<bool>()))
                .ReturnsAsync(expectedThemes);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsTrue(viewResult.Model is IEnumerable<Theme>);
            Assert.AreEqual(expectedThemes.Count, ((IEnumerable<Theme>)viewResult.Model).Count());
        }

        [Test]
        public async Task Details_WithValidId_ReturnsViewWithTheme()
        {
            // Arrange
            var themeId = 1;
            var expectedTheme = new Theme { ThemeId = themeId };
            _mockUnitWork.Setup(uow => uow.theme.ObtenerAsync(themeId)).ReturnsAsync(expectedTheme);

            // Act
            var result = await _controller.Details(themeId);

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = (ViewResult)result;
            Assert.IsInstanceOf<Theme>(viewResult.Model);
            Assert.AreEqual(themeId, ((Theme)viewResult.Model).ThemeId);
        }

        [Test]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var invalidThemeId = 999;
            _mockUnitWork.Setup(uow => uow.theme.ObtenerAsync(invalidThemeId)).ReturnsAsync((Theme)null);

            // Act
            var result = await _controller.Details(invalidThemeId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Create_ValidTheme_RedirectsToIndex()
        {
            // Arrange
            var theme = new Theme { Name = "Test Theme" };
            _mockUnitWork.Setup(uow => uow.theme.AgregarAsync(It.IsAny<Theme>())).Returns(Task.CompletedTask);
            _mockUnitWork.Setup(uow => uow.GuardarAsync()).Returns(Task.CompletedTask);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.Create(theme);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [Test]
        public async Task Edit_ValidTheme_RedirectsToIndex()
        {
            // Arrange
            var themeId = 1;
            var theme = new Theme { ThemeId = themeId, Name = "Updated Theme" };
            _mockUnitWork.Setup(uow => uow.theme.ObtenerAsync(themeId)).ReturnsAsync(theme);
            _mockUnitWork.Setup(uow => uow.theme.Actualizar(It.IsAny<Theme>()));
            _mockUnitWork.Setup(uow => uow.GuardarAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(themeId, theme);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [Test]
        public async Task DeleteConfirmed_ValidId_RedirectsToIndex()
        {
            // Arrange
            var themeId = 1;
            var theme = new Theme { ThemeId = themeId };
            _mockUnitWork.Setup(uow => uow.theme.ObtenerAsync(themeId)).ReturnsAsync(theme);
            _mockUnitWork.Setup(uow => uow.theme.Eliminar(It.IsAny<Theme>()));
            _mockUnitWork.Setup(uow => uow.GuardarAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(themeId);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectToActionResult = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirectToActionResult.ActionName);
        }

        [Test]
        public async Task ApplyTheme_ValidId_ReturnsSuccessJson()
        {
            // Arrange
            var themeId = 1;
            var theme = new Theme { ThemeId = themeId };
            _mockUnitWork.Setup(uow => uow.theme.ObtenerAsync(themeId)).ReturnsAsync(theme);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _controller.ApplyTheme(themeId);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = (JsonResult)result;
            var data = JObject.FromObject(jsonResult.Value);
            Assert.IsTrue((bool)data["success"]);
        }

        [Test]
        public async Task ApplyTheme_InvalidId_ReturnsErrorJson()
        {
            // Arrange
            var invalidThemeId = 999;
            _mockUnitWork.Setup(uow => uow.theme.ObtenerAsync(invalidThemeId)).ReturnsAsync((Theme)null);

            // Act
            var result = await _controller.ApplyTheme(invalidThemeId);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result);
            var jsonResult = (JsonResult)result;
            var data = JObject.FromObject(jsonResult.Value);
            Assert.IsFalse((bool)data["success"]);
            Assert.AreEqual("Tema no encontrado", (string)data["message"]);
        }


        // Helper class for testing sessions
        public class TestSession : ISession
        {
            private Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

            public string Id => throw new System.NotImplementedException();

            public bool IsAvailable => throw new System.NotImplementedException();

            public IEnumerable<string> Keys => _sessionStorage.Keys;

            public void Clear() => _sessionStorage.Clear();

            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

            public void Remove(string key) => _sessionStorage.Remove(key);

            public void Set(string key, byte[] value) => _sessionStorage[key] = value;

            public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
        }
    }

}