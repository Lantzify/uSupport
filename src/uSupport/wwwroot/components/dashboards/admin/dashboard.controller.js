angular.module("umbraco").controller("uSupport.dashboard.admin.controller", function ($location, uSupportTicketResources, uSupportConfig) {

    var vm = this;

    vm.page = {
        title: "uSupport",
    };

    uSupportTicketResources.anyResolvedTickets().then(function (response) {
        vm.anyResolvedTickets = response;
    });

    vm.viewAllActiveTickets = function () {
        $location.path("/uSupport/tickets/overview/");
    };
});