(function () {
    'use strict';
    angular.module("umbraco.services").factory("uSupportHelperServices", function (navigationService, treeService) {
        return {
            getChildActions: function (tree, path, childId) {
                return navigationService.syncTree({ tree: tree, path: path }).then(function (data) {
                    return treeService.getChildren({ node: data.node }).then(function (treeChildren) {
                        return treeChildren.find(function (node) {
                            return node.id === childId;
                        });
                    })
                });
			}
        };
    });
})();