angular.module("umbraco").controller("uSupport.editUser.controller", function ($scope, notificationsService, uSupportHelperResources, umbRequestHelper, mediaHelper, Upload) {

    var vm = this;
    vm.loading = true;

    uSupportHelperResources.getUserById($scope.model.userId).then(function (user) {
        vm.user = user;
        vm.properties = [
            {
                alias: "name",
                label: "Name",
                view: "textbox",
                value: user.name,
                validation: {
                    mandatory: true,
                    //  pattern: ""
                }
            },
            {
                alias: "email",
                label: "Email",
                view: "textbox",
                value: user.email,
                validation: {
                    mandatory: true,
                    //  pattern: ""
                }
            },
        ];

        vm.loading = false;
    });

    vm.avatarFile = {
        filesHolder: null,
        uploadStatus: null,
        uploadProgress: 0,
        maxFileSize: Umbraco.Sys.ServerVariables.umbracoSettings.maxFileSize + "KB",
        acceptedFileTypes: mediaHelper.formatFileTypes(Umbraco.Sys.ServerVariables.umbracoSettings.imageFileTypes),
        uploaded: false
    };

    vm.changeAvatar = function (files, event) {
        if (files && files.length > 0) {
            upload(files[0]);
        }
    };

    vm.clearAvatar = function () {
        uSupportHelperResources.clearAvatar(vm.user).then(function (data) {
            vm.user.avatars = data;
        });
    };

    function upload(file) {

        vm.avatarFile.uploadProgress = 0;

        Upload.upload({
            url: umbRequestHelper.getApiUrl("currentUserApiBaseUrl", "PostSetAvatar"),
            fields: {},
            file: file
        }).progress(function (evt) {

            if (vm.avatarFile.uploadStatus !== "done" && vm.avatarFile.uploadStatus !== "error") {
                vm.avatarFile.uploadStatus = "uploading";
                var progressPercentage = parseInt(100.0 * evt.loaded / evt.total, 10);
                vm.avatarFile.uploadProgress = progressPercentage;
            }

        }).success(function (data, status, headers, config) {
            vm.avatarFile.uploadProgress = 100;
            vm.avatarFile.uploadStatus = "done";
            vm.user.avatars = data;
            vm.avatarFile.uploaded = true;
        }).error(function (evt, status, headers, config) {
            vm.avatarFile.uploadStatus = "error";
            if (status === 404) {
                vm.avatarFile.serverErrorMessage = "File not found";
            } else if (status == 400) {
                vm.avatarFile.serverErrorMessage = evt.message;
            } else {
                if (evt.InnerException) {
                    vm.avatarFile.serverErrorMessage = evt.InnerException.ExceptionMessage;
                    if (evt.InnerException.StackTrace && evt.InnerException.StackTrace.indexOf("ValidateRequestEntityLength") > 0) {
                        vm.avatarFile.serverErrorMessage = "File too large to upload";
                    }
                } else if (evt.Message) {
                    vm.avatarFile.serverErrorMessage = evt.Message;
                }
            }
        });
    };

    vm.save = function () {
        vm.buttonState = "busy";

        vm.userSave = {
            Id: vm.user.id,
            Name: vm.properties[0].value,
            Email: vm.properties[1].value,
        };

        uSupportHelperResources.saveUser(vm.userSave).then(function (updatedUser) {
            if (updatedUser !== null) {
                vm.buttonState = "success";
                if ($scope.model.submit) {
                    $scope.model.submit($scope.model);
                }
            } else {
                notificationsService.error("uSupport", "Failed to update profile");
                vm.buttonState = "error";
            }
        });



    };

    vm.close = function () {
        if ($scope.model.close) {
            $scope.model.close();
        }
    };
});