// Función para eliminar el contacto Messenger
function deleteMessenger(messengerId) {
    swal({
        title: "¿Estás seguro de eliminar este Contacto?",
        text: "Este registro no se podrá recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            const token = $('input[name="__RequestVerificationToken"]').val();

            $.ajax({
                type: "POST",
                url: "/Messengers/Delete",
                data: {
                    id: messengerId,
                    __RequestVerificationToken: token
                },
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        setTimeout(function () {
                            location.href = '/Messengers/Index';
                        }, 500);
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("Error al intentar eliminar el contacto.");
                }
            });
        }
    });
}
