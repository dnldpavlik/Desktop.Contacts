
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
	using DonPavlik.WikiRepository.Interfaces;
	using Microsoft.Win32;
	using ReactiveUI;
	using ReactiveUI.Xaml;

	[Export(typeof(IAddUserViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
	public class AddOrganization :
		ReactiveObject,
		IAddOrganization
	{
		private const string ADD_ORGANIZATION = "Add Organization";

		private const string EDIT_ORGANIZATION = "Edit Organization";

		private IRepository<IOrganization> _organizationRepo;

		public AddOrganization(
			IRepository<IOrganization> organizationRepo)
		{
			this._organizationRepo = organizationRepo;

			var canSave = this.WhenAny(x => x.OrganizationName, 
				(b) => string.IsNullOrWhiteSpace(b.Value));

			this.SaveCommand = new ReactiveCommand();
			this.SaveCommand
				.Subscribe((obj) =>
					{
						this.Save();
					});
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
	}
}
