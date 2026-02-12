angular.module("umbraco").controller("uSupport.ticket.contentApp.controller", function (
    $scope,
    uSupportConfig) {

    'use strict';
  
    $scope.model.properties = [
        {
            alias: "ticketTitle",
            label: "Ticket title",
            view: "readonlyvalue",
            value: $scope.model.ticket.Title
        },
        {
            alias: "ticketSummary",
            label: "Ticket summary",
            view: "readonlyvalue",
            value: $scope.model.ticket.Summary
        },
        {
            alias: "type",
            label: "Type",
            view: "dropdownFlexible",
            value: $scope.model.ticket.Type.Name,
            config: {
                items: $scope.model.typesNamesArr
            },
            validation: {
                mandatory: true,
                pattern: ""
            },
        }
    ];

    if ($scope.model.ticket.Type.PropertyView !== null) {
        $scope.model.properties.push({
            alias: $scope.model.ticket.Type.PropertyAlias,
            label: $scope.model.ticket.Type.PropertyName,
            view: uSupportConfig.basePathAppPlugins + "components/propertyEditors/readonly/readonly.html",
            value: $scope.model.ticket.PropertyValue,
            admin: true
        });
    }

    $scope.model.properties.push({
        alias: "status",
        label: "Status",
        view: "dropdownFlexible",
        value: $scope.model.ticket.Status.Name,
        config: {
            items: $scope.model.statusNamesArr
        },
        validation: {
            mandatory: true,
            pattern: ""
        },
    });
});