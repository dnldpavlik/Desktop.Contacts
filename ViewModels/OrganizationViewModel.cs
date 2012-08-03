using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DonPavlik.Domain.Interfaces;

namespace DonPavlik.Desktop.Contacts.ViewModels
{
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
