angular.module("umbraco").component("usupportComments", {
    templateUrl: "/App_Plugins/uSupport/components/tickets/uSupportcommentsComponent.html",
    controllerAs: "vm",
    bindings: {
        ticketId: "=",
        userId: "=",
        adminView: "="
    },
    controller: function (uSupportTicketCommentResources, notificationsService) {
        var vm = this;
        vm.comments = []

        vm.$onInit = function () {
            (vm.loadComments = function (pageNumber) {
                uSupportTicketCommentResources.getPagedCommentsForTicket(vm.ticketId, pageNumber).then(function (comments) {
                    vm.comments = comments.Items;
                    vm.pagination = {
                        pageNumber: pageNumber,
                        totalPages: comments.TotalPages
                    };
                });
            })(1);
        }

        vm.addComment = function () {
            vm.commentbuttonState = "busy";
            uSupportTicketCommentResources.comment({
                TicketId: vm.ticketId,
                UserId: vm.userId,
                Comment: vm.comment
            }).then(function () {
                vm.commentbuttonState = "success";
                vm.comment = "";
                vm.$onInit();
            }, function (err) {
                if (err.data && (err.data.message || err.data.Detail)) {
                    notificationsService.error("uSupport", err.data.message ?? err.data.Detail);
                }

                vm.commentbuttonState = "error";
            });
        };
    }
});