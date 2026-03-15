angular.module("umbraco").controller("uSupport.settings.contentApp.controller", function ($scope) {

    'use strict';
  
    $scope.model.properties = [
        {
            alias: "SendEmailOnTicketCreated",
            label: "Send email on ticket created",
            description: "Email will be sent when new ticket is created",
            view: "boolean",
            value: $scope.model.settings.SendEmailOnTicketCreated
        },
        {
            alias: "SendEmailOnTicketComment",
            label: "Send email on ticket comment",
            description: "Email will be sent when new comment is added",
            view: "boolean",
            value: $scope.model.settings.SendEmailOnTicketComment
        },
        {
            alias: "TicketUpdateEmail",
            label: "Ticket update email",
            description: "Email that ticket updates will be sent to. (Can be a comma seperated list of emails)",
            view: "textbox",
            value: $scope.model.settings.TicketUpdateEmail
        },
        {
            alias: "EmailSubjectNewTicket",
            label: "Email Subject New Ticket",
            description: "Subject of the email when a new ticket is created. You can add ticket properties by adding {} around the property name (e.g., {ExternalTicketId}, {Title}, {Status.Name}, {Type.Name})",
            view: "textbox",
            value: $scope.model.settings.EmailSubjectNewTicket
        },
        {
            alias: "EmailSubjectUpdateTicket",
            label: "Email Subject Update Ticket",
            description: "Subject of the email when a ticket is updated. You can add ticket properties by adding {} around the property name (e.g., {ExternalTicketId}, {Title}, {Status.Name}, {Type.Name})",
            view: "textbox",
            value: $scope.model.settings.EmailSubjectUpdateTicket
        },
        {
            alias: "EmailTemplateNewTicketPath",
            label: "Email Template New Ticket Path",
            description: "Path to template being sent when new ticket is created",
            view: "textbox",
            value: $scope.model.settings.EmailTemplateNewTicketPath
        },
        {
            alias: "EmailTemplateUpdateTicketPath",
            label: "Email Template Update Ticket Path",
            description: "Path to template being sent when ticket is updated",
            view: "textbox",
            value: $scope.model.settings.EmailTemplateUpdateTicketPath
        },
    ];
});