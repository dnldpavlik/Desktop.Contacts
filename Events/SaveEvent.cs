
namespace DonPavlik.Desktop.Contacts.Events
{
	/// <summary>
	/// Save event definition, used for letting all subscribers know when 
	/// a save was completed.
	/// </summary>
	public class SaveEvent
	{
		/// <summary>
		/// Gets or sets the message that is go along with the save event
		/// </summary>
		public string Message { get; set; }
	}
}
