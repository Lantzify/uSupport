using System;
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
using Umbraco.Cms.Web.Common.Authorization;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Backoffice.Trees
{
	[PluginController(uSupportConstants.SectionAlias)]
	[Authorize(Policy = AuthorizationPolicies.TreeAccessDocumentTypes)]
	[Tree(uSupportConstants.SectionAlias, TicketStatusesTreeAlias, SortOrder = 4, TreeTitle = "Ticket statuses", TreeGroup = TreeGroupAlias)]
	public class uSupportTicketStatusesTreeController : TreeController
	{
		private readonly IuSupportTicketStatusService _uSupportTicketStatusService;
		private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;

		public uSupportTicketStatusesTreeController(
			IuSupportTicketStatusService uSupportTicketStatusService,
			ILocalizedTextService localizedTextService,
			UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
			IMenuItemCollectionFactory menuItemCollectionFactory,
			IEventAggregator eventAggregator)
			: base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
		{
			_uSupportTicketStatusService = uSupportTicketStatusService;
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
			root.RoutePath = string.Format("/{0}/{1}/{2}", uSupportConstants.SectionAlias, TicketStatusesTreeAlias, "overview");
			root.Icon = "icon-file-cabinet";
			root.HasChildren = false;
			root.AdditionalData.Add("type", TicketStatusesTreeAlias);

			return root;
		}

		protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
		{
			var nodes = new TreeNodeCollection();

			if (id == Umbraco.Cms.Core.Constants.System.Root.ToInvariantString())
			{
				foreach (uSupportTicketStatus ticketStatus in _uSupportTicketStatusService.GetAll())
				{
					var node = CreateTreeNode($"{ticketStatus.Id}", TicketStatusesTreeAlias, queryStrings, ticketStatus.Name, $"icon-file-cabinet", false, $"uSupport/{TicketStatusesTreeAlias}/edit/{ticketStatus.Id}");
					node.AdditionalData.Add("overviewRoutePath", string.Format("/{0}/{1}/overview", uSupportConstants.SectionAlias, TicketStatusesTreeAlias));

					nodes.Add(node);
				}
			}

			return nodes;
		}

		protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
		{
			var menu = _menuItemCollectionFactory.Create();

			if (id == Umbraco.Cms.Core.Constants.System.Root.ToInvariantString())
            {
				menu.Items.Add<ActionNew>(LocalizedTextService, true, false).NavigateToRoute($"/uSupport/ticketStatuses/edit/-1?create");
				menu.Items.Add<ActionSort>(LocalizedTextService, true, true).LaunchDialogView("/App_Plugins/uSupport/components/actions/sort.html", "Sort");
			}
            else
            {
				menu.Items.Add<ActionDelete>(LocalizedTextService, true, true).LaunchDialogView("/App_Plugins/uSupport/components/actions/delete.html", "Delete");
			}

			return menu;
		}
	}
}