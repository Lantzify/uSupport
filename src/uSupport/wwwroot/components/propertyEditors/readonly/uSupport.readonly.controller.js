angular.module("umbraco").controller("uSupport.readonly.controller", function ($scope, $location, overlayService, uSupportHelperResources) {
    'use strict';

    uSupportHelperResources.readOnlyValue($scope.model.value).then(function (entityValue) {
        if (entityValue !== null) {
            $scope.nodes = entityValue;
        }
    });

    console.log($scope.model)

    $scope.openEditor = function (node) {
        if ($scope.model.admin) {
            var overlay = {
                title: "Are you sure?",
                content: "You are about navigate away from this page. Are you sure?",
                submit: function () {
                    overlayService.close();

                    if (node.MediaPath !== undefined) {
                        $location.url("/media/media/edit/" + node.Id)
                    } else {
                        $location.path("/content/content/edit/" + node.Id)
                    }                  
                }
            };
            overlayService.confirm(overlay)
        } else {
            if (node.MediaPath !== undefined) {
                $location.url("/media/media/edit/" + node.Id)
            } else {
                $location.path("/content/content/edit/" + node.Id)
            }

            overlayService.close();
		}
	}
});