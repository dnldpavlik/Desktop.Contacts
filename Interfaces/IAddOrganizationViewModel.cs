
namespace DonPavlik.Desktop.Contacts.Interfaces
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using DonPavlik.Domain.Interfaces;
	using ReactiveUI.Xaml;

	public interface IAddOrganizationViewModel
	{
		/// <summary>
		/// Hydrates the add organization view model with the organization that is 
		/// to be edited and sets its mode to edit mode from save.
		/// </summary>
		/// <param name="organization">The organization to 
		/// be edited</param>
		void EditExistingOrganization(IOrganization organization);

		/// <summary>
		/// Gets or sets the Save Command
		/// </summary>
		ReactiveCommand SaveCommand { get; }
	}
}
