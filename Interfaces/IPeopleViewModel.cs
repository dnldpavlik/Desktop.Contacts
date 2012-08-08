
namespace DonPavlik.Desktop.Contacts.Interfaces
{
	using System;
	using DonPavlik.Desktop.Contacts.ViewModels;

	/// <summary>
	/// People view model definition
	/// </summary>
	public interface IPeopleViewModel
	{
		/// <summary>
		/// Gets or sets teh Selected Person
		/// </summary>
		ContactViewModel SelectedPerson { get; set; }
	}
}
