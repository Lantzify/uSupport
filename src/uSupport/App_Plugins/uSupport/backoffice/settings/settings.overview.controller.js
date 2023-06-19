﻿angular.module("umbraco").controller("uSupport.settings.overview.controller", function ($q, uSupportSettingsResources) {

    var vm = this;
    vm.page = {
        title: "Settings",
        description: "A collection of all uSupports settings"
    };

    var appSettings = {
        uSupport: {
            Settings: {
                Tickets: {
                    SendEmailOnTicketCreated: true,
                    TicketUpdateEmail: "None",
                    EmailSubjectNewTicket: "A new ticket has been created",
		            EmailSubjectUpdateTicket: "Your ticket has been updated",
                    EmailTemplateNewTicketPath: "/App_Plugins/uSupport/templates/NewTicketEmail.cshtml",
                    EmailTemplateUpdateTicketPath: "/App_Plugins/uSupport/templates/UpdateTicketEmail.cshtml",
				}
			}
        }
    };

    vm.appSettings = JSON.stringify(appSettings, null, 4);

    vm.web = '<add key="SendEmailOnTicketCreated" value="true" /> \n<add key="TicketUpdateEmail" value="None" /> \n<add key="EmailSubjectNewTicket" value="A new ticket has been created" />\n<add key="EmailSubjectUpdateTicket" value="Your ticket has been updated" /> \n<add key="EmailTemplateNewTicketPath" value="/App_Plugins/uSupport/templates/NewTicketEmail.cshtml" />\n<add key="EmailTemplateUpdateTicketPath" value="/App_Plugins/uSupport/templates/UpdateTicketEmail.cshtml" />';

    $q.all({
        getSendEmailOnTicketCreatedSetting: uSupportSettingsResources.getSendEmailOnTicketCreatedSetting(),
        getTicketUpdateEmailSetting: uSupportSettingsResources.getTicketUpdateEmailSetting(),
        getEmailSubjectNewTicket: uSupportSettingsResources.getEmailSubjectNewTicket(),
        getEmailSubjectUpdateTicket: uSupportSettingsResources.getEmailSubjectUpdateTicket(),
        getEmailTemplateNewTicketPath: uSupportSettingsResources.getEmailTemplateNewTicketPath(),
        getEmailTemplateUpdateTicketPath: uSupportSettingsResources.getEmailTemplateUpdateTicketPath(),
    }).then(function (promises) {
        vm.sendEmailOnTicketCreated = promises.getSendEmailOnTicketCreatedSetting;
        vm.ticketUpdateEmailSetting = promises.getTicketUpdateEmailSetting;
        vm.emailSubjectNewTicket = promises.getEmailSubjectNewTicket;
        vm.emailSubjectUpdateTicket = promises.getEmailSubjectUpdateTicket;
        vm.emailTemplateNewTicketPath = promises.getEmailTemplateNewTicketPath;
        vm.emailTemplateUpdateTicketPath = promises.getEmailTemplateUpdateTicketPath;
    });
});