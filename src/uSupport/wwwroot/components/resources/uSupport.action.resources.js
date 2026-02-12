(function () {
    'use strict';
    angular.module("umbraco.resources").factory("uSupportActionResources", function ($http, navigationService, treeService, umbRequestHelper, uSupportConfig) {
        return {
            delete: function (dto) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportActionAuthorizedApi/Delete",
                        data: dto
                    })
                );
            },
            previewDelete: function (dto) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportActionAuthorizedApi/PreviewDelete",
                        data: dto
                    })
                );
            },
            sort: function (dto) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportActionAuthorizedApi/Sort",
                        data: dto
                    })
                );
            },
            getChildren: function (type) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportActionAuthorizedApi/GetChildren?type=" + type)
                );
            }
        };
    });
})();