angular.module("umbraco").controller("uSupport.createTicket.controller", function (eventsService, $scope, userService, uSupportHelperResources, uSupportTicketTypeResources) {

    var vm = this;

    vm.step = 1;
    $scope.model.ticket = {};
    $scope.model.complete = false;
    $scope.model.process = process;
    vm.selectedTicketTypeName = "dummyValue";

    uSupportTicketTypeResources.getAllTicketTypes().then(function (ticketTypes) {
        vm.ticketTypes = ticketTypes;
        
    });

    vm.selectTicketType = function (ticketType) {
        vm.selectedTicketType = ticketType;
        vm.selectedTicketTypeName = ticketType.Name;
        $scope.model.disableSubmitButton = false;
        $scope.model.ticket["typeId"] = ticketType.Id;
    };

    function process() {
        vm.step += vm.selectedTicketType.PropertyId !== 0 ? 1 : 2;

        switch (vm.step) {
            case 2:
                vm.loadingProperty = true;
                uSupportTicketTypeResources.getDataTypeFromId(vm.selectedTicketType.PropertyId).then(function (dataType) {
                    var unsubscribeOpen = eventsService.on("appState.editors.open", function () {
                        document.getElementsByClassName("umb-overlay")[0].style.zIndex = "7499";
                    });

                    var unsubscribeClose = eventsService.on("appState.editors.close", function () {
                        document.getElementsByClassName("umb-overlay")[0].style.zIndex = "7500";
                    });

                    $scope.model.title = vm.selectedTicketTypeName;
                    $scope.model.subtitle = vm.selectedTicketType.PropertyDescription;
                    vm.dataTypeConfiguration = dataType.Configuration;

                    if (dataType.EditorAlias === "Umbraco.MediaPicker3") {
                        dataType.Configuration.validationLimit = {
                            max: dataType.Configuration.ValidationLimit.max,
                            min: dataType.Configuration.ValidationLimit.min
                        };
                    }

                    $scope.model.property = {
                        editor: dataType.EditorAlias,
                        alias: "property",
                        view: vm.selectedTicketType.PropertyView,
                        config: vm.dataTypeConfiguration,
                        validation: {
                            mandatory: true,
                            pattern: ""
                        },
                    };

                    if (vm.dataTypeConfiguration.Items !== undefined) {
                        vm.dataTypeConfiguration.items = vm.dataTypeConfiguration.Items.map(x => x.value);
                        $scope.model.property.value = vm.dataTypeConfiguration.items[0];
                    }

                    vm.loadingProperty = false;

                    $scope.$on("$destroy", function () {
                        unsubscribeOpen();
                        unsubscribeClose();
                    });
                });
                break;
            case 3:
                $scope.model.title = "Ticket details";
                $scope.model.subtitle = "Enter ticket details";
                $scope.model.submitButtonLabel = "Create ticket";
                $scope.model.complete = true;

                userService.getCurrentUser().then(function (user) {

                    $scope.model.ticket["authorId"] = user.id;

                    $scope.model.properties = [
                        {
                            alias: "ticketTitle",
                            label: "Ticket title",
                            description: "Enter a appropriate title",
                            view: "textbox",
                            validation: {
                                mandatory: true,
                                pattern: ""
                            }
                        },
                        {
                            alias: "ticketSummary",
                            label: "Ticket summary",
                            description: "Try to explain the issue as clearly as possible",
                            view: "textarea",
                            validation: {
                                mandatory: true,
                                pattern: ""
                            }
                        },
                        {
                            alias: "type",
                            label: "Type",
                            value: vm.selectedTicketType.Name,
                            view: "readonlyvalue"
                        },
                    ];

                    if ($scope.model.property !== undefined) {
                        $scope.model.properties.push({
                            alias: "property",
                            label: vm.selectedTicketType.PropertyName,
                            description: vm.selectedTicketType.PropertyDescription,
                            view: vm.selectedTicketType.PropertyView,
                            value: $scope.model.property.value,
                            config: vm.dataTypeConfiguration,
                            validation: {
                                mandatory: true,
                                pattern: ""
                            },
                            allowRemoveMedia: false
                        });

                        $scope.model.readonly = true;
                        $scope.readonly = true;

                        $scope.model.ticket["propertyValue"] = JSON.stringify($scope.model.properties[3].value);
                    }

                    uSupportHelperResources.generateExternalTicketId().then(function (externalTicketId) {
                        $scope.model.properties.push(
                            {
                                alias: "author",
                                label: "Author",
                                value: user.name,
                                view: "readonlyvalue"
                            },
                            {
                            alias: "ticketId",
                            label: "Ticket id",
                            value: externalTicketId,
                            view: "readonlyvalue"
                        });

                        $scope.model.ticket["externalTicketId"] = externalTicketId;
                    });
                });
                break;
        }
    }
});