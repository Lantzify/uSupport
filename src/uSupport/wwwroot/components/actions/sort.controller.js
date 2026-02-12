angular.module("umbraco").controller("uSupport.sort.controller", function ($filter, $route, $scope, navigationService, notificationsService, uSupportActionResources) {

	var vm = this;
	var dto = {};

	vm.loading = true;
	vm.children = [];
	vm.sortOrder = {};

	uSupportActionResources.getChildren($scope.currentNode.metaData["type"]).then(function (children) {
		vm.loading = false;
		vm.children = children;
	});

	vm.sort = function () {
		dto.list = vm.children;
		dto.type = $scope.currentNode.metaData["type"];

		vm.buttonState = "busy";

		uSupportActionResources.sort(dto).then(function () {
			vm.buttonState = "success";
			notificationsService.success("uSupport", "Successfully sorted!");
			navigationService.hideDialog();
			$route.reload();
		}, function (error) {
			vm.error = error;
			vm.buttonState = "error";
		});
	};

	vm.order = function (columnName) {
		if (vm.sortOrder.column === columnName) {
			!vm.sortOrder.reverse
		} else {
			vm.sortOrder.column = columnName;
			vm.sortOrder.reverse = false;
		}
		vm.children = $filter('orderBy')(vm.children, vm.sortOrder.column, !vm.sortOrder.reverse);
	};

	vm.close = function () {
		navigationService.hideDialog();
	};
});