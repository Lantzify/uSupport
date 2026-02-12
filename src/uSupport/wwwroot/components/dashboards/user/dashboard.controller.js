angular.module("umbraco").controller("uSupport.dashboard.user.controller", function ($route,
    editorService,
    userService,
    overlayService,
    uSupportHelperResources,
    uSupportTicketResources,
    uSupportConfig,
    notificationsService) {

    var vm = this;

    vm.loading = true;

    vm.createTicket = function () {
        var options = {
            view: uSupportConfig.basePathAppPlugins + "components/overlays/createTicket.html",
            title: "Create ticket",
            subtitle: "Choose what type of problem you're having",
            disableBackdropClick: true,
            disableEscKey: true,
            disableSubmitButton: true,
            submitButtonLabel: "Next",
            closeButtonLabel: "Close",
            submit: function (model) {
                if (model.complete) {
                    model.submitButtonState = "busy";

                    model.ticket["title"] = model.properties[0].value;
                    model.ticket["summary"] = model.properties[1].value;

                    uSupportTicketResources.createTicket(model.ticket).then(function () {
                        overlayService.close();
                        notificationsService.success("uSupport", "A new ticket was created successfully!");
                        $route.reload();
                    }, function (err) {
                        notificationsService.error("uSupport", "Failed to created ticket");
                        model.submitButtonState = "error";
                    });
                }
                else {
                    model.process();
                }
            },
            close: function () {
                overlayService.close();
            }
        };

        overlayService.open(options);
    };

    userService.getCurrentUser().then(function (basicUser) {
        uSupportHelperResources.getUserById(basicUser.id).then(function (user) {
            vm.user = user;
            vm.loading = false;
        });
    });

    vm.editProfile = function () {
        var options = {
            view: uSupportConfig.basePathAppPlugins + "components/overlays/editUser.html",
            size: "large",
            userId: vm.user.id,
            submit: function (model) {
                $route.reload();
                editorService.close();
            },
            close: function () {
                $route.reload();
                editorService.close();
            }
        };

        editorService.open(options);
    };
});