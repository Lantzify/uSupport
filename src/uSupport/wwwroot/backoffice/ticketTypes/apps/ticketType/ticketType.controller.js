angular.module("umbraco").controller("uSupport.ticketType.contentApp.controller", function (
    $scope,
    uSupportTicketTypeResources) {

    'use strict';

    $scope.loading = true;

    uSupportTicketTypeResources.getAllDataTypes().then(function (dataTypes) {
        $scope.model.dataTypes = dataTypes;
        $scope.model.properties = [
            {
                alias: "propertyName",
                label: "Property name",
                view: "textbox"
            },
            {
                alias: "propertyDescription",
                label: "Property description",
                view: "textbox"
            },
            {
                alias: "property",
                label: "Property",
                view: "dropdownFlexible",
                config: {
                    items: dataTypes
                }
            }
        ];

        if ($scope.model.ticketType && $scope.model.ticketType.PropertyId) {
            uSupportTicketTypeResources.getDataTypeFromId($scope.model.ticketType.PropertyId).then(function (type) {
                $scope.model.properties[0].value = $scope.model.ticketType.PropertyName;
                $scope.model.properties[1].value = $scope.model.ticketType.PropertyDescription;
                $scope.model.properties[2].value = type.Name;

                $scope.loading = false;
            });
        } else {
            $scope.loading = false;
        }
    });
});