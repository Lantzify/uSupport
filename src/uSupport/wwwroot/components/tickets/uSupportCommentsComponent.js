angular.module("umbraco").component("usupportComments", {
    templateUrl: "/App_Plugins/uSupport/components/tickets/uSupportcommentsComponent.html",
    controllerAs: "vm",
    bindings: {
        comments: "=",
        ticketId: "=",
        userId: "=",
        adminView: "="
    },
    controller: function (uSupportTicketCommentResources) {
        var vm = this;

        vm.addComment = function () {
            vm.commentbuttonState = "busy";
            uSupportTicketCommentResources.comment({
                TicketId: vm.ticketId,
                UserId: vm.userId,
                Comment: vm.comment
            }).then(function (comments) {
                vm.comment = "";
                vm.comments = comments;
                vm.commentbuttonState = "success";
            }, function (err) {
                if (err.data && (err.data.message || err.data.Detail)) {
                    notificationsService.error("uSupport", err.data.message ?? err.data.Detail);
                }

                vm.commentbuttonState = "error";
            });
        };
    }
});