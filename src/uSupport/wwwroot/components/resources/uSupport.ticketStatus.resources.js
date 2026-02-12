(function () {
    'use strict';
    angular.module("umbraco.resources").factory("uSupportTicketStatusResources", function ($http, umbRequestHelper, uSupportConfig) {
        return {
            createTicketStatus: function (ticketStatus) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketStatusAuthorizedApi/CreateTicketStatus",
                        data: ticketStatus
                    })
                );
            },
            updateTicketStatus: function (ticketStatus) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketStatusAuthorizedApi/UpdateTicketStatus",
                        data: ticketStatus
                    })
                );
            },
            getTicketStatuses: function (ids) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketStatusAuthorizedApi/GetTicketStatuses",
                        data: ids
                    })
                );
            },
            getTicketStatus: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketStatusAuthorizedApi/GetTicketStatus?id=" + id)
                );
            },
            getAllTicketStatuses: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketStatusAuthorizedApi/GetAllTicketStatuses")
                );
            },
            getStatusIdFromName: function (statusName) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketStatusAuthorizedApi/GetStatusIdFromName?statusName=" + statusName)
                );
            }
        };
    });
})();