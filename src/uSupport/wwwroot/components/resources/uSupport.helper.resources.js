(function () {
    'use strict';
    angular.module("umbraco.resources").factory("uSupportHelperResources", function ($http, umbRequestHelper, uSupportConfig) {
        return {
            clearAvatar: function (user) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportHelperAuthorizedApi/ClearAvatar",
                        data: user
                    })
                );
            },
            saveUser: function (user) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportHelperAuthorizedApi/SaveUser",
                        data: user
                    })
                );
            },
            getUserById: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportHelperAuthorizedApi/GetUserById?id=" + id)
                );
            },
            generateExternalTicketId: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportHelperAuthorizedApi/GenerateExternalTicketId")
                );
            },
            readOnlyValue: function (value) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportHelperAuthorizedApi/ReadOnlyValue",
                        data: value
                    })
                );
            },
            getAddons: function (type) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportHelperAuthorizedApi/GetAddons",
                        data: type
                    })
                );
            }
        };
    });
})();