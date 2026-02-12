(function () {
    'use strict';
    angular.module("umbraco.resources").factory("uSupportTicketResources", function ($http, umbRequestHelper, uSupportConfig) {
        return {
            //Ticket Api
            createTicket: function (ticket,) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/CreateTicket",
                        data: ticket
                            
                    })
                );
            },
            updateTicket: function (ticket, sendEmail) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/UpdateTicket",
                        data: {
                            ticket,
                            sendEmail
                        }
                    })
                );
            },
            getTicket: function (ticketId) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/GetTicket?ticketId=" + ticketId)
                );
            },
            getPagedActiveTickets: function (page) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/GetPagedActiveTickets?page=" + page)
                );
            },
            getPagedResolvedTickets: function (page) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/GetPagedResolvedTickets?page=" + page)
                );
            },
            anyResolvedTickets: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/AnyResolvedTickets")
                );
            },
        };
    });
})();