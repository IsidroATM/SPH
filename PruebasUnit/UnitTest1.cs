using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using SPH.Controllers;
using SPH.Models;
using SPH.Repositories.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Routing;

namespace PruebasUnit
{
    public class UsersControllerTests
    {
        private Mock<IUnitWork> _mockUnitWork;
        private UsersController _controller;
        private Mock<HttpContext> _mockHttpContext;
        private Mock<IAuthenticationService> _mockAuthService;
        private Mock<ITempDataDictionary> _mockTempData;
        private Mock<IUrlHelper> _mockUrlHelper;

        [SetUp]
        public void Setup()
        {
            // Inicializar Mock del UnitWork
            _mockUnitWork = new Mock<IUnitWork>();

            // Crear un Mock para HttpContext
            _mockHttpContext = new Mock<HttpContext>();

            // Crear un Mock para IAuthenticationService
            _mockAuthService = new Mock<IAuthenticationService>();

            // Crear un Mock para TempData
            _mockTempData = new Mock<ITempDataDictionary>();

            // Crear un Mock para UrlHelper
            _mockUrlHelper = new Mock<IUrlHelper>();

            // Simular el contexto Http con el servicio de autenticación
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(_mockAuthService.Object);

            _mockHttpContext.Setup(h => h.RequestServices)
                .Returns(serviceProviderMock.Object);

            // Crear el controlador y asignar el contexto simulado
            _controller = new UsersController(_mockUnitWork.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _mockHttpContext.Object
                },
                TempData = _mockTempData.Object, // Asignar TempData mockeado
                Url = _mockUrlHelper.Object      // Asignar UrlHelper mockeado
            };
        }

        [Test]
        public async Task LoginOk()
        {
            // Preparar
            string email = "isiatom114@gmail.com";
            string password = "Axia2114";

            // Simulación de lo que debería devolver el UnitWork al buscar el usuario
            var mockUser = new User
            {
                Id = 7,
                Nombre = "Isidro Antonio Torres Martínez",
                Correo = email,
                Contraseña = password,
                Rol = "Estudiante"
            };

            _mockUnitWork.Setup(u => u.user.GetUsuario(email, It.IsAny<string>()))
                .ReturnsAsync(mockUser);

            // Preparar el ClaimsPrincipal para el usuario autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, mockUser.Nombre),
                new Claim(ClaimTypes.Email, mockUser.Correo),
                new Claim(ClaimTypes.Role, mockUser.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Simular el proceso de SignInAsync
            _mockAuthService.Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), claimsPrincipal, null))
                .Returns(Task.CompletedTask);

            // Simular la redirección
            _mockUrlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("/Home/Index");

            // Ejecutar: Llamar al método Login del controlador
            var result = await _controller.Login(email, password);

            // Verificación
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirectResult.ActionName);
        }

        [Test]
        public async Task LoginFailed()
        {
            // Preparar: Usuario incorrecto
            string u = "wrong@test.com";
            string p = "wrongpassword";

            // Simular que no se encuentra ningún usuario
            _mockUnitWork.Setup(u => u.user.GetUsuario(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Ejecutar: Llamar al método Login del controlador
            var result = await _controller.Login(u, p);

            // Verificación: Asegurarse de que se regresa la vista de Login con un mensaje de error
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("No se encontraron coincidencias", _controller.ViewData["Mensaje"]);
        }
    }
}
