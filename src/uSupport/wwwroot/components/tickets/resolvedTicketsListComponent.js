angular.module("umbraco").component("resolvedTicketsList", {
    templateUrl: "/App_Plugins/uSupport/components/tickets/resolvedTicketsListComponent.html",
    controllerAs: "vm",
    bindings: {
        adminDashboard: "@",
        useLongList: "="
    },
    controller: function (uSupportTicketResources, uSupportConfig, overlayService, userService, $location) {
        var vm = this;

        vm.$onInit = function () {
            (vm.loadPage = function (pageNumber) {
                uSupportTicketResources.getPagedResolvedTickets(pageNumber).then(function (tickets) {
                    vm.tickets = tickets.Items;
                    vm.pagination = {
                        pageNumber: pageNumber,
                        totalPages: tickets.TotalPages
                    };
                });
            })(1);

            userService.getCurrentUser().then(function (user) {
                if (user.allowedSections.indexOf("uSupport") === -1) {
                    vm.openTicket = function (ticketId) {
                        uSupportTicketResources.getTicket(id).then(function (ticket) {
                            var options = {
                                view: uSupportConfig.basePathAppPlugins + "components/overlays/openTicket.html",
                                title: ticket.Title,
                                subtitle: "Ticket details",
                                ticket: ticket,
                                hideSubmitButton: true,
                                submit: function (model) {
                                    overlayService.close();
                                },
                                close: function () {
                                    overlayService.close();
                                }
                            };

                            overlayService.open(options);
                        });
                    }
                } else {
                    vm.openTicket = function (ticketId) {
                        $location.path("/uSupport/tickets/edit/" + ticketId);
                    };
                }
             });
        }
    }
});