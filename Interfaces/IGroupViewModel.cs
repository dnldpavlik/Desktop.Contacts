
using System;
using DonPavlik.Desktop.Contacts.ViewModels;
namespace DonPavlik.Desktop.Contacts.Interfaces
{
	/// <summary>
	/// Group view model interface defintion
	/// </summary>
	public interface IGroupViewModel
	{
		/// <summary>
		/// Handles the save event. 
		/// </summary>
		/// <param name="saveEvent">
		/// Save event
		/// </param>
		void HandleSave(string saveType);

		/// <summary>
		/// Gets the active item for the group view model.
		/// </summary>
		object ActiveItem { get; }

		/// <summary>
		/// Gets the selected contact item
		/// </summary>
		object SelectedContactItem { get; }

		/// <summary>
		/// Removes an existing organization
		/// </summary>
		/// <param name="organization">Organization to be removed</param>
		void RemoveExistingOrganization(OrganizationViewModel organization);

		/// <summary>
		/// Removes existing contact
		/// </summary>
		/// <param name="contact">Contact that is be destroyed</param>
		void RemoveExistingContact(ContactViewModel contact);
	}
}
