(function () {
    'use strict';
    angular.module("umbraco.resources").factory("uSupportTicketTypeResources", function ($http, umbRequestHelper, uSupportConfig) {
        return {
            createTicketType: function (ticketType) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/CreateTicketType",
                        data: ticketType
                    })
                );
            },
            getTicketTypes: function (ids) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/GetTicketTypes",
                        data: ids
                    })
                );
            },
            updateTicketType: function (ticketType) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/UpdateTicketType",
                        data: ticketType
                    })
                );
            },
            deleteTicketType: function (ticketTypeId) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/DeleteTicketType?ticketTypeId=" + ticketTypeId)
                );
            },
            getAllTicketTypes: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/GetAllTicketTypes")
                );
            },
            getTicketType: function (ticketTypeId) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/GetTicketType?id=" + ticketTypeId)
                );
            },
            getTypeIdFromName: function (name) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/GetTypeIdFromName?name=" + name)
                );
            },
            getAllDataTypes: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/GetAllDataTypes")
                );
            },
            getDataTypeFromId: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/GetDataTypeFromId?id=" + id)
                );
            },
            getDataTypeFromName: function (name) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/GetDataTypeFromName?name=" + name)
                );
            },
            getDataTypeViewFromEditorAlias: function (editorAlias) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketTypeAuthorizedApi/GetDataTypeViewFromEditorAlias?editorAlias=" + editorAlias)
                );
            },
        };
    });
})();