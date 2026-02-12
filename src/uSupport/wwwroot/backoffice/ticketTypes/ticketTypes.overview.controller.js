angular.module("umbraco").controller("uSupport.ticketType.overview.controller", function (appState, navigationService, $location, uSupportTicketTypeResources) {

    var vm = this;

    vm.page = {
        title: "Ticket types",
        description: "A collection of all ticket types",
        menu: {}
    };

    navigationService.syncTree({ tree: "ticketTypes", path: "-1", forceReload: false }).then(function (syncArgs) {
        vm.page.menu.currentNode = syncArgs.node;
        vm.page.menu.currentSection = appState.getSectionState("currentSection");
    });

    uSupportTicketTypeResources.getAllTicketTypes().then(function (ticketTypes) {
        vm.ticketTypes = ticketTypes;
    });

    vm.openType = function (id) {
        $location.url("/uSupport/ticketTypes/edit/" + id);
    };
});