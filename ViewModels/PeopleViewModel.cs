
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Reactive.Subjects;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Domain.Interfaces.Roles;
	using DonPavlik.WikiRepository.Interfaces;
	using ReactiveUI;
using ReactiveUI.Xaml;

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
		/// Initializes a new instance of the <see cref="PeopleViewModel"/> class.
		/// </summary>
		[ImportingConstructor]
		public PeopleViewModel()
		{
			this.SetupRemoveCommand();
		}

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
		/// Gets the remove command
		/// </summary>
		public ReactiveCommand Remove { get; protected set; }

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

		private void SetupRemoveCommand()
		{
			var canRemove = this.WhenAny(x => x.SelectedItem, (b) => b.Value != null);

			this.Remove = new ReactiveCommand(canRemove);
			this.Remove
				.Subscribe((arg) =>
					{
						ContactViewModel contactVM = arg as ContactViewModel;

						if (contactVM != null)
						{
							this.ContactRepository.Delete(contactVM.Contact);
						}
					});
		}

		private async Task<ICollection<IContact>> GetPeople()
		{
			return await this.ContactRepository.GetAll();
		}
	}
}
