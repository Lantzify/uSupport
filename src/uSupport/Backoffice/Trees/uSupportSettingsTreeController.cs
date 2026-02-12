using System;
using Umbraco.Cms.Core;
using uSupport.Constants;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Core.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Umbraco.Cms.Web.Common.Authorization;
using static uSupport.Constants.uSupportConstants;

namespace uSupport.Backoffice.Trees
{
    [PluginController(uSupportConstants.SectionAlias)]
	[Authorize(Policy = AuthorizationPolicies.TreeAccessDocumentTypes)]
    [Tree(uSupportConstants.SectionAlias, "settings", SortOrder = 5, TreeTitle = "Settings", TreeGroup = TreeGroupAlias)]
    public class uSupportSettingsTreeController : TreeController
    {
		private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;

		public uSupportSettingsTreeController(
			ILocalizedTextService localizedTextService,
			UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
			IMenuItemCollectionFactory menuItemCollectionFactory,
			IEventAggregator eventAggregator)
			: base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
		{
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
			root.RoutePath = "uSupport/settings/overview";
			root.Icon = "icon-settings";
			root.HasChildren = false;

			root.MenuUrl = null;

			return root;
		}

		protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
		{
			var nodes = new TreeNodeCollection();

			return nodes;
		}

		protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
		{
			var menu = _menuItemCollectionFactory.Create();

			return menu;
		}
    }
}