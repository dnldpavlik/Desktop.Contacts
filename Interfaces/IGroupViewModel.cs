
using System;
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
		void HandleSave();

		/// <summary>
		/// Gets the active module name (Should be view)
		/// </summary>
		string ActiveModuleName { get; }

		/// <summary>
		/// Gets or sets the selected contact item
		/// </summary>
		IObservable<object> SelectedContactItem { get; set; }
	}
}
