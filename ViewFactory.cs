
namespace DonPavlik.Desktop.Contacts
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using DonPavlik.Desktop.Contacts.Interfaces;

	/// <summary>
	/// View factory class definition
	/// </summary>
	[Export(typeof(IViewFactory))]
	public class ViewFactory : IViewFactory
	{
		private readonly Dictionary<string, object> _cachedViews =
			new Dictionary<string, object>();

		public IAddOrganizationViewModel GetAddOrganizationView()
		{
			this.InitializeAddOrganizationView(ViewNames.ADD_ORGANIZATION);
			return this._cachedViews[ViewNames.ADD_ORGANIZATION] as IAddOrganizationViewModel;
		}

		public IAddOrganizationViewModel GetEditOrganizationView()
		{
			this.InitializeAddOrganizationView(ViewNames.EDIT_ORGANIZATION);
			return this._cachedViews[ViewNames.EDIT_ORGANIZATION] as IAddOrganizationViewModel;
		}

		public IGroupViewModel GetGroupView()
		{
			this.InitializeGroupView();
			return this._cachedViews[ViewNames.GROUPS] as IGroupViewModel;
		}

		public IAddPersonViewModel GetAddPersonView()
		{
			this.InitializeAddPersonView(ViewNames.ADD_PERSON);
			return this._cachedViews[ViewNames.ADD_PERSON] as IAddPersonViewModel;
		}

		public IAddPersonViewModel GetEditPersonView()
		{
			this.InitializeAddPersonView(ViewNames.EDIT_PERSON);
			return this._cachedViews[ViewNames.EDIT_PERSON] as IAddPersonViewModel;
		}

		public IPeopleViewModel GetPeopleView()
		{
			this.InitializePeopleView();
			return this._cachedViews[ViewNames.PEOPLE] as IPeopleViewModel;
		}

		public IOrganizationsViewModel GetOrganizationView()
		{
			this.InitializeOrganizationView();
			return this._cachedViews[ViewNames.ORGANIZATION] as IOrganizationsViewModel;
		}

		public void ClearView(string viewName)
		{
			if (this._cachedViews.ContainsKey(viewName))
			{
				this._cachedViews.Remove(viewName);
			}
		}

		private void InitializeOrganizationView()
		{
			if (!this._cachedViews.ContainsKey(ViewNames.ORGANIZATION))
			{
				this._cachedViews.Add(
					ViewNames.ORGANIZATION,
					IoC.Get<IOrganizationsViewModel>());
			}
		}

		private void InitializePeopleView()
		{
			if (!this._cachedViews.ContainsKey(ViewNames.PEOPLE))
			{
				this._cachedViews.Add(
					ViewNames.PEOPLE,
					IoC.Get<IPeopleViewModel>());
			}
		}

		private void InitializeGroupView()
		{
			if (!this._cachedViews.ContainsKey(ViewNames.GROUPS))
			{
				this._cachedViews.Add(
					ViewNames.GROUPS,
					IoC.Get<IGroupViewModel>());
			}
		}

		private void InitializeAddOrganizationView(string viewName)
		{
			if (!this._cachedViews.ContainsKey(viewName))
			{
				IAddOrganizationViewModel addOrganization = 
					IoC.Get<IAddOrganizationViewModel>();

				this._cachedViews.Add(
					viewName,
					addOrganization);
			}
		}

		private void InitializeAddPersonView(string viewName)
		{
			if (!this._cachedViews.ContainsKey(viewName))
			{
				IAddPersonViewModel addPerson = IoC.Get<IAddPersonViewModel>();

				this._cachedViews.Add(
					viewName,
					addPerson);
			}
		}
	}
}
