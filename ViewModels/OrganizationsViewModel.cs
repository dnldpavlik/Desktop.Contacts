
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Threading.Tasks;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Domain.Interfaces;
	using DonPavlik.WikiRepository.Interfaces;
	using ReactiveUI;

	[Export(typeof(IOrganizationsViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class OrganizationsViewModel :
		ReactiveObject,
		IPartImportsSatisfiedNotification, 
		IOrganizationsViewModel
	{
		[Import]
		public IRepository<IOrganization> OrganizationRepository { get; private set; }

		public ReactiveCollection<OrganizationViewModel> Organizations 
		{ 
			get; private set; 
		}

		public async void OnImportsSatisfied()
		{
			ICollection<IOrganization> orgainizations = await this.GetOrganizations();
			this.Organizations =
				orgainizations.CreateDerivedCollection(x => new OrganizationViewModel(x));

			this.RaisePropertyChanged(x => x.Organizations);
		}

		private async Task<ICollection<IOrganization>> GetOrganizations()
		{
			return await this.OrganizationRepository.GetAll();
		}

	}
}
