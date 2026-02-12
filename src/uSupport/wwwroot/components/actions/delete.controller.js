angular.module("umbraco").controller("uSupport.delete.controller", function ($scope, navigationService, notificationsService, $location, uSupportActionResources) {

	var vm = this;
	var dto = {
		type: $scope.currentNode.parentId,
		id: $scope.currentNode.id
	};

	uSupportActionResources.previewDelete(dto).then(function (children) {
		if (children.length > 0) {
			vm.children = children;
		}
	});

	vm.delete = function () {
		vm.buttonState = "busy";

		uSupportActionResources.delete(dto).then(function (response) {
			vm.buttonState = "success";
			navigationService.hideDialog();
			notificationsService.success("uSupport", $scope.currentNode.name + " was deleted successfully!");
			$location.path($scope.currentNode.metaData["overviewRoutePath"]);
		});
	}

	vm.open = function (item) {
		navigationService.hideDialog();
		$location.path("uSupport/tickets/edit/" + item.Id);
	};

	vm.close = function () {
		navigationService.hideDialog();
	};
});