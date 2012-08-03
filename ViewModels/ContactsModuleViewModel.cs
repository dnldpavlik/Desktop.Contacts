
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Configuration;
	using System.Linq;
	using System.Reactive;
	using System.Reactive.Linq;
	using Caliburn.Micro;
	using DonPavlik.Desktop.Contacts.Events;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Desktop.Infrastructure.Interfaces;
	using DonPavlik.Domain.Interfaces.Roles;
	using DonPavlik.WikiRepository.Repositories;
	using ReactiveUI;
	using ReactiveUI.Xaml;

	[Export(typeof(IModule))]
	public class ContactsModuleViewModel : 
		ReactiveObject, 
		IModule, 
		IHandle<SaveEvent>,
		IHandle<SelectedPersonEvent>
	{
		private const string ADD_PERSON = "AddPerson";

		private const string EDIT_PERSON = "EditPerson";

		private const string GROUPS = "Groups";

		private object _ActiveItem;

		private readonly Dictionary<string, object> _cachedViews =
			new Dictionary<string, object>();

		private string _activeModuleName;

		private IContact _SelectedPerson;

		private List<string> _Navigation = new List<string>();

		[ImportingConstructor]
		public ContactsModuleViewModel(IEventAggregator events)
		{
			events.Subscribe(this);

			var canAddPerson = this.WhenAny(x => x.Navigation, x => x.ActiveModuleName,
				(b, u) => !string.Equals(u.Value, ADD_PERSON));

			this.Add = new ReactiveAsyncCommand(canAddPerson);
			this.Add
				.Subscribe((arg) => 
				{
					this._activeModuleName = ADD_PERSON;
					this.LoadViewModelFromActiveModuleName(); 
				});

			var canEditPerson = this.WhenAny(x => x.ActiveModuleName, x => x.SelectedPerson,
				(b, u) => u.Value != null && string.Equals(b.Value, GROUPS));

			this.Edit = new ReactiveAsyncCommand(canEditPerson);
			this.Edit
				.Subscribe((arg) =>
				{
					this._activeModuleName = EDIT_PERSON;
					this.LoadViewModelFromActiveModuleName();
				});

			var canOk = this.WhenAny(x => x.Navigation, x => x.ActiveModuleName,
				(b, u) => b.Value.Count() > 1 && !string.Equals(u.Value, GROUPS));

			this.NavigateBack = new ReactiveAsyncCommand(canOk);
			this.NavigateBack
				.Subscribe(
					(obj) =>
					{
						int secondedToLast = this._Navigation.Count() - 2;
						this._activeModuleName = this._Navigation[secondedToLast];
						this._Navigation.RemoveAt(this._Navigation.Count() - 1);
						this.LoadViewModelFromActiveModuleName();
					});

			// Default children
			this._activeModuleName = GROUPS;
			this.LoadViewModelFromActiveModuleName();
		}

		public string ActiveModuleName
		{
			get
			{
				return this._activeModuleName;
			}
		}

		public List<string> Navigation
		{
			get
			{
				return this._Navigation;
			}
		}

		public ReactiveAsyncCommand NavigateBack 
		{ 
			get; 
			protected set; 
		}

		public ReactiveAsyncCommand Add
		{
			get;
			protected set;
		}

		public ReactiveAsyncCommand Edit
		{
			get;
			protected set;
		}

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

		public IContact SelectedPerson
		{
			get
			{
				return this._SelectedPerson;
			}

			set
			{
				this._SelectedPerson = value;
				this.RaisePropertyChanged(x => x.SelectedPerson);
			}
		}

		public void Handle(SelectedPersonEvent selectedPersonEvent)
		{
			if (this.SelectedPerson != selectedPersonEvent.SelectedContacts)
			{
				this._cachedViews.Remove(EDIT_PERSON);
				this.SelectedPerson = selectedPersonEvent.SelectedContacts;
			}
		}

		public void Handle(SaveEvent saveEvent)
		{
			this._cachedViews.Remove(ADD_PERSON);
			this.NavigateBack.Execute(null);
		}

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
	}
}
