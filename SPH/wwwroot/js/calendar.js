// calendar.js
$(function () {
    var events = JSON.parse($('#events-data').val());
    var expandedEvents = [];

    // Función para manejar los eventos expandidos
    function expandEvents(events) {
        events.forEach(function (event) {
            var start = moment(event.start);
            var end = moment(event.end);

            // Si el evento dura varios días, creamos eventos para cada día
            if (start.isBefore(end, 'day')) {
                var current = start.clone();

                // Agregar el primer segmento (desde la hora de inicio hasta el final del primer día)
                expandedEvents.push({
                    title: event.title,
                    start: current.clone(),
                    end: current.clone().endOf('day'),
                    color: event.color,
                    id: event.id,
                    allDay: false
                });

                current.add(1, 'days');

                // Agregar los segmentos intermedios como eventos de todo el día
                while (current.isBefore(end, 'day')) {
                    expandedEvents.push({
                        title: event.title,
                        start: current.clone().startOf('day'),
                        end: current.clone().endOf('day'),
                        color: event.color,
                        id: event.id,
                        allDay: true
                    });
                    current.add(1, 'days');
                }

                // Agregar el último segmento (desde el inicio del último día hasta la hora de finalización)
                expandedEvents.push({
                    title: event.title,
                    start: current.clone().startOf('day'),
                    end: end.clone(),
                    color: event.color,
                    id: event.id,
                    allDay: false
                });
            } else {
                // Si el evento es de un solo día, agregarlo directamente
                expandedEvents.push(event);
            }
        });
    }

    expandEvents(events); // Llamar a la función para expandir los eventos

    console.log(expandedEvents); // Para depuración: muestra los eventos expandidos en la consola

    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        defaultView: 'month',
        editable: true,
        events: expandedEvents,
        eventClick: function (event) {
            window.location.href = '/Calendaries/Detail/' + event.id;
        },
        displayEventTime: true, // Mostrar la hora del evento si está definida
        allDaySlot: true, // Mostrar eventos que ocupan todo el día en la vista diaria y semanal
        allDayText: 'Todo el día'
    });

    // Función para eliminar el evento
    window.deleteEvent = function (eventId) {
        swal({
            title: "¿Estás seguro de eliminar este evento?",
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
                    url: "/Calendaries/Delete/" + eventId,
                    data: {
                        __RequestVerificationToken: token
                    },
                    success: function (data) {
                        if (data.success) {
                            toastr.success(data.message);
                            // Redirigir al index del calendario después de mostrar el mensaje
                            setTimeout(function () {
                                window.location.href = '/Calendaries/Index';
                            }, 1000); // Redirigir después de 1 segundo
                        } else {
                            toastr.error(data.message);
                        }
                    },
                    error: function () {
                        toastr.error("Error al intentar eliminar el evento.");
                    }
                });
            }
        });
    };
});
