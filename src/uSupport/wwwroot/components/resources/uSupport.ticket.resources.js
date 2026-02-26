(function () {
    'use strict';
    angular.module("umbraco.resources").factory("uSupportTicketResources", function ($http, umbRequestHelper, uSupportConfig) {
        return {
            //Ticket Api
            createTicket: function (ticket) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/CreateTicket",
                        data: ticket
                            
                    })
                );
            },
            updateTicket: function (ticket, sendEmail, userId) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/UpdateTicket",
                        data: {
                            ticket,
                            sendEmail,
                            userId
                        }
                    })
                );
            },
            getTicketHistory: function (ticketId) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/GetTicketHistory?ticketId=" + ticketId)
                );
            },
            getTicket: function (ticketId) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/GetTicket?ticketId=" + ticketId)
                );
            },
            getPagedActiveTickets: function (page, searchTerm) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/GetPagedActiveTickets?page=" + page + "&searchTerm=" + searchTerm)
                );
            },
            getPagedResolvedTickets: function (page, searchTerm) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/GetPagedResolvedTickets?page=" + page + "&searchTerm=" + searchTerm)
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