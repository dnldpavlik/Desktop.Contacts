
namespace DonPavlik.Desktop.Contacts.Interfaces
{
	using System;
	using System.Reactive.Subjects;
	using DonPavlik.Desktop.Contacts.ViewModels;
	using ReactiveUI.Xaml;

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
		/// Gets the remove command
		/// </summary>
		ReactiveCommand Remove { get; }
	}
}
