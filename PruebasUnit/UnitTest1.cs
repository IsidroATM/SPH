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
using Microsoft.AspNetCore.Authentication.Cookies;

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
                Contraseña = SPH.Utilities.Serv_Encrip.EncriptarClave(password), // Encriptar clave como en el controlador
                Rol = "Estudiante"
            };

            // Simular la respuesta de UnitWork para GetUsuario
            _mockUnitWork.Setup(u => u.user.GetUsuario(email, It.IsAny<string>()))
                .ReturnsAsync(mockUser);

            // Preparar los claims esperados para el usuario autenticado
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, mockUser.Nombre),
        new Claim(ClaimTypes.NameIdentifier, mockUser.Id.ToString()),
        new Claim(ClaimTypes.Role, mockUser.Rol)
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Capturar el ClaimsPrincipal pasado a SignInAsync
            ClaimsPrincipal capturedClaimsPrincipal = null;

            _mockAuthService.Setup(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme, // Esquema correcto
                It.IsAny<ClaimsPrincipal>(),  // Capturar ClaimsPrincipal
                It.IsAny<AuthenticationProperties>()))
                .Callback<HttpContext, string, ClaimsPrincipal, AuthenticationProperties>((context, scheme, principal, properties) =>
                {
                    capturedClaimsPrincipal = principal;
                })
                .Returns(Task.CompletedTask);

            // Ejecutar: Llamar al método Login del controlador
            var result = await _controller.Login(email, password);

            // Verificación de la redirección
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Index", redirectResult.ActionName);

            // Verificar que el método SignInAsync fue llamado una vez
            _mockAuthService.Verify(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,  // Verificar que se utilizó el esquema "Cookies"
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()), Times.Once);

            // Verificar que los reclamos en capturedClaimsPrincipal son correctos
            Assert.NotNull(capturedClaimsPrincipal);
            Assert.IsTrue(capturedClaimsPrincipal.HasClaim(c => c.Type == ClaimTypes.Name && c.Value == mockUser.Nombre));
            Assert.IsTrue(capturedClaimsPrincipal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier && c.Value == mockUser.Id.ToString()));
            Assert.IsTrue(capturedClaimsPrincipal.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == mockUser.Rol));
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

        [Test]
        public async Task LoginEmptyCredentials()
        {
            var result = await _controller.Login("", "");
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("Correo y contraseña son requeridos", _controller.ViewData["Mensaje"]);
        }

        [Test]
        public async Task Register_EmailAlreadyExists()
        {
            // Simular un usuario ya existente
            var existingUser = new User { Id = 1, Correo = "existing@test.com" };
            _mockUnitWork.Setup(u => u.user.GetUsuarioByEmail(It.IsAny<string>())).ReturnsAsync(existingUser);

            // Preparar el usuario nuevo con el mismo correo
            var newUser = new User { Correo = "existing@test.com", Contraseña = "password123" };

            // Ejecutar
            var result = await _controller.Register(newUser);

            // Verificar que se regresa la vista de registro con el mensaje de error
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("El correo ya está registrado. Por favor, intenta con otro correo.", _controller.ViewData["Mensaje"]);
        }

        [Test]
        public async Task Details_UserFound_ReturnsViewWithUser()
        {
            // Preparar
            int userId = 7;

            // Simulación de lo que debería devolver UnitWork al buscar el usuario
            var mockUser = new User
            {
                Id = userId,
                Nombre = "Isidro Antonio Torres Martínez",
                Correo = "isiatom114@gmail.com"
            };

            // Simular la obtención del ID del usuario de los claims
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            // Simular HttpContext con los claims
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Simular la respuesta de UnitWork para GetUsuarioById
            _mockUnitWork.Setup(u => u.user.GetUsuarioById(userId))
                .ReturnsAsync(mockUser);

            // Ejecutar: Llamar al método Details del controlador
            var result = await _controller.Details();

            // Verificación
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.AreEqual(mockUser, viewResult.Model);
        }

        [Test]
        public async Task Details_UserNotFound_ReturnsNotFound()
        {
            // Preparar
            int userId = 7;

            // Simular la obtención del ID del usuario de los claims
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            // Simular HttpContext con los claims
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Simular que no se encuentra el usuario
            _mockUnitWork.Setup(u => u.user.GetUsuarioById(userId))
                .ReturnsAsync((User)null);

            // Ejecutar: Llamar al método Details del controlador
            var result = await _controller.Details();

            // Verificación
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //

        [Test]
        public async Task Edit_Get_UserFound_ReturnsViewWithUser()
        {
            // Preparar
            int userId = 7;

            var mockUser = new User
            {
                Id = userId,
                Nombre = "Isidro Antonio Torres Martínez"
            };

            // Simular la obtención del ID del usuario de los claims
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            // Simular HttpContext con los claims
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Simular la respuesta de UnitWork para GetUsuarioById
            _mockUnitWork.Setup(u => u.user.GetUsuarioById(userId))
                .ReturnsAsync(mockUser);

            // Ejecutar: Llamar al método Edit (GET) del controlador
            var result = await _controller.Edit();

            // Verificación
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.AreEqual(mockUser, viewResult.Model);
        }

        [Test]
        public async Task Edit_Get_UserNotFound_ReturnsNotFound()
        {
            // Preparar
            int userId = 7;

            // Simular la obtención del ID del usuario de los claims
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            // Simular HttpContext con los claims
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Simular que no se encuentra el usuario
            _mockUnitWork.Setup(u => u.user.GetUsuarioById(userId))
                .ReturnsAsync((User)null);

            // Ejecutar: Llamar al método Edit (GET) del controlador
            var result = await _controller.Edit();

            // Verificación
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        //

        [Test]
        public async Task Edit_Post_ValidModel_UpdatesUserAndRedirects()
        {
            // Preparar
            int userId = 7;
            var mockUser = new User
            {
                Id = userId,
                Nombre = "Isidro Antonio Torres Martínez",
                Correo = "isiatom114@gmail.com"
            };

            // Simular la obtención del ID del usuario de los claims
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            // Simular HttpContext con los claims
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = userClaims }
            };

            // Simular la actualización del usuario en UnitWork
            _mockUnitWork.Setup(u => u.user.UpdateUsuario(mockUser))
                .Returns(Task.CompletedTask);

            // Ejecutar: Llamar al método Edit (POST) del controlador
            var result = await _controller.Edit(mockUser);

            // Verificación
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Details", redirectResult.ActionName);

            // Verificar que el método UpdateUsuario fue llamado
            _mockUnitWork.Verify(u => u.user.UpdateUsuario(mockUser), Times.Once);
        }

        //

        [Test]
        public async Task CerrarSesion_LogsOutUserAndRedirectsToLogin()
        {
            // Preparar: Simular HttpContext con SignOutAsync
            _mockAuthService.Setup(a => a.SignOutAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme, null))
                .Returns(Task.CompletedTask);

            // Ejecutar: Llamar al método CerrarSesion del controlador
            var result = await _controller.CerrarSesion();

            // Verificación
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Login", redirectResult.ActionName);
            Assert.AreEqual("Users", redirectResult.ControllerName);

            // Verificar que SignOutAsync fue llamado
            _mockAuthService.Verify(a => a.SignOutAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme, null), Times.Once);
        }

    }
}
