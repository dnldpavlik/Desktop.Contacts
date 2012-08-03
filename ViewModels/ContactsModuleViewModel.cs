
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
		IHandle<SaveEvent>,
		IHandle<SelectedPersonEvent>
	{
		#region Private Constants and Fields

		private const string ADD_PERSON = "AddPerson";

		private const string EDIT_PERSON = "EditPerson";

		private const string GROUPS = "Groups";

		private object _ActiveItem;

		private readonly Dictionary<string, object> _cachedViews =
			new Dictionary<string, object>();

		private string _activeModuleName;

		private IContact _SelectedPerson;

		private List<string> _Navigation = new List<string>(); 

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

			this.SetupAddCommand();
			this.SetupEditCommand();
			this.SetupNavigationCommand();
			this.InitializeDefaults();
		}
		
		#region Public Properties

		/// <summary>
		/// Gets the active module name (Should be view name)
		/// </summary>
		public string ActiveModuleName
		{
			get
			{
				return this._activeModuleName;
			}
		}

		/// <summary>
		/// Gets the Navigation for this module. 
		/// </summary>
		public List<string> Navigation
		{
			get
			{
				return this._Navigation;
			}
		}

		/// <summary>
		/// Gets the Navigation back command that controls moving backwards.
		/// </summary>
		public ReactiveAsyncCommand NavigateBack
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the add command that shows the add view for the active
		/// subject matter.
		/// </summary>
		public ReactiveAsyncCommand Add
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the edit command that shows the edit view for the active
		/// subject matter.
		/// </summary>
		public ReactiveAsyncCommand Edit
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
				this._ActiveItem = value;
				this.RaisePropertyChanged(t => t.ActiveItem);
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
				this._SelectedPerson = value;
				this.RaisePropertyChanged(x => x.SelectedPerson);
			}
		}
		
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
				this._cachedViews.Remove(EDIT_PERSON);
				this.SelectedPerson = selectedPersonEvent.SelectedContacts;
			}
		}

		/// <summary>
		/// Handles the save event, used to force updates for the subject 
		/// list view.
		/// </summary>
		/// <param name="saveEvent">Save event</param>
		public void Handle(SaveEvent saveEvent)
		{
			this._cachedViews.Remove(ADD_PERSON);
			this.NavigateBack.Execute(null);
		}
		
		#region Private Methods

		private void LoadViewModelFromActiveModuleName()
		{
			switch (this._activeModuleName)
			{
				case ADD_PERSON:
					if (!this._cachedViews.ContainsKey(ADD_PERSON))
					{
						this._cachedViews.Add(
							ADD_PERSON,
							IoC.Get<IAddUserViewModel>());
					}
					break;
				case EDIT_PERSON:
					if (!this._cachedViews.ContainsKey(EDIT_PERSON))
					{
						IAddUserViewModel editPerson = 
							IoC.Get<IAddUserViewModel>();

						editPerson.EditExistingContact(this.SelectedPerson);

						this._cachedViews.Add(
							EDIT_PERSON,
							editPerson);
					}
					break;
				default:
					if (!this._cachedViews.ContainsKey(GROUPS))
					{
						this._cachedViews.Add(
							GROUPS,
							IoC.Get<IGroupViewModel>());
					}
					break;
			}

			this.ActiveItem = this._cachedViews[this._activeModuleName];
			this._Navigation.Add(this._activeModuleName);
			this.RaisePropertyChanged(x => x.ActiveModuleName);
			this.RaisePropertyChanged(x => x.Navigation);
		}

		private void SetupAddCommand()
		{
			var canAddPerson = this.WhenAny(x => x.Navigation, x => x.ActiveModuleName,
				(b, u) => !string.Equals(u.Value, ADD_PERSON));

			this.Add = new ReactiveAsyncCommand(canAddPerson);
			this.Add
				.Subscribe((arg) =>
				{
					this._activeModuleName = ADD_PERSON;
					this.LoadViewModelFromActiveModuleName();
				});
		}

		private void SetupEditCommand()
		{
			var canEditPerson = this.WhenAny(x => x.ActiveModuleName, x => x.SelectedPerson,
				(b, u) => u.Value != null && string.Equals(b.Value, GROUPS));

			this.Edit = new ReactiveAsyncCommand(canEditPerson);
			this.Edit
				.Subscribe((arg) =>
				{
					this._activeModuleName = EDIT_PERSON;
					this.LoadViewModelFromActiveModuleName();
				});
		}

		private void SetupNavigationCommand()
		{
			var canOk = this.WhenAny(x => x.Navigation, x => x.ActiveModuleName,
				(b, u) => b.Value.Count() > 1 && !string.Equals(u.Value, GROUPS));

			this.NavigateBack = new ReactiveAsyncCommand(canOk);
			this.NavigateBack
				.Subscribe((obj) =>
				{
					int secondedToLast = this._Navigation.Count() - 2;
					this._activeModuleName = this._Navigation[secondedToLast];
					this._Navigation.RemoveAt(this._Navigation.Count() - 1);
					this.LoadViewModelFromActiveModuleName();
				});
		}

		private void InitializeDefaults()
		{
			this._activeModuleName = GROUPS;
			this.LoadViewModelFromActiveModuleName();
		} 

		#endregion
	}
}
