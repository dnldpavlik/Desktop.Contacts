
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using DonPavlik.Domain.Interfaces;

	/// <summary>
	/// Organization view model class definiton
	/// </summary>
	public class OrganizationViewModel
	{
		/// <summary>
		/// Creasts an instance of the <see cref="OrganizationViewModel"/> class.
		/// </summary>
		/// <param name="organization">
		/// Organization being wrapped
		/// </param>
		public OrganizationViewModel(IOrganization organization)
		{
			this.Organization = organization;
		}

		/// <summary>
		/// Gets the organization name
		/// </summary>
		public string OrgName
		{
			get
			{
				return this.Organization.Name;
			}
		}

		/// <summary>
		/// Gets or sets the Organization
		/// </summary>
		public IOrganization Organization { get; set; }

	}
}
