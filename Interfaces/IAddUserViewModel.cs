
namespace DonPavlik.Desktop.Contacts.Interfaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using DonPavlik.Domain.Interfaces.Roles;

	public interface IAddUserViewModel
	{
		void EditExistingContact(IContact selectedContact);
	}
}
