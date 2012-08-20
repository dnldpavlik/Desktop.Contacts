
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.ComponentModel.Composition;
	using System.Diagnostics.Contracts;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Desktop.Infrastructure.Interfaces;
	using DonPavlik.Domain.Interfaces;
	using ReactiveUI;
	using ReactiveUI.Xaml;

	/// <summary>
	/// Contacts Module View Model definition.  This Model class is the root of the 
	/// contacts module.
	/// </summary>
	[Export(typeof(IModule))]
	public class ContactsModuleViewModel : 
		ReactiveObject, 
		IModule
	{
		#region Private Constants and Fields

		private readonly IViewFactory _ViewFactory;

		private object _ActiveItem;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ContactsModuleViewModel"/> class.
		/// </summary>
		/// <param name="viewFactory">
		/// View factory used to get views.
		/// </param>
		[ImportingConstructor]
		public ContactsModuleViewModel(IViewFactory viewFactory)
		{
			Contract.Requires(viewFactory != null, "View Factory can not be null.");
			this._ViewFactory = viewFactory;

			this.InitializeCommands();
		}
		
		#region Public Properties

		/// <summary>
		/// Gets the Navigation back command that controls moving backwards.
		/// </summary>
		public ReactiveCommand NavigateBack
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the add command that shows the add view for the active
		/// subject matter.
		/// </summary>
		public ReactiveCommand Add
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the edit command that shows the edit view for the active
		/// subject matter.
		/// </summary>
		public ReactiveCommand Edit
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the delete command
		/// </summary>
		public ReactiveCommand Delete
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the active item, which is the active view
		/// </summary>
		public object ActiveItem
		{
			get
			{
				if (this._ActiveItem == null)
				{
					this.GroupViewModel = this._ViewFactory.GetGroupView();
					this._ActiveItem = this.GroupViewModel;
				}

				return this._ActiveItem;
			}

			protected set
			{
				this.RaiseAndSetIfChanged(t => t.ActiveItem, value);
			}
		}

		/// <summary>
		/// Gets the Group view Model
		/// </summary>
		public IGroupViewModel GroupViewModel { get; protected set; }

		#endregion

		#region Private Methods

		private void InitializeCommands()
		{
			this.SetupAddCommand();
			this.SetupEditCommand();
			this.SetupDeleteCommand();
			this.SetupNavigationCommand();
		}

		private void SetupAddCommand()
		{
			var canAddPerson = this.WhenAny(x => x.ActiveItem, x => x.ActiveItem,
				(b, u) => string.Equals(u.Value.GetType().Name, "GroupViewModel") );

			this.Add = new ReactiveCommand(canAddPerson);
			this.Add
				.Subscribe((arg) =>
				{
					if (string.Equals(GroupViewModel.ActiveItem.GetType().Name, "PeopleViewModel"))
					{
						IAddPersonViewModel addPersonView = this._ViewFactory.GetAddPersonView();

						addPersonView.SaveCommand
							.Subscribe((obj) =>
							{
								this._ViewFactory.ClearView(ViewNames.ADD_PERSON);
								GroupViewModel.HandleSave(ViewNames.PEOPLE);
								this.NavigateBack.Execute(null);
							});

						this.ActiveItem = addPersonView;
					}
					else if (string.Equals(GroupViewModel.ActiveItem.GetType().Name, "OrganizationsViewModel"))
					{
						IAddOrganizationViewModel addOrganization = this._ViewFactory.GetAddOrganizationView();

						addOrganization.SaveCommand
							.Subscribe((obj) =>
							{
								this._ViewFactory.ClearView(ViewNames.ADD_ORGANIZATION);
								GroupViewModel.HandleSave(ViewNames.ORGANIZATION);
								this.NavigateBack.Execute(null);
							});

						this.ActiveItem = addOrganization;
					}
				});
		}

		private void SetupEditCommand()
		{
			var canEditPerson = this.WhenAny(x => x.GroupViewModel.SelectedContactItem, x => x.GroupViewModel.ActiveItem,
				(b, u) => b.Value != null && 
					(b.Value is ContactViewModel && u.Value is PeopleViewModel) ||
					(b.Value is OrganizationViewModel && u.Value is OrganizationsViewModel));

			this.Edit = new ReactiveCommand(canEditPerson);
			this.Edit
				.Subscribe((arg) =>
				{
					if (string.Equals(GroupViewModel.ActiveItem.GetType().Name, "PeopleViewModel"))
					{
						IAddPersonViewModel addPersonView = this._ViewFactory.GetEditPersonView();

						ContactViewModel contactVM = GroupViewModel.SelectedContactItem as ContactViewModel;
						addPersonView.EditExistingContact(contactVM);

						addPersonView.SaveCommand
							.Subscribe((obj) =>
							{
								this._ViewFactory.ClearView(ViewNames.EDIT_PERSON);
								GroupViewModel.HandleSave(ViewNames.PEOPLE);
								this.NavigateBack.Execute(null);
							});

						this.ActiveItem = addPersonView;
					}
					else if (string.Equals(GroupViewModel.ActiveItem.GetType().Name, "OrganizationsViewModel"))
					{
						IAddOrganizationViewModel addOrganization = this._ViewFactory.GetEditOrganizationView();

						addOrganization.EditExistingOrganization(
							GroupViewModel.SelectedContactItem as IOrganization);

						addOrganization.SaveCommand
							.Subscribe((obj) =>
							{
								this._ViewFactory.ClearView(ViewNames.EDIT_ORGANIZATION);
								GroupViewModel.HandleSave(ViewNames.ORGANIZATION);
								this.NavigateBack.Execute(null);
							});

						this.ActiveItem = addOrganization;
					}
				});
		}

		private void SetupNavigationCommand()
		{
			var canOk = this.WhenAny(x => x.ActiveItem,
				(b) => !string.Equals(b.Value.GetType().Name, typeof(GroupViewModel).Name));

			this.NavigateBack = new ReactiveCommand(canOk);
			this.NavigateBack
				.Subscribe((obj) =>
				{
					this.ActiveItem = this._ViewFactory.GetGroupView();
				});
		}

		private void SetupDeleteCommand()
		{
			var canDeletePerson = this.WhenAny(x => x.GroupViewModel.SelectedContactItem, x => x.GroupViewModel.ActiveItem,
				(b, u) => b.Value != null &&
					(b.Value is ContactViewModel && u.Value is PeopleViewModel) ||
					(b.Value is OrganizationViewModel && u.Value is OrganizationsViewModel));

			this.Delete = new ReactiveCommand(canDeletePerson);
			this.Delete
				.Subscribe((arg) =>
					{
						if (string.Equals(GroupViewModel.ActiveItem.GetType().Name, "PeopleViewModel"))
						{
							ContactViewModel contactVM = GroupViewModel.SelectedContactItem as ContactViewModel;
							GroupViewModel.RemoveExistingContact(contactVM);
						}
						else if (string.Equals(GroupViewModel.ActiveItem.GetType().Name, "OrganizationsViewModel"))
						{
							GroupViewModel.RemoveExistingOrganization(
								GroupViewModel.SelectedContactItem as OrganizationViewModel);
						}
					});
		}

		#endregion
	}
}
