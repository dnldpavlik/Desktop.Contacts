
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using Caliburn.Micro;
	using DonPavlik.Desktop.Contacts.Events;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Desktop.Infrastructure.Interfaces;
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
		IModule, 
		IHandle<SelectedPersonEvent>
	{
		#region Private Constants and Fields

		private object _ActiveItem;

		private readonly Dictionary<string, object> _cachedViews =
			new Dictionary<string, object>();

		private ObservableAsPropertyHelper<string> _ActiveModuleName;

		private IContact _SelectedPerson;

		private ReactiveCollection<string> _Navigation = new ReactiveCollection<string>(); 

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ContactsModuleViewModel"/> class.
		/// </summary>
		/// <param name="events">
		/// Event aggregator for dealing with external events.
		/// </param>
		[ImportingConstructor]
		public ContactsModuleViewModel(IEventAggregator events)
		{
			Contract.Requires(events != null, "Events can not be null.");
			events.Subscribe(this);

			this.InitializeDefaults();

			this.SetupAddCommand();
			this.SetupEditCommand();
			this.SetupNavigationCommand();
		}
		
		#region Public Properties

		/// <summary>
		/// Gets the active module name (Should be view name)
		/// </summary>
		public string ActiveModuleName
		{
			get
			{
				return this._ActiveModuleName.Value;
			}
		}

		/// <summary>
		/// Gets the Navigation for this module. 
		/// </summary>
		public ReactiveCollection<string> Navigation
		{
			get
			{
				return this._Navigation;
			}
		}

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
				return this._ActiveItem;
			}

			protected set
			{
				this.RaiseAndSetIfChanged(t => t.ActiveItem, value);
			}
		}

		/// <summary>
		/// Gets the Selected person.
		/// </summary>
		public IContact SelectedPerson
		{
			get
			{
				return this._SelectedPerson;
			}

			protected set
			{
				this.RaiseAndSetIfChanged(x => x.SelectedPerson, value);
			}
		}

		public bool HasSelectedContactItem { get; set; }

		#endregion

		/// <summary>
		/// Handles the selected person event, used enable edit functionality
		/// of contacts.
		/// </summary>
		/// <param name="selectedPersonEvent">Selected person event</param>
		public void Handle(SelectedPersonEvent selectedPersonEvent)
		{
			if (this.SelectedPerson != selectedPersonEvent.SelectedContacts)
			{
				this._cachedViews.Remove(ViewNames.EDIT_PERSON);
				this.SelectedPerson = selectedPersonEvent.SelectedContacts;
			}
		}

		#region Private Methods

		private void LoadViewModelFromActiveModuleName()
		{
			switch (this.ActiveModuleName)
			{
				case ViewNames.ADD_PERSON:
					this.InitializeAddPersonView();
					break;
				case ViewNames.EDIT_PERSON:
					if (!this._cachedViews.ContainsKey(ViewNames.EDIT_PERSON))
					{
						IAddUserViewModel editPerson = IoC.Get<IAddUserViewModel>();
						editPerson.SaveCommand
							.Subscribe((obj) =>
							{
								this.HandleSavePerson(ViewNames.EDIT_PERSON);
							});
						editPerson.EditExistingContact(this.SelectedPerson);

						this._cachedViews.Add(
							ViewNames.EDIT_PERSON,
							editPerson);
					}
					break;
				case ViewNames.ADD_ORGANIZATION:
					if (!this._cachedViews.ContainsKey(ViewNames.ADD_ORGANIZATION))
					{
						IAddOrganizationViewModel addOrganization = 
							IoC.Get<IAddOrganizationViewModel>();
						addOrganization.SaveCommand
							.Subscribe((obj) =>
							{
								this.HandleSavePerson(ViewNames.ADD_ORGANIZATION);
							});

						this._cachedViews.Add(
							ViewNames.ADD_ORGANIZATION,
							addOrganization);
					}
					break;
				case ViewNames.EDIT_ORGANIZATION:
					if (!this._cachedViews.ContainsKey(ViewNames.EDIT_ORGANIZATION))
					{
						IAddOrganizationViewModel editOrganization = 
							IoC.Get<IAddOrganizationViewModel>();
						editOrganization.SaveCommand
							.Subscribe((obj) =>
							{
								this.HandleSavePerson(ViewNames.EDIT_ORGANIZATION);
							});
						editOrganization.EditExistingOrganization(null);

						this._cachedViews.Add(
							ViewNames.EDIT_ORGANIZATION,
							editOrganization);
					}
					break;
				default:
					if (!this._cachedViews.ContainsKey(ViewNames.GROUPS))
					{
						this._cachedViews.Add(
							ViewNames.GROUPS,
							IoC.Get<IGroupViewModel>());
					}
					break;
			}

			this.ActiveItem = this._cachedViews[this.ActiveModuleName];
			this.Navigation.Add(this.ActiveModuleName);
		}

		private void InitializeAddPersonView()
		{
			if (!this._cachedViews.ContainsKey(ViewNames.ADD_PERSON))
			{
				IAddUserViewModel addPerson = IoC.Get<IAddUserViewModel>();
				addPerson.SaveCommand
					.Subscribe((obj) =>
					{
						this.HandleSavePerson(ViewNames.ADD_PERSON);
					});

				this._cachedViews.Add(
					ViewNames.ADD_PERSON,
					addPerson);
			}
		}

		private IGroupViewModel GetGroupView()
		{
			return this._cachedViews[ViewNames.GROUPS] as IGroupViewModel;
		}

		private void HandleSavePerson(string viewName)
		{
			this._cachedViews.Remove(viewName);
			this.NavigateBack.Execute(null);

			if (!this._cachedViews.ContainsKey(ViewNames.GROUPS))
			{
				IGroupViewModel groupView = this.GetGroupView();
				groupView.HandleSave();
			}
		}

		private void SetActiveModuleNameBasedOnGroupState(string peopleName, string organizationName)
		{
			IGroupViewModel groupView = this.GetGroupView();
			if (string.Equals(groupView.ActiveModuleName, ViewNames.PEOPLE))
			{
				this._ActiveModuleName = this
					.WhenAny(x => x.ActiveItem, x => peopleName)
					.ToProperty(this, x => x.ActiveModuleName);
			}
			else if (string.Equals(groupView.ActiveModuleName, ViewNames.ORGANIZATION))
			{
				this._ActiveModuleName = this
					.WhenAny(x => x.ActiveItem, x => organizationName)
					.ToProperty(this, x => x.ActiveModuleName);
			}
		}

		private void SetupAddCommand()
		{
			var canAddPerson = this.WhenAny(x => x.ActiveItem, x => x.ActiveItem,
				(b, u) => !string.Equals(u.Value.GetType().Name, "AddUserViewModel") &&
					!string.Equals(u.Value.GetType().Name, "AddOrganizationViewModel") );

			this.Add = new ReactiveCommand(canAddPerson);
			this.Add
				.Subscribe((arg) =>
				{
					IGroupViewModel groupView = this.GetGroupView();
					if (string.Equals(groupView.ActiveModuleName, ViewNames.PEOPLE))
					{
						this.InitializeAddPersonView();
						this.ActiveItem = this._cachedViews[ViewNames.ADD_PERSON];
					}
					else if (string.Equals(groupView.ActiveModuleName, ViewNames.ORGANIZATION))
					{
						this._ActiveModuleName = this
							.WhenAny(x => x.ActiveItem, x => ViewNames.ADD_ORGANIZATION)
							.ToProperty(this, x => x.ActiveModuleName);
					}
				});
		}

		private void SetupEditCommand()
		{
			var canEditPerson = this.WhenAny(x => x.ActiveModuleName, x => x.HasSelectedContactItem,
				(b, u) => u.Value != false && string.Equals(b.Value, ViewNames.GROUPS));

			this.Edit = new ReactiveCommand(canEditPerson);
			this.Edit
				.Subscribe((arg) =>
				{
					this.SetActiveModuleNameBasedOnGroupState(
						ViewNames.EDIT_PERSON,
						ViewNames.EDIT_ORGANIZATION);
					this.LoadViewModelFromActiveModuleName();
				});
		}

		private void SetupNavigationCommand()
		{
			var canOk = this.WhenAny(x => x.ActiveItem, x => x.ActiveItem,
				(b, u) => !string.Equals(b.Value.GetType().Name, typeof(IGroupViewModel).Name));

			this.NavigateBack = new ReactiveCommand(canOk);
			this.NavigateBack
				.Subscribe((obj) =>
				{
					//this.ActiveModuleName = ViewNames.GROUPS;
					//this.LoadViewModelFromActiveModuleName();
					this.ActiveItem = this._cachedViews[ViewNames.GROUPS];
				});
		}

		private void InitializeDefaults()
		{
			this._ActiveModuleName = this
				.WhenAny(x => x.ActiveItem, x => ViewNames.GROUPS)
				.ToProperty(this, x => x.ActiveModuleName);

			this.LoadViewModelFromActiveModuleName();
		} 

		#endregion
	}
}
