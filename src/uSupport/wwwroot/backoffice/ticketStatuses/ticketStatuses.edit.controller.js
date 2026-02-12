angular.module("umbraco").controller("uSupport.ticketStatus.edit.controller", function (
    $scope,
    appState,
    formHelper,
    $location,
    $routeParams,
    uSupportConfig,
    notificationsService,
    uSupportHelperServices,
    uSupportHelperResources,
    uSupportTicketStatusResources) {

    'use strict';

    var vm = this;
    vm.loading = true;
    vm.create = $routeParams.create;

    vm.page = {};

    vm.navigation = [{
        name: "Ticket status",
        alias: "ticketStatus",
        icon: "icon-file-cabinet",
        view: uSupportConfig.basePathAppPlugins + "backoffice/ticketStatuses/apps/ticketStatus/ticketStatus.html",
        active: true
    }];

    vm.page.navigation = vm.navigation;

    if (!vm.create) {
        uSupportTicketStatusResources.getTicketStatus($routeParams.id).then(function (ticketStatus) {
            vm.ticketStatus = ticketStatus;

            uSupportHelperServices.getChildActions("ticketStatuses", "-1", ticketStatus.Id).then(function (currentNode) {
                vm.page = {
                    title: ticketStatus.Name,
                    alias: ticketStatus.Alias,
                    menu: {
                        currentNode: currentNode,
                        currentSection: appState.getSectionState("currentSection")
                    },
                    navigation: vm.navigation
                };

                uSupportHelperResources.getAddons(vm.ticketStatus).then(function (apps) {
                    if (apps.length > 0) {
                        vm.page.navigation = vm.navigation.concat(apps)
                    }

                    vm.loading = false;
                });
            });
        });
    } else {
        vm.ticketStatus = {};
        vm.loading = false;
    }

    vm.save = function () {
        vm.buttonState = "busy";

        if (formHelper.submitForm({ scope: $scope })) {

            vm.ticketStatus.Name = vm.page.title;
            vm.ticketStatus.Alias = vm.page.alias;

            vm.properties.forEach(function (property) {
                if (property.alias === "Color") {
                    vm.ticketStatus[property.alias] = property.singleDropdownValue
                } else {
                    vm.ticketStatus[property.alias] = parseInt(+property.value)
                }
            });

            if (!vm.create) {
              
                uSupportTicketStatusResources.updateTicketStatus(vm.ticketStatus).then(function () {
                    vm.buttonState = "success";
                    $scope.formName.$dirty = false;
                    notificationsService.success("uSupport", "Ticket status was updated successfully!");
                    vm.close();
                }, function (err) {
                    if (err.data && (err.data.message || err.data.Detail)) {
                        notificationsService.error("uSupport", err.data.message ?? err.data.Detail);
                    }
                    vm.buttonState = "error";
                });
            } else {
                uSupportTicketStatusResources.createTicketStatus(vm.ticketStatus).then(function (status) {
                    vm.buttonState = "success";
                    $scope.formName.$dirty = false;
                    notificationsService.success("uSupport", "A new ticket status was created successfully!");
                    vm.close();
                }, function (err) {
                    if (err.data && (err.data.message || err.data.Detail)) {
                        notificationsService.error("uSupport", err.data.message ?? err.data.Detail);
                    }
                    vm.buttonState = "error";
                });
            }
        } else {
            vm.buttonState = "error";
            notificationsService.error("uSupport", "Failed to create ticket status");
        }
    };

    vm.close = function () {
        $location.url("/uSupport/ticketStatuses/overview");
    };
});