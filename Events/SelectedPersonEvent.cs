
namespace DonPavlik.Desktop.Contacts.Events
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using DonPavlik.Domain.Interfaces.Roles;

	public class SelectedPersonEvent
	{
		public IContact SelectedContacts
		{
			get;
			set;
		}
	}
}
