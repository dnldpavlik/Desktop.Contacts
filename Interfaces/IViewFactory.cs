
namespace DonPavlik.Desktop.Contacts.Interfaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public interface IViewFactory
	{
		IAddOrganizationViewModel GetAddOrganizationView();

		IAddOrganizationViewModel GetEditOrganizationView();

		IGroupViewModel GetGroupView();

		IAddPersonViewModel GetAddPersonView();

		IAddPersonViewModel GetEditPersonView();

		IPeopleViewModel GetPeopleView();

		IOrganizationsViewModel GetOrganizationView();

		void ClearView(string viewName);
	}
}
