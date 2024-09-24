function Delete(url, id) {
    swal({
        title: "¿Está seguro de eliminar esta Tarea?",
        text: "Este registro no se podrá recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "POST",
                url: url, // URL de la acción en tu controlador
                data: {
                    id: id
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
                    toastr.error("Error al intentar eliminar la tarea.");
                }
            });
        }
    });
}
