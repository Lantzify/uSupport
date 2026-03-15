(function () {
    'use strict';
    angular.module("umbraco.resources").factory("uSupportSettingsResources", function ($http, umbRequestHelper, uSupportConfig) {
        return {
            getSettings: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportSettingsAuthorizedApi/GetSettings")
                );
            },
            updateSettings: function (settings) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportSettingsAuthorizedApi/UpdateSettings",
                        data: settings
                        
                    })
                );
            },
        };
    });
})();