$(function () {
    $('#calendar').fullCalendar({
        firstDay: "1",
        events: {
            url: '/Entry/GetCalendarEntries',
            type: 'POST',
            error: function () {
                console.log('There was an error while fetching events!');
            }
        }
    })
});