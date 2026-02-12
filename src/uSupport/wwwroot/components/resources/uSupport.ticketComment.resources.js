(function () {
    'use strict';
    angular.module("umbraco.resources").factory("uSupportTicketCommentResources", function ($http, umbRequestHelper, uSupportConfig) {
        return {
            comment: function (comment) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        method: "POST",
                        url: uSupportConfig.baseApiUrl + "uSupportTicketCommentAuthorizedApi/Comment",
                        data: comment
                    })
                );
            },
            getCommentsFromTicketId: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.get(uSupportConfig.baseApiUrl + "uSupportTicketCommentAuthorizedApi/GetCommentsFromTicketId?ticketId=" + id)
                );
            },
        };
    });
})();