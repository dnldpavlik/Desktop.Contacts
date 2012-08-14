
namespace DonPavlik.Desktop.Contacts.Interfaces
{
	using System;
	using System.Reactive.Subjects;
	using DonPavlik.Desktop.Contacts.ViewModels;

	/// <summary>
	/// People view model definition
	/// </summary>
	public interface IPeopleViewModel
	{
		/// <summary>
		/// Gets the Selected item
		/// </summary>
		ContactViewModel SelectedItem { get; }

		/// <summary>
		/// Removes existing contact
		/// </summary>
		/// <param name="contact">Contact that is be destroyed</param>
		void RemoveExistingContact(ContactViewModel contact);
	}
}
