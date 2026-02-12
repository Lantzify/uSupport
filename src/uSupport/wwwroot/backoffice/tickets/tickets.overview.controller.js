angular.module("umbraco").controller("uSupport.tickets.overview.controller", function ($location) {

    var vm = this;

    vm.page = {
        title: "Tickets",
        description: "A collection for all support tickets"
    };

    vm.openTicket = function (id) {
        $location.path("/uSupport/tickets/edit/" + id);
    };
});