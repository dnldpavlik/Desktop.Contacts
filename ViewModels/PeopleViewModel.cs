
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using DonPavlik.Desktop.Contacts.Events;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Domain.Interfaces.Roles;
	using DonPavlik.WikiRepository.Interfaces;
	using ReactiveUI;

	/// <summary>
	/// 
	/// </summary>
	[Export(typeof(IPeopleViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class PeopleViewModel : 
		ReactiveObject, 
		IPartImportsSatisfiedNotification, 
		IPeopleViewModel
	{
		private ContactViewModel _selectedPerson;

		private IEventAggregator _eventAggregator;

		/// <summary>
		/// Creates an instance of the <see cref="PeopleViewModel"/> class.
		/// </summary>
		/// <param name="eventAggregator">Event Aggregator for dealing with 
		/// events.</param>
		[ImportingConstructor]
		public PeopleViewModel(IEventAggregator eventAggregator)
		{
			this._eventAggregator = eventAggregator;


		}

		/// <summary>
		/// Gets the Contact Repository
		/// </summary>
		[Import]
		public IRepository<IContact> ContactRepository { get; private set; }

		/// <summary>
		/// Gets or sets teh Selected Person
		/// </summary>
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

		/// <summary>
		/// Gets the collection of People
		/// </summary>
		public ReactiveCollection<ContactViewModel> People { get; private set; }

		/// <summary>
		/// Executed when the all imports have been satisfied by MEF.  
		/// 
		/// Gets the collection of contacts from the contact repository.
		/// </summary>
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
	}
}
