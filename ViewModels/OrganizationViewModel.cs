
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using DonPavlik.Domain.Interfaces;

	/// <summary>
	/// Organization view model class definiton
	/// </summary>
	public class OrganizationViewModel
	{
		private IOrganization _organization;

		/// <summary>
		/// Creasts an instance of the <see cref="OrganizationViewModel"/> class.
		/// </summary>
		/// <param name="organization">
		/// Organization being wrapped
		/// </param>
		public OrganizationViewModel(IOrganization organization)
		{
			this._organization = organization;
		}

		/// <summary>
		/// Gets the organization name
		/// </summary>
		public string OrgName
		{
			get
			{
				return this._organization.Name;
			}
		}
	}
}
