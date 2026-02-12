angular.module("umbraco").controller("uSupport.ticketStatus.contentApp.controller", function ($scope) {

    'use strict';

    $scope.model.properties = [{
        alias: "Color",
        label: "Color",
        view: "dropdownFlexible",
        value: "primary",
        config: {
            items: [
                "primary",
                "secondary",
                "success",
                "warning",
                "danger",
                "gray"
            ],
        },
        validation: {
            mandatory: true,
            //  pattern: ""
        }
    },
    {
        alias: "Active",
        label: "Active",
        description: "If tickets with this status should count as active",
        view: "boolean",
    },
    {
        alias: "Default",
        label: "Default",
        description: "If all new tickets should have this status as default status. (Only 1 can be default)",
        view: "boolean",

    }];

    $scope.model.preview = {
        property: $scope.model.properties[0],
        page: $scope.model.page
    };

    if (!$scope.model.create) {
        $scope.model.properties.forEach(function (property) {
            property.value = $scope.model.ticketStatus[property.alias];
        });
    }

    $scope.$watch("model.preview", function (newValue, oldValue) {
        $scope.model.selectedColor = newValue.property.singleDropdownValue;
        $scope.model.selectedName = newValue.page.title;
    }, true);
});