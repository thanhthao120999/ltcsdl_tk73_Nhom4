/// <reference path="E:\ILearning\LTPM\ASP.NET MVC\uStora\uStora\uStora.Web\Assets/libs/angular/angular.js" />
(function (app) {
    app.module('notifications', ['uStora.common']).config(config);
    config.$inject = ['$stateProvider', '$urlRouterProvider'];
    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('notifications', {
                url: "/noti",
                parent: 'base',
                templateUrl: "/app/components/notifications/notificationListView.html",
                controller: "notificationListController"
            });
    }
})();