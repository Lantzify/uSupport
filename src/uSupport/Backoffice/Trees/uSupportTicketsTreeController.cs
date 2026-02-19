using Umbraco.Cms.Core;
using Umbraco.Extensions;
using uSupport.Constants;
using uSupport.Dtos.Tables;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Actions;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Services;
using uSupport.Services.Interfaces;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Backoffice.Trees
{
    [PluginController(uSupportConstants.SectionAlias)]
	[Authorize(Policy = uSupportSectionAccess)]
    [Tree(uSupportConstants.SectionAlias, TicketsTreeAlias, SortOrder = 0, TreeTitle = "Tickets", TreeGroup = uSupportConstants.TreeGroupAlias)]
    public class uSupportTicketsTreeController : TreeController
    {

        private readonly IuSupportTicketService _uSupportTicketService;
        private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;

        public uSupportTicketsTreeController(
            IuSupportTicketService uSupportTicketService,
            ILocalizedTextService localizedTextService,
            UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
            IMenuItemCollectionFactory menuItemCollectionFactory,
            IEventAggregator eventAggregator)
            : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        {
            _uSupportTicketService = uSupportTicketService;
            _menuItemCollectionFactory = menuItemCollectionFactory ?? throw new ArgumentNullException(nameof(menuItemCollectionFactory));
        }

        protected override ActionResult<TreeNode> CreateRootNode(FormCollection queryStrings)
        {
            var rootResult = base.CreateRootNode(queryStrings);
            if (!(rootResult.Result is null))
            {
                return rootResult;
            }

            var root = rootResult.Value;
            root.RoutePath = string.Format("/{0}/{1}/{2}", uSupportConstants.SectionAlias, "tickets", "overview");
            root.Icon = "icon-inbox-full";
            root.HasChildren = false;
            root.MenuUrl = null;

            return root;
        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            if (id == Umbraco.Cms.Core.Constants.System.Root.ToInvariantString())
            {
                foreach (uSupportTicket ticket in _uSupportTicketService.GetAll())
                {
                    var node = CreateTreeNode($"{ticket.Id}", "tickets", queryStrings, ticket.Title);
                    node.AdditionalData.Add("overviewRoutePath", "uSupport/tickets/overview");
                    node.AdditionalData.Add("type", "tickets");
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
        {
            var menu = _menuItemCollectionFactory.Create();
            if (id != Umbraco.Cms.Core.Constants.System.Root.ToInvariantString())
            {
                menu.Items.Add<ActionDelete>(LocalizedTextService, true, true).LaunchDialogView("/App_Plugins/uSupport/components/actions/delete.html", "Delete");
            }

            return menu;
        }
    }
}