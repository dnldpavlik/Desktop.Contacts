
namespace DonPavlik.Desktop.Contacts.Interfaces
{
	using System.ComponentModel;
	using DonPavlik.Desktop.Contacts.ViewModels;
	using DonPavlik.Domain.Interfaces.Roles;
	using ReactiveUI;
	using ReactiveUI.Xaml;

	/// <summary>
	/// Interface definition for the add user view model
	/// </summary>
	public interface IAddPersonViewModel
	{
		/// <summary>
		/// Hydrates the add user view model with the contact that is 
		/// to be edited and sets its mode to edit mode from save.
		/// </summary>
		/// <param name="selectedContact">The selected contact to 
		/// be edited</param>
		void EditExistingContact(ContactViewModel selectedContact);

		/// <summary>
		/// Gets or sets the Save Command
		/// </summary>
		ReactiveCommand SaveCommand { get; }
	}
}
