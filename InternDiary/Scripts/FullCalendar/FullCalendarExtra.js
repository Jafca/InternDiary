$(function () {
    $('#calendar').fullCalendar({
        firstDay: "1",
        events: {
            url: '/Entry/GetCalendarEntries',
            type: 'POST',
            error: function () {
                console.log('There was an error while fetching events!');
            }
        },
        dayClick: function (date) {
            window.location.href = "/Entry/Create?date=" + date.format();
        }
    })
});