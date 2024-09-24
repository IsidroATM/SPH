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

    public class DiariesControllerTests

    {

        private Mock<IUnitWork> _mockUnitWork;

        private DiariesController _controller;

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


            // Setup the User claim

            var claims = new List<Claim>

      {

        new Claim(ClaimTypes.NameIdentifier, UserId)

      };

            var identity = new ClaimsIdentity(claims, "TestAuthType");

            var claimsPrincipal = new ClaimsPrincipal(identity);


            _mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);


            var tempData = new TempDataDictionary(_mockHttpContext.Object, Mock.Of<ITempDataProvider>());


            _controller = new DiariesController(_mockUnitWork.Object, _context)

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

        public async Task Index_ReturnsViewResult_WithListOfDiaries()

        {

            // Arrange

            var diaries = new List<Diary>

      {

        new Diary { NoteId = 1, NombreNota = "Nota 1", Descripcion = "Descripción 1", FechaCreacion = DateTime.Now, UserId = 1 },

        new Diary { NoteId = 2, NombreNota = "Nota 2", Descripcion = "Descripción 2", FechaCreacion = DateTime.Now, UserId = 1 }

      };


            _mockUnitWork.Setup(u => u.diary.GetUserNotesAsync(It.IsAny<int>())).ReturnsAsync(diaries);


            // Act

            var result = await _controller.Index(null) as ViewResult;


            // Assert

            Assert.IsNotNull(result);

            Assert.IsInstanceOf<ViewResult>(result);

            var model = result.Model as IEnumerable<Diary>;

            Assert.IsNotNull(model);

            Assert.AreEqual(2, model.Count());

            _mockUnitWork.Verify(u => u.diary.GetUserNotesAsync(1), Times.Once);

        }


        [Test]
        public async Task Index_ReturnsErrorView_WhenExceptionOccurs()
        {
            // Arrange
            _mockUnitWork.Setup(u => u.diary.GetUserNotesAsync(It.IsAny<int>()))
             .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Index(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");


            
            Assert.AreEqual("Error", result.ViewName);             
            Assert.AreEqual("Se produjo un error al obtener las notas.", _controller.TempData[DS.Error]);
        }


        [Test]

        public async Task Create_ValidModel_RedirectsToIndex()

        {

            // Arrange

            var diary = new Diary { NoteId = 1, NombreNota = "New Note", UserId = 1 };


            _mockUnitWork.Setup(u => u.diary.AgregarAsync(It.IsAny<Diary>()))

              .Returns(Task.CompletedTask);

            _mockUnitWork.Setup(u => u.GuardarAsync())

              .Returns(Task.CompletedTask);


            // Act

            var result = await _controller.Create(diary) as RedirectToActionResult;


            // Assert

            Assert.IsNotNull(result, "Result should not be null");

            Assert.AreEqual("Index", result.ActionName, "Should redirect to Index");

            _mockUnitWork.Verify(u => u.diary.AgregarAsync(It.IsAny<Diary>()), Times.Once);

            _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);

            Assert.AreEqual("Nota creada correctamente.", _controller.TempData[DS.Successful]);

        }


        [Test]
        public async Task Create_InvalidModel_ReturnsViewWithModel()

        {

            // Arrange
            var diary = new Diary(); // Invalid model
            _controller.ModelState.AddModelError("NombreNota", "Required");

            // Act
            var result = await _controller.Create(diary) as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsNotNull(result.Model, "Model should not be null");
            Assert.AreEqual(diary, result.Model, "Returned model should be the same as input");
            Assert.AreEqual("Error al guardar la nota, intente de nuevo.", _controller.TempData[DS.Error]);
        }


        [Test]

        public async Task Edit_Get_ReturnsViewResult_WithDiary()

        {

            // Arrange

            var diary = new Diary { NoteId = 1, NombreNota = "Nota 1", Descripcion = "Descripción 1", FechaCreacion = DateTime.Now, UserId = 1 };

            _mockUnitWork.Setup(u => u.diary.ObtenerAsync(1)).ReturnsAsync(diary);


            // Act

            var result = await _controller.Edit(1) as ViewResult;


            // Assert

            Assert.IsNotNull(result);

            Assert.IsInstanceOf<ViewResult>(result);

            var model = result.Model as Diary;

            Assert.AreEqual(diary.NoteId, model.NoteId);

        }


        [Test]

        public async Task Edit_Post_ReturnsRedirectToActionResult_WhenModelStateIsValid()

        {

            // Arrange

            var diary = new Diary { NoteId = 1, NombreNota = "Nota Editada", Descripcion = "Descripción Editada", FechaCreacion = DateTime.Now, UserId = 1 };


            _mockUnitWork.Setup(u => u.diary.Actualizar(diary));

            _mockUnitWork.Setup(u => u.GuardarAsync()).Returns(Task.CompletedTask);


            // Act

            var result = await _controller.Edit(1, diary) as RedirectToActionResult;


            // Assert

            Assert.IsNotNull(result);

            Assert.AreEqual("Index", result.ActionName);

        }

        [Test]

        public async Task Delete_ExistingDiary_ReturnsJsonSuccess()

        {

            // Arrange

            int diaryId = 1;

            var diary = new Diary { NoteId = diaryId, NombreNota = "Note to delete" };


            _mockUnitWork.Setup(u => u.diary.ObtenerAsync(diaryId))

              .ReturnsAsync(diary);

            _mockUnitWork.Setup(u => u.diary.Eliminar(It.IsAny<Diary>())).Verifiable();

            _mockUnitWork.Setup(u => u.GuardarAsync())

              .Returns(Task.CompletedTask);


            // Act

            var result = await _controller.Delete(diaryId) as JsonResult;


            // Assert

            Assert.IsNotNull(result, "Result should not be null");

            Assert.IsNotNull(result.Value, "JSON value should not be null");


            // Imprimir el tipo y valor del resultado para depuración

            Console.WriteLine($"Result type: {result.Value.GetType()}");

            Console.WriteLine($"Result value: {Newtonsoft.Json.JsonConvert.SerializeObject(result.Value)}");


            if (result.Value is IDictionary<string, object> jsonDictionary)

            {

                Assert.IsTrue(jsonDictionary.ContainsKey("success"), "JSON should contain 'success' key");

                Assert.IsTrue((bool)jsonDictionary["success"], "Success should be true");

                Assert.IsTrue(jsonDictionary.ContainsKey("message"), "JSON should contain 'message' key");

                Assert.AreEqual("Nota eliminada correctamente.", jsonDictionary["message"]);

            }

            else if (result.Value is object anonymousObject)

            {

                // Si es un objeto anónimo, accedemos a sus propiedades mediante reflexión

                var successProperty = anonymousObject.GetType().GetProperty("success");

                var messageProperty = anonymousObject.GetType().GetProperty("message");


                Assert.IsNotNull(successProperty, "JSON should contain 'success' property");

                Assert.IsTrue((bool)successProperty.GetValue(anonymousObject), "Success should be true");

                Assert.IsNotNull(messageProperty, "JSON should contain 'message' property");

                Assert.AreEqual("Nota eliminada correctamente.", messageProperty.GetValue(anonymousObject));

            }

            else

            {

                Assert.Fail($"Unexpected result type: {result.Value.GetType()}");

            }


            _mockUnitWork.Verify(u => u.diary.Eliminar(It.IsAny<Diary>()), Times.Once);

            _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);

        }

        [Test]

        public async Task Delete_NonExistingDiary_ReturnsJsonFailure()

        {

            // Arrange

            int diaryId = 999; // ID que no existe en la base de datos

            _mockUnitWork.Setup(u => u.diary.ObtenerAsync(diaryId))

              .ReturnsAsync((Diary)null); // El diario no se encuentra


            // Act

            var result = await _controller.Delete(diaryId) as JsonResult;


            // Assert

            Assert.IsNotNull(result, "Result should not be null");

            Assert.IsNotNull(result.Value, "JSON value should not be null");


            // Imprimir el tipo y valor del resultado para depuración

            Console.WriteLine($"Result type: {result.Value.GetType()}");

            Console.WriteLine($"Result value: {Newtonsoft.Json.JsonConvert.SerializeObject(result.Value)}");


            if (result.Value is IDictionary<string, object> jsonDictionary)

            {

                Assert.IsTrue(jsonDictionary.ContainsKey("success"), "JSON should contain 'success' key");

                Assert.IsFalse((bool)jsonDictionary["success"], "Success should be false");

                Assert.IsTrue(jsonDictionary.ContainsKey("message"), "JSON should contain 'message' key");

                Assert.AreEqual("Nota no encontrada.", jsonDictionary["message"]);

            }

            else if (result.Value is object anonymousObject)

            {

                // Si es un objeto anónimo, accedemos a sus propiedades mediante reflexión

                var successProperty = anonymousObject.GetType().GetProperty("success");

                var messageProperty = anonymousObject.GetType().GetProperty("message");


                Assert.IsNotNull(successProperty, "JSON should contain 'success' property");

                Assert.IsFalse((bool)successProperty.GetValue(anonymousObject), "Success should be false");

                Assert.IsNotNull(messageProperty, "JSON should contain 'message' property");

                Assert.AreEqual("Nota no encontrada.", messageProperty.GetValue(anonymousObject));

            }

            else

            {

                Assert.Fail($"Unexpected result type: {result.Value.GetType()}");

            }


            // Verificar que Eliminar no haya sido llamado

            _mockUnitWork.Verify(u => u.diary.Eliminar(It.IsAny<Diary>()), Times.Never);

            _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Never);

        }



        [Test]

        public async Task AutoSave_ValidModel_ReturnsJsonSuccess()

        {

            // Arrange

            var diary = new Diary { NoteId = 1, NombreNota = "Auto Saved Note" };


            _mockUnitWork.Setup(u => u.diary.ObtenerAsync(diary.NoteId))

              .ReturnsAsync(diary);

            _mockUnitWork.Setup(u => u.diary.Actualizar(It.IsAny<Diary>())).Verifiable();

            _mockUnitWork.Setup(u => u.GuardarAsync()).Returns(Task.CompletedTask);


            // Act

            var result = await _controller.AutoSave(diary) as JsonResult;


            // Assert

            Assert.IsNotNull(result, "Result should not be null");

            Assert.IsNotNull(result.Value, "JSON value should not be null");


            // Imprimir el tipo y valor del resultado para depuración

            Console.WriteLine($"Result type: {result.Value.GetType()}");

            Console.WriteLine($"Result value: {Newtonsoft.Json.JsonConvert.SerializeObject(result.Value)}");


            if (result.Value is IDictionary<string, object> jsonDictionary)

            {

                Assert.IsTrue(jsonDictionary.ContainsKey("success"), "JSON should contain 'success' key");

                Assert.IsTrue((bool)jsonDictionary["success"], "Success should be true");

                Assert.IsTrue(jsonDictionary.ContainsKey("message"), "JSON should contain 'message' key");

                Assert.AreEqual("Guardado automático realizado.", jsonDictionary["message"]);

            }

            else if (result.Value is object anonymousObject)

            {

                // Si es un objeto anónimo, accedemos a sus propiedades mediante reflexión

                var successProperty = anonymousObject.GetType().GetProperty("success");

                var messageProperty = anonymousObject.GetType().GetProperty("message");


                Assert.IsNotNull(successProperty, "JSON should contain 'success' property");

                Assert.IsTrue((bool)successProperty.GetValue(anonymousObject), "Success should be true");

                Assert.IsNotNull(messageProperty, "JSON should contain 'message' property");

                Assert.AreEqual("Guardado automático realizado.", messageProperty.GetValue(anonymousObject));

            }

            else

            {

                Assert.Fail($"Unexpected result type: {result.Value.GetType()}");

            }


            _mockUnitWork.Verify(u => u.diary.Actualizar(It.IsAny<Diary>()), Times.Once);

            _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);

        }


        [Test]

        public async Task AutoSave_NewDiary_ReturnsJsonSuccess()

        {

            // Arrange

            var diary = new Diary { NoteId = 0, NombreNota = "New Auto Saved Note" };


            _mockUnitWork.Setup(u => u.diary.AgregarAsync(It.IsAny<Diary>()))

              .Returns(Task.CompletedTask);

            _mockUnitWork.Setup(u => u.GuardarAsync()).Returns(Task.CompletedTask);


            // Act

            var result = await _controller.AutoSave(diary) as JsonResult;


            // Assert

            Assert.IsNotNull(result, "Result should not be null");

            Assert.IsNotNull(result.Value, "JSON value should not be null");


            // Imprimir el tipo y valor del resultado para depuración

            Console.WriteLine($"Result type: {result.Value.GetType()}");

            Console.WriteLine($"Result value: {Newtonsoft.Json.JsonConvert.SerializeObject(result.Value)}");


            if (result.Value is IDictionary<string, object> jsonDictionary)

            {

                Assert.IsTrue(jsonDictionary.ContainsKey("success"), "JSON should contain 'success' key");

                Assert.IsTrue((bool)jsonDictionary["success"], "Success should be true");

                Assert.IsTrue(jsonDictionary.ContainsKey("message"), "JSON should contain 'message' key");

                Assert.AreEqual("Guardado automático realizado.", jsonDictionary["message"]);

            }

            else if (result.Value is object anonymousObject)

            {

                // Si es un objeto anónimo, accedemos a sus propiedades mediante reflexión

                var successProperty = anonymousObject.GetType().GetProperty("success");

                var messageProperty = anonymousObject.GetType().GetProperty("message");


                Assert.IsNotNull(successProperty, "JSON should contain 'success' property");

                Assert.IsTrue((bool)successProperty.GetValue(anonymousObject), "Success should be true");

                Assert.IsNotNull(messageProperty, "JSON should contain 'message' property");

                Assert.AreEqual("Guardado automático realizado.", messageProperty.GetValue(anonymousObject));

            }

            else

            {

                Assert.Fail($"Unexpected result type: {result.Value.GetType()}");

            }


            _mockUnitWork.Verify(u => u.diary.AgregarAsync(It.IsAny<Diary>()), Times.Once);

            _mockUnitWork.Verify(u => u.GuardarAsync(), Times.Once);

        }

    }

}