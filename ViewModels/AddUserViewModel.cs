
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Linq;
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
	using ReactiveUI.Xaml;

	/// <summary>
	/// Add user view model defintion that describes the input needed to create a user.
	/// </summary>
	[Export(typeof(IAddUserViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class AddUserViewModel : 
		ReactiveObject, 
		IAddUserViewModel
	{
		private const string ADD_CONTACT = "Add Person";

		private const string EDIT_CONTACT = "Edit Person";

		private const string GENDER_MALE = "Male";

		private const string GENDER_FEMALE = "Female";

		private readonly IRepository<IContact> _contactRepo;

		private readonly IEventAggregator _events;

		private string _FullName;

		/// <summary>
		/// Initializes a new instance of <see cref="AddUserViewModel"/> class.
		/// </summary>
		/// <param name="contactRepo">
		/// The contact repository
		/// </param>
		/// <param name="events">
		/// Event aggregator used for sending of events.
		/// </param>
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

			var canSave = this.WhenAny(x => x.FullName, x => x.EMailAddress,
				(b, u) => string.IsNullOrWhiteSpace(b.Value));

			this.SaveCommand= new ReactiveCommand(canSave);
			this.SaveCommand
				.Subscribe((obj) =>
					{
						this.Save();
					});
		}

		/// <summary>
		/// Gets or sets the caption to let the user know if they are
		/// adding or editing a user.
		/// </summary>
		public string Caption
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the date of death for this Contact
		/// </summary>
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

		/// <summary>
		/// Gets or sets the date of birth for this Contact
		/// </summary>
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

		/// <summary>
		/// Gets or sets boolean value if this Contact is a Male
		/// </summary>
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

		/// <summary>
		/// Gets or sets boolean value if this Contact is a Female
		/// </summary>
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

		/// <summary>
		/// Gets or sets the full name of the Contact
		/// </summary>
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

		/// <summary>
		/// Gets or sets the selected picture for the Contact
		/// </summary>
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

		/// <summary>
		/// Gets or sets the Email Address for the Contact
		/// </summary>
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

		/// <summary>
		/// Gets or sets the contact view model that is being edited
		/// </summary>
		public ContactViewModel PersonPreview { get; set; }

		/// <summary>
		/// Saves the changes to the contact repository.
		/// </summary>
		private void Save()
		{
			if (string.Equals(EDIT_CONTACT, this.Caption))
			{
				this._contactRepo.Update(this.PersonPreview.Contact);
			}
			else
			{
				this._contactRepo.Add(this.PersonPreview.Contact);
			}
		}

		/// <summary>
		/// Opens the file open dialog so that the user can select the picture they want for 
		/// the user, and assigns that value to the Contact.
		/// </summary>
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

		/// <summary>
		/// Hydrates the add user view model with the contact that is 
		/// to be edited and sets its mode to edit mode from save.
		/// </summary>
		/// <param name="selectedContact">The selected contact to 
		/// be edited</param>
		public void EditExistingContact(IContact selectedContact)
		{
			this.PersonPreview = new ContactViewModel(selectedContact);
			this._FullName = selectedContact.PrimaryName.FullName;
			this.Caption = EDIT_CONTACT;
		}

		/// <summary>
		/// Gets or sets the Save Command
		/// </summary>
		public ReactiveCommand SaveCommand { get; private set; }
	}
}
