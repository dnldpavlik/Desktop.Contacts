
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Reactive.Subjects;
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
		private ContactViewModel _SelectedItem;

		/// <summary>
		/// Gets the Contact Repository
		/// </summary>
		[Import]
		public IRepository<IContact> ContactRepository { get; private set; }

		/// <summary>
		/// Gets the Selected item
		/// </summary>
		public ContactViewModel SelectedItem
		{
			get
			{
				return this._SelectedItem;
			}

			protected set
			{
				this._SelectedItem = value;
				this.RaisePropertyChanged(x => x.SelectedItem);
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
			string path = this.ContactRepository.Path;
			ICollection<IContact> contacts = await this.GetPeople();
			this.People =
				contacts.CreateDerivedCollection(x => new ContactViewModel(x, path));

			this.RaisePropertyChanged(x => x.People);
		}

		/// <summary>
		/// Removes existing contact
		/// </summary>
		/// <param name="contact">Contact that is be destroyed</param>
		public void RemoveExistingContact(ContactViewModel contact)
		{
			//this.ContactRepository.
		}

		private async Task<ICollection<IContact>> GetPeople()
		{
			return await this.ContactRepository.GetAll();
		}
	}
}
