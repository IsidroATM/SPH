function Delete(noteId) {
    swal({
        title: "¿Está seguro de eliminar esta Nota?",
        text: "Este registro no se podrá recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            // Obtener el token de verificación
            const token = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                type: "POST",
                url: "/Diaries/Delete", // Asegúrate de que esta URL sea correcta
                data: {
                    id: noteId,
                    __RequestVerificationToken: token // Incluir el token en los datos de la solicitud
                },
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        setTimeout(function () {
                            location.reload(); // Recargar la página para reflejar los cambios
                        }, 500);
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("Error al intentar eliminar la nota.");
                }
            });
        }
    });
}
