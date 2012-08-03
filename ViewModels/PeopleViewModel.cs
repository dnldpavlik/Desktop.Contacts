
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using DonPavlik.Desktop.Contacts.Events;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Domain.Interfaces.Roles;
	using DonPavlik.WikiRepository.Interfaces;
	using ReactiveUI;

	[Export(typeof(IPeopleViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class PeopleViewModel : 
		ReactiveObject, 
		IPartImportsSatisfiedNotification, 
		IPeopleViewModel
	{
		private ContactViewModel _selectedPerson;

		private IEventAggregator _eventAggregator;

		[ImportingConstructor]
		public PeopleViewModel(IEventAggregator eventAggregator)
		{
			this._eventAggregator = eventAggregator;
		}

		[Import]
		public IRepository<IContact> ContactRepository { get; private set; }

		public ContactViewModel SelectedPerson
		{
			get
			{
				return this._selectedPerson;
			}

			set
			{
				this._selectedPerson = value;
				this._eventAggregator.Publish(
					new SelectedPersonEvent() 
					{ 
						SelectedContacts = value.Contact 
					});
			}
		}

		public async void OnImportsSatisfied()
		{
			ICollection<IContact> contacts = await this.GetPeople();
			this.People = 
				contacts.CreateDerivedCollection(x => new ContactViewModel(x));

			this.RaisePropertyChanged(x => x.People);
		}

		private async Task<ICollection<IContact>> GetPeople()
		{
			return await this.ContactRepository.GetAll();
		}

		public ReactiveCollection<ContactViewModel> People { get; private set; }

		public object ActionsRegion { get; set; }
	}
}
