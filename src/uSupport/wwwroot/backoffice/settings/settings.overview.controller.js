angular.module("umbraco").controller("uSupport.settings.overview.controller", function (
    $scope,
    formHelper,
    uSupportConfig,
    notificationsService,
    uSupportHelperResources,
    uSupportSettingsResources) {

    var vm = this;
    vm.loading = true;
    vm.buttonState = "init";

    vm.page = {
        title: "Settings",
        description: "A collection of all uSupports settings",
    };

    vm.navigation = [{
        name: "Settings",
        alias: "settings",
        icon: "icon-document",
        view: uSupportConfig.basePathAppPlugins + "backoffice/settings/apps/settings/settings.html",
        active: true
    }]

    uSupportSettingsResources.getSettings().then(function (settings) {

        vm.settings = settings;
        vm.page.navigation = vm.navigation;

        uSupportHelperResources.getAddons(settings).then(function (apps) {
            if (apps.length > 0) {
                vm.page.navigation = vm.page.navigation.concat(apps)
            }

            vm.loading = false;
        });

    });

    vm.save = function () {
        vm.buttonState = "busy";
        if (formHelper.submitForm({ scope: $scope })) {

            let settings = vm.properties.reduce(function (obj, property) {
                obj[property.alias] = property.value;
                return obj;
            }, {});

            settings["Id"] = vm.settings.Id

            uSupportSettingsResources.updateSettings(settings).then(function () {
                vm.buttonState = "success";
                notificationsService.success("uSupport", "successfully updated settings!");
                $scope.formName.$dirty = false;

            }, function (err) {
                if (err.data && (err.data.message || err.data.Detail)) {
                    notificationsService.error("uSupport", err.data.message ?? err.data.Detail);
                }
                vm.buttonState = "error";
            });
        } else {
            vm.buttonState = "error";
        }
    };
});
