using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonPavlik.Domain.Interfaces;
using DonPavlik.WikiRepository.Repositories;

namespace DonPavlik.Desktop.Contacts.ViewModels
{
	public class OrganizationsViewModel
	{
		public OrganizationsViewModel()
		{
			WikiOrganizationRepository repo =
				new WikiOrganizationRepository(
					@"D:\Center\My Dropbox\Contacts\Organizations");

			this.Organizations = 
				new ObservableCollection<OrganizationViewModel>();
			foreach (IOrganization organization in repo.AllOrganizations)
			{
				this.Organizations.Add(
					new OrganizationViewModel(organization));
			}
		}

		public ObservableCollection<OrganizationViewModel> Organizations 
		{ 
			get; private set; 
		}
	}
}
