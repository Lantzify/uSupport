(function () {
    'use strict';
    angular.module("umbraco.resources").factory("uSupportSettingsResources", function ($http, umbRequestHelper, uSupportConfig) {
        return {
            getSendEmailOnTicketCreatedSetting: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportSettingsAuthorizedApi/GetSendEmailOnTicketCreatedSetting")
                );
            },
            getTicketUpdateEmailSetting: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportSettingsAuthorizedApi/GetTicketUpdateEmailSetting")
                );
            },
            getEmailSubjectNewTicket: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportSettingsAuthorizedApi/GetEmailSubjectNewTicket")
                );
            },
            getEmailSubjectUpdateTicket: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportSettingsAuthorizedApi/GetEmailSubjectUpdateTicket")
                );
            },
            getEmailTemplateNewTicketPath: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportSettingsAuthorizedApi/GetEmailTemplateNewTicketPath")
                );
            },
            getEmailTemplateUpdateTicketPath: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportSettingsAuthorizedApi/GetEmailTemplateUpdateTicketPath")
                );
            },
        };
    });
})();