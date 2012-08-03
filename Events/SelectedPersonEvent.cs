
namespace DonPavlik.Desktop.Contacts.Events
{
	using DonPavlik.Domain.Interfaces.Roles;

	/// <summary>
	/// Selected person event used to let all subscribers know what 
	/// person was selected.
	/// </summary>
	public class SelectedPersonEvent
	{
		/// <summary>
		/// Gets or sets the selected Contact
		/// </summary>
		public IContact SelectedContacts
		{
			get;
			set;
		}
	}
}
