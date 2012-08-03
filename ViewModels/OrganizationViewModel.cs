
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Linq;
	using System.Text;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Domain.Interfaces;

	public class OrganizationViewModel
	{
		private IOrganization _organization;

		public OrganizationViewModel(IOrganization organization)
		{
			this._organization = organization;
		}

		public string Name
		{
			get
			{
				return this._organization.Name;
			}
		}
	}
}
