$(document).ready(function () {
    const events = JSON.parse($('#events-data').val());
    const expandedEvents = expandEvents(events);

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
            window.location.href = `/Calendaries/Detail/${event.id}`;
        },
        displayEventTime: true,
        allDaySlot: true,
        allDayText: 'Todo el día'
    });

    // Expand multi-day events for fullCalendar
    function expandEvents(events) {
        const expandedEvents = [];
        events.forEach(event => {
            const start = moment(event.start);
            const end = moment(event.end);

            if (start.isBefore(end, 'day')) {
                let current = start.clone();

                expandedEvents.push({
                    ...event,
                    start: current.clone(),
                    end: current.clone().endOf('day')
                });

                current.add(1, 'days');

                while (current.isBefore(end, 'day')) {
                    expandedEvents.push({
                        ...event,
                        start: current.clone().startOf('day'),
                        end: current.clone().endOf('day'),
                        allDay: true
                    });
                    current.add(1, 'days');
                }

                expandedEvents.push({
                    ...event,
                    start: current.clone().startOf('day'),
                    end: end.clone()
                });
            } else {
                expandedEvents.push(event);
            }
        });
        return expandedEvents;
    }

    // Delete event confirmation
    window.deleteEvent = function (eventId) {
        swal({
            title: "¿Estás seguro de eliminar este evento?",
            text: "Este registro no se podrá recuperar",
            icon: "warning",
            buttons: true,
            dangerMode: true
        }).then((willDelete) => {
            if (willDelete) {
                const token = $('input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    type: "POST",
                    url: `/Calendaries/Delete/${eventId}`,
                    data: { __RequestVerificationToken: token },
                    success: function (data) {
                        if (data.success) {
                            toastr.success(data.message);
                            setTimeout(() => window.location.href = '/Calendaries/Index', 1000);
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
