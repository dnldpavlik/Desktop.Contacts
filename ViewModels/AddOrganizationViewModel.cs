
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Domain.Interfaces;
	using DonPavlik.Domain.Model;
	using DonPavlik.WikiRepository.Interfaces;
	using Microsoft.Win32;
	using ReactiveUI;
	using ReactiveUI.Xaml;

	[Export(typeof(IAddOrganizationViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class AddOrganizationViewModel :
		ReactiveObject,
		IAddOrganizationViewModel
	{
		private const string ADD_ORGANIZATION = "Add Organization";

		private const string EDIT_ORGANIZATION = "Edit Organization";

		private IRepository<IOrganization> _organizationRepo;

		/// <summary>
		/// Initializes a new instance of <see cref="AddOrganizationViewModel"/> class.
		/// </summary>
		/// <param name="organizationRepo">Organization Repository</param>
		[ImportingConstructor]
		public AddOrganizationViewModel(IRepository<IOrganization> organizationRepo)
		{
			this._organizationRepo = organizationRepo;

			this.OrganizationPreview = new OrganizationViewModel(new Organization());
			this.Caption = ADD_ORGANIZATION;

			this.SetupSaveCommand();
		}

		/// <summary>
		/// Gets or sets the caption to let the user know if they are
		/// adding or editing an organization
		/// </summary>
		public string Caption { get; private set; }

		/// <summary>
		/// Gets or sets the organization name.
		/// </summary>
		public string OrganizationName { get; set; }

		/// <summary>
		/// Gets or sets the selected picture for the organization
		/// </summary>
		public string SelectedPicture { get; set; }

		/// <summary>
		/// Gets the save command
		/// </summary>
		public ReactiveCommand SaveCommand { get; private set; }

		/// <summary>
		/// Gets the organization view model that is being edited
		/// </summary>
		public OrganizationViewModel OrganizationPreview { get; private set; }

		/// <summary>
		/// Opens the file open dialog so that the user can select the picture they want for 
		/// the user, and assigns that value to the Contact.
		/// </summary>
		public void SelectPictureAction()
		{
			OpenFileDialog selectPictureDialog = new OpenFileDialog();

			if (selectPictureDialog.ShowDialog() == true)
			{
				//this.OrganizationPreview.Contact.Image = selectPictureDialog.FileName;
				this.RaisePropertyChanged(x => x.SelectedPicture);
			}
		}

		/// <summary>
		/// Hydrates the add organization view model with the organization that is 
		/// to be edited and sets its mode to edit mode from save.
		/// </summary>
		/// <param name="organization">The organization to 
		/// be edited</param>
		public void EditExistingOrganization(IOrganization organization)
		{
			this.OrganizationPreview = new OrganizationViewModel(organization);
			this.OrganizationName = OrganizationPreview.OrgName;
			this.Caption = EDIT_ORGANIZATION;
		}

		/// <summary>
		/// Saves the changes to organization repository
		/// </summary>
		private void Save()
		{
			if (string.Equals(EDIT_ORGANIZATION, this.Caption))
			{
				this._organizationRepo.Update(this.OrganizationPreview.Organization);
			}
			else
			{
				this._organizationRepo.Add(this.OrganizationPreview.Organization);
			}
		}

		/// <summary>
		/// Initializes the Save Command
		/// </summary>
		private void SetupSaveCommand()
		{
			var canSave = this.WhenAny(x => x.OrganizationName,
				(b) => string.IsNullOrWhiteSpace(b.Value));

			this.SaveCommand = new ReactiveCommand();
			this.SaveCommand
				.Subscribe((obj) =>
				{
					this.Save();
				});
		}
	}
}
