angular.module("umbraco").controller("uSupport.history.contentApp.controller", function ($scope) {
    'use strict';

    var vm = this;

    vm.getStatusName = function (guid) {
        return $scope.model.statuses.find(function (s) {
            return  s.Id === guid;
        }).Name;
    };

    vm.getTypeName = function (guid) {
        return $scope.model.types.find(function (s) {
            return s.Id === guid;
        }).Name;
    };

    vm.getBadgeColor = function (actionType) {
        var type = actionType.toLowerCase();

        if (type === "created") {
            return 'secondary';
        } else if (type === 'update' || type === 'updated') {
            return 'gray';
        } else if (type === 'resolve' || type === 'resolved') {
            return 'success';
        }
    };
});