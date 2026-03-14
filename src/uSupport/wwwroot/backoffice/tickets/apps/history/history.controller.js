angular.module("umbraco").controller("uSupport.history.contentApp.controller", function ($scope, uSupportTicketResources) {
    'use strict';

    var vm = this;

    vm.history = $scope.model.history.Items;
    vm.pagination = {
        pageNumber: 1,
        totalPages: $scope.model.history.TotalPages
    };

    vm.loadHistory = function (pageNumber) {
        uSupportTicketResources.getPagedHistoryByTicketId($scope.model.ticket.Id, pageNumber).then(function (history) {
            vm.history = history.Items;
            vm.pagination = {
                pageNumber: pageNumber,
                totalPages: history.TotalPages
            };
        });
    };

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
        } else if (type === 'comment' || type === 'resolved') {
            return 'success';
        }
    };
});