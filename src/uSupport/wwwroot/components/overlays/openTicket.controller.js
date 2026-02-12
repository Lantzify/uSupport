angular.module("umbraco").controller("uSupport.openTicket.controller", function (
    $q,
    $scope,
    uSupportConfig,
    uSupportHelperResources,
    uSupportTicketCommentResources,
    uSupportTicketTypeResources) {

    var vm = this;
    vm.ticket = $scope.model.ticket;

    vm.properties = [
        {
            alias: "ticketId",
            label: "Ticket id",
            view: "readonlyvalue",
            value: vm.ticket.ExternalTicketId
        }
    ];

    if (vm.ticket.Type.PropertyView !== null) {
        uSupportTicketTypeResources.getDataTypeFromId(vm.ticket.Type.PropertyId).then(function (dataType) {
            vm.customProperties = [{
                alias: vm.ticket.Type.Alias,
                label: vm.ticket.Type.PropertyName,
                view: uSupportConfig.basePathAppPlugins + "components/propertyEditors/readonly/readonly.html",
                value: vm.ticket.PropertyValue,
                config: dataType.Configuration
            }];
        });
    }

    $q.all({
        author: uSupportHelperResources.getUserById(vm.ticket.AuthorId),
        comments: uSupportTicketCommentResources.getCommentsFromTicketId(vm.ticket.Id)
    }).then(function (promises) {

        vm.user = promises.author;

        vm.properties.push({
            alias: "author",
            label: "Author",
            view: "readonlyvalue",
            value: promises.author.name
        });

        if (vm.ticket.LastUpdatedBy !== null) {
            vm.properties.push({
                alias: "lastUpdatedBy",
                label: "Last updated by",
                view: "readonlyvalue",
                value: vm.ticket.LastUpdatedBy
            })
        }

        vm.properties.push(
            {
                alias: "ticketTitle",
                label: "Ticket title",
                view: "readonlyvalue",
                value: vm.ticket.Title
            },
            {
                alias: "ticketSummary",
                label: "Ticket summary",
                view: "readonlyvalue",
                value: vm.ticket.Summary
            },
        );

        $scope.model.comments = promises.comments;
    });
});