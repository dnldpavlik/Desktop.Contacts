
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.ComponentModel.Composition;
	using System.Diagnostics.Contracts;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Desktop.Infrastructure.Interfaces;
	using DonPavlik.Domain.Interfaces;
	using DonPavlik.Domain.Interfaces.Roles;
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

		private object _ActiveItem;

		private readonly IViewFactory _ViewFactory;

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
		/// Gets the active item, which is the active view
		/// </summary>
		public object ActiveItem
		{
			get
			{
				if (this._ActiveItem == null)
				{
					this._ActiveItem = this._ViewFactory.GetGroupView();
				}

				return this._ActiveItem;
			}

			protected set
			{
				this.RaiseAndSetIfChanged(t => t.ActiveItem, value);
			}
		}

		#endregion

		#region Private Methods

		private void InitializeCommands()
		{
			this.SetupAddCommand();
			this.SetupEditCommand();
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
					IGroupViewModel groupView = this._ViewFactory.GetGroupView();
					if (string.Equals(groupView.ActiveItem.GetType().Name, "PeopleViewModel"))
					{
						IAddPersonViewModel addPersonView = this._ViewFactory.GetAddPersonView();

						addPersonView.SaveCommand
							.Subscribe((obj) =>
							{
								this._ViewFactory.ClearView(ViewNames.ADD_PERSON);
								groupView.HandleSave(ViewNames.PEOPLE);
								this.NavigateBack.Execute(null);
							});

						this.ActiveItem = addPersonView;
					}
					else if (string.Equals(groupView.ActiveItem.GetType().Name, "OrganizationsViewModel"))
					{
						IAddOrganizationViewModel addOrganization = this._ViewFactory.GetAddOrganizationView();

						addOrganization.SaveCommand
							.Subscribe((obj) =>
							{
								this._ViewFactory.ClearView(ViewNames.ADD_ORGANIZATION);
								groupView.HandleSave(ViewNames.ORGANIZATION);
								this.NavigateBack.Execute(null);
							});

						this.ActiveItem = addOrganization;
					}
				});
		}

		private void SetupEditCommand()
		{
			var canEditPerson = this.WhenAny(x => x.ActiveItem,
				(b) => 
					{
						IGroupViewModel groupViewModel = b.Value as IGroupViewModel;

						if (groupViewModel == null)
						{
							return false;
						}

						if (groupViewModel.SelectedContactItem != null)
						{
							return true;
						}

						return false;
					});

			this.Edit = new ReactiveCommand(canEditPerson);
			this.Edit
				.Subscribe((arg) =>
				{
					IGroupViewModel groupView = this._ViewFactory.GetGroupView();
					if (string.Equals(groupView.ActiveItem.GetType().Name, "PeopleViewModel"))
					{
						IAddPersonViewModel addPersonView = this._ViewFactory.GetEditPersonView();

						addPersonView.EditExistingContact(
							groupView.SelectedContactItem as IContact);

						addPersonView.SaveCommand
							.Subscribe((obj) =>
							{
								this._ViewFactory.ClearView(ViewNames.EDIT_PERSON);
								groupView.HandleSave(ViewNames.PEOPLE);
								this.NavigateBack.Execute(null);
							});

						this.ActiveItem = addPersonView;
					}
					else if (string.Equals(groupView.ActiveItem.GetType().Name, "OrganizationsViewModel"))
					{
						IAddOrganizationViewModel addOrganization = this._ViewFactory.GetEditOrganizationView();

						addOrganization.EditExistingOrganization(
							groupView.SelectedContactItem as IOrganization);

						addOrganization.SaveCommand
							.Subscribe((obj) =>
							{
								this._ViewFactory.ClearView(ViewNames.EDIT_ORGANIZATION);
								groupView.HandleSave(ViewNames.ORGANIZATION);
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

		#endregion
	}
}
