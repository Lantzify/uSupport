angular.module("umbraco").component("activeTicketsList", {
    templateUrl: "/App_Plugins/uSupport/components/tickets/activeTicketsListComponent.html",
    controllerAs: "vm",
    bindings: {
        adminDashboard: "@",
        useLongList: "="
    },
    controller: function (uSupportTicketResources, userService, uSupportConfig, overlayService, $location) {
        var vm = this;
        vm.useLongList = vm.useLongList === "true";

        vm.$onInit = function () {
            (vm.loadPage = function (pageNumber) {
                uSupportTicketResources.getPagedActiveTickets(pageNumber).then(function (tickets) {
                    vm.activeTickets = tickets.Items;
                    vm.pagination = {
                        pageNumber: pageNumber,
                        totalPages: tickets.TotalPages
                    };
                });
            })(1);

            userService.getCurrentUser().then(function (user) {
                if (user.allowedSections.indexOf("uSupport") === -1) {
                    vm.openTicket = function (id) {
                        uSupportTicketResources.getTicket(id).then(function (ticket) {
                            var options = {
                                view: uSupportConfig.basePathAppPlugins + "components/overlays/openTicket.html",
                                title: ticket.Title,
                                subtitle: "Ticket details",
                                ticket: ticket,
                                hideSubmitButton: true,
                                close: function () {
                                    overlayService.close();
                                }
                            };

                            overlayService.open(options);
                        });
                    };
                } else {
                    vm.openTicket = function (ticketId) {
                        $location.path("/uSupport/tickets/edit/" + ticketId);
                    };
                }
            });
        }
    }
});