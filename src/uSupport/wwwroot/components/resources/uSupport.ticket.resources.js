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
            getPagedHistoryByTicketId: function (ticketId, page) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/GetPagedHistoryByTicketId?ticketId=" + ticketId + "&page=" + page)
                );
            },
            getPagedActiveTickets: function (page, searchTerm, sort) {
                var url = uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/GetPagedActiveTickets?page=" + page + "&sortColumnName=" + sort.column + "&sortReverse=" + sort.reverse;

                if (searchTerm) {
                    url += "&searchTerm=" + searchTerm;
                }
                
                return umbRequestHelper.resourcePromise(
                    $http.get(url)
                );
            },
            getPagedResolvedTickets: function (page, searchTerm, sort) {
                var url = uSupportConfig.baseApiUrl + "uSupportTicketAuthorizedApi/GetPagedResolvedTickets?page=" + page + "&sortColumnName=" + sort.column + "&sortReverse=" + sort.reverse;

                if (searchTerm) {
                    url += "&searchTerm=" + searchTerm;
                }
                
                return umbRequestHelper.resourcePromise(
                    $http.get(url)
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