$(function () {
    $('#calendar').fullCalendar({
        firstDay: '1',
        events: {
            url: '/Entry/GetCalendarEntries',
            type: 'POST',
            error: function () {
                console.log('There was an error while fetching events!');
            }
        },
        dayClick: function (date) {
            window.location.href = '/Entry/Create?date=' + date.format();
        },
        customButtons: {
            toggleWeekends: {
                text: 'Toggle Weekends',
                click: function () {
                    toggleWeekends();
                }
            }
        },
        header: {
            left: 'title',
            center: 'prev,today,next',
            right: 'toggleWeekends'
        },
        weekends: localStorage.getItem('weekends') == 'true'
    })
});

function toggleWeekends() {
    showWeekends = $('#calendar').fullCalendar('option', 'weekends');
    localStorage.setItem('weekends', 'true');

    if (showWeekends)
        localStorage.setItem('weekends', 'false');

    $('#calendar').fullCalendar('option', {
        weekends: !showWeekends
    });
}