﻿
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Threading.Tasks;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Domain.Interfaces;
	using DonPavlik.WikiRepository.Interfaces;
	using ReactiveUI;

	/// <summary>
	/// Organizations view model class definition
	/// </summary>
	[Export(typeof(IOrganizationsViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class OrganizationsViewModel :
		ReactiveObject,
		IPartImportsSatisfiedNotification, 
		IOrganizationsViewModel
	{
		private OrganizationViewModel _SelectedItem;

		/// <summary>
		/// Gets the Organization Repository. Populated by MEF.
		/// </summary>
		[Import]
		public IRepository<IOrganization> OrganizationRepository { get; private set; }

		/// <summary>
		/// Gets the collection of Organizations
		/// </summary>
		public ReactiveCollection<OrganizationViewModel> Organizations 
		{ 
			get; private set; 
		}

		/// <summary>
		/// Gets or sets the selected item
		/// </summary>
		public OrganizationViewModel SelectedItem
		{
			get
			{
				return this._SelectedItem;
			}

			protected set
			{
				this._SelectedItem = value;
				this.RaisePropertyChanged(x => x.SelectedItem);
			}
		}

		/// <summary>
		/// When imports have been satisfied for getting the orgainization repository
		/// to get the collection of organizations from the repo.
		/// </summary>
		public async void OnImportsSatisfied()
		{
			ICollection<IOrganization> orgainizations = await this.GetOrganizations();
			this.Organizations =
				orgainizations.CreateDerivedCollection(x => new OrganizationViewModel(x));

			this.RaisePropertyChanged(x => x.Organizations);
		}

		/// <summary>
		/// Removes an existing organization
		/// </summary>
		/// <param name="organization">Organization to be removed</param>
		public void RemoveExistingOrganization(OrganizationViewModel organization)
		{
			//this.OrganizationRepository.
		}

		private async Task<ICollection<IOrganization>> GetOrganizations()
		{
			return await this.OrganizationRepository.GetAll();
		}
	}
}
