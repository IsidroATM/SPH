function Delete(messengerId) {
    swal({
        title: "¿Está seguro de eliminar este Contacto?",
        text: "Este registro no se podrá recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            const token = document.querySelector('#deleteForm input[name="__RequestVerificationToken"]').value;

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
