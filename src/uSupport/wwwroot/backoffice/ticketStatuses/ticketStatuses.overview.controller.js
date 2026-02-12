angular.module("umbraco").controller("uSupport.ticketStatus.overview.controller", function (appState, navigationService, treeService, $location, uSupportTicketStatusResources) {

    var vm = this;

    vm.page = {
        title: "Ticket statuses",
        description: "A collection of all ticket statuses",
        menu: {}
    };

    navigationService.syncTree({ tree: "ticketStatuses", path: "-1", forceReload: true }).then(function (syncArgs) {
        vm.page.menu.currentNode = syncArgs.node;
        vm.page.menu.currentSection = appState.getSectionState("currentSection");
    });

    uSupportTicketStatusResources.getAllTicketStatuses().then(function (ticketStatuses) {
        vm.ticketStatuses = ticketStatuses;
    });

    vm.openStatus = function (id) {
        $location.url("/uSupport/ticketStatuses/edit/" + id);
    };
});