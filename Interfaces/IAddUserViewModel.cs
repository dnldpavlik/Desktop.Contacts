
namespace DonPavlik.Desktop.Contacts.Interfaces
{
	using DonPavlik.Domain.Interfaces.Roles;
	using ReactiveUI.Xaml;

	/// <summary>
	/// Interface definition for the add user view model
	/// </summary>
	public interface IAddUserViewModel
	{
		/// <summary>
		/// Hydrates the add user view model with the contact that is 
		/// to be edited and sets its mode to edit mode from save.
		/// </summary>
		/// <param name="selectedContact">The selected contact to 
		/// be edited</param>
		void EditExistingContact(IContact selectedContact);

		/// <summary>
		/// Gets or sets the Save Command
		/// </summary>
		ReactiveCommand SaveCommand { get; }
	}
}
