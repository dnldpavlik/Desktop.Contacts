
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.ComponentModel.Composition;
	using Caliburn.Micro;
	using DonPavlik.Desktop.Contacts.Events;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Domain.Interfaces.Roles;
	using DonPavlik.Domain.Model;
	using DonPavlik.Domain.Model.Roles;
	using DonPavlik.WikiRepository.Interfaces;
	using Microsoft.Win32;
	using ReactiveUI;

	[Export(typeof(IAddUserViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class AddUserViewModel : ReactiveObject, IAddUserViewModel
	{
		private const string ADD_CONTACT = "Add Person";

		private const string EDIT_CONTACT = "Edit Person";

		private const string GENDER_MALE = "Male";

		private const string GENDER_FEMALE = "Female";

		private readonly IRepository<IContact> _contactRepo;

		private readonly IEventAggregator _events;

		private string _FullName;

		[ImportingConstructor]
		public AddUserViewModel(
			IRepository<IContact> contactRepo,
			IEventAggregator events)
		{
			this._events = events;
			this._contactRepo = contactRepo;

			Contact contact = new Contact(new Person());
			this.PersonPreview = new ContactViewModel(contact);
			this.Caption = ADD_CONTACT;
		}

		public string Caption
		{
			get;
			set;
		}

		public DateTime? DateOfDeath 
		{
			get
			{
				return this.PersonPreview.Contact.DateOfDeath;
			}

			set
			{
				this.PersonPreview.Contact.DateOfDeath = value;
			}
		}

		public DateTime? DateOfBirth 
		{
			get
			{
				return this.PersonPreview.Contact.DateOfBirth;
			}

			set
			{
				this.PersonPreview.Contact.DateOfBirth = value;
			}
		}

		public bool IsMale
		{
			get
			{
				return string.Equals(this.PersonPreview.Contact.Gender, GENDER_MALE);
			}

			set
			{
				this.PersonPreview.Contact.Gender = GENDER_MALE;
			}
		}

		public bool IsFemale
		{
			get
			{
				return string.Equals(this.PersonPreview.Contact.Gender, GENDER_FEMALE);
			}

			set
			{
				this.PersonPreview.Contact.Gender = GENDER_FEMALE;
			}
		}

		public string FullName 
		{
			get
			{
				return this._FullName;
			}

			set
			{
				this.RaiseAndSetIfChanged(x => x.FullName, value);
				this.PersonPreview.FullName = value;
			}
		}

		public string SelectedPicture
		{
			get
			{
				return this.PersonPreview.Contact.Image;
			}

			set
			{
				this.PersonPreview.Contact.Image = value;
				this.PersonPreview.RefreshImageBinding();
			}
		}

		public string EMailAddress
		{
			get
			{
				return this.PersonPreview.Contact.Email;
			}

			set
			{
				this.PersonPreview.Contact.Email = value;
			}
		}

		public void Save()
		{
			if (string.Equals(EDIT_CONTACT, this.Caption))
			{
				this._contactRepo.Update(this.PersonPreview.Contact);
			}
			else
			{
				this._contactRepo.Add(this.PersonPreview.Contact);
			}

			this._events.Publish(
				new SaveEvent
				{
					Message = "Saved Contact " + this.PersonPreview.FullName
				});
		}

		public void SelectPictureAction()
		{
			OpenFileDialog selectPictureDialog = new OpenFileDialog();

			if (selectPictureDialog.ShowDialog() == true)
			{
				this.PersonPreview.Contact.Image = selectPictureDialog.FileName;
				this.PersonPreview.RefreshImageBinding();
				this.RaisePropertyChanged(x => x.SelectedPicture);
			}
		}

		public ContactViewModel PersonPreview { get; set; }

		public void EditExistingContact(IContact selectedContact)
		{
			this.PersonPreview = new ContactViewModel(selectedContact);
			this._FullName = selectedContact.PrimaryName.FullName;
			this.Caption = EDIT_CONTACT;
		}
	}
}
