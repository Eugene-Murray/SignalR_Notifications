/// <reference path="../_references.js" />



var homeIndexViewModel = {
    notificationList: ko.observableArray(),
    hoverItem: ko.observable(),



    animateBackgroundColour: function (elem) {
        console.log(elem);
        if (elem.nodeType === 1) {

            $(elem).find('div').animate({
                backgroundColor: "#dddddd"
            }, 5000);
        }
    },

    onMouseOver: function () {
        $('#divNotificationDetails').show();
        homeIndexViewModel.hoverItem(this);
    },

    onMouseOut: function () {
        $('#divNotificationDetails').hide();
        homeIndexViewModel.hoverItem(null);
    }
};

ko.applyBindings(homeIndexViewModel);


var notifications;
$(function () {
    notifications = $.connection.notificationsHub;

    $.connection.hub.start(function () {
        console.log(notifications);

        notifications.recentNotifications().done(function (data) {
            console.log(data);
            $.each(data, function (index, value) {
                console.log(value);
                homeIndexViewModel.notificationList.push({ id: value.Id, importance: value.Importance, title: value.Title, description: value.Description, timeCreated: value.TimeCreated });
            });
        });
    });

    notifications.addNotification = function (data) {
        console.log(data);
        homeIndexViewModel.notificationList.pop();
        homeIndexViewModel.notificationList.unshift({ id: data.Id, importance: data.Importance, title: data.Title, description: data.Description, timeCreated: data.TimeCreated });
    };

//    $('#divNotificationDetails').hide();

//    $('.divRoundedBordersNewNotification').hover(function () {
//        $('#divNotificationDetails').show();
//        console.log('hover1');
//    }, function () {
//        $('#divNotificationDetails').hide();
//        console.log('stop hover1');
//    });

//    $('.liStyle').hover(function () {
//        $('#divNotificationDetails').show();
//        console.log('hover2');
//    }, function () {
//        $('#divNotificationDetails').hide();
//        console.log('stop hover2');
//    });

});
