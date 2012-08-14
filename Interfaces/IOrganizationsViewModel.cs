
namespace DonPavlik.Desktop.Contacts.Interfaces
{
	using DonPavlik.Desktop.Contacts.ViewModels;

	/// <summary>
	/// Organizations view model definition
	/// </summary>
	public interface IOrganizationsViewModel
	{
		/// <summary>
		/// Gets the selected item
		/// </summary>
		OrganizationViewModel SelectedItem { get; }

		/// <summary>
		/// Removes an existing organization
		/// </summary>
		/// <param name="organization">Organization to be removed</param>
		void RemoveExistingOrganization(OrganizationViewModel organization);
	}
}
