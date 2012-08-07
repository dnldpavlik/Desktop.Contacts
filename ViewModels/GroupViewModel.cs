
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using Caliburn.Micro;
	using DonPavlik.Desktop.Contacts.Events;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using ReactiveUI;
	using ReactiveUI.Xaml;

	/// <summary>
	/// Group view model class definition
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable"), Export(typeof(IGroupViewModel))]
	public class GroupViewModel : 
		ReactiveObject, 
		IGroupViewModel
	{
		#region Private Constants and Fields

		private const string PEOPLE = "People";

		private const string ORGANIZATION = "Organizations";

		private readonly Dictionary<string, object> _cachedViews =
			new Dictionary<string, object>();

#pragma warning disable 0649
		/// <summary>
		/// Local backing field for the active module name, this is set 
		/// via the ReactiveUI frame work, so this warning is not accurate.
		/// </summary>
		private string _ActiveModuleName;
#pragma warning restore 0649

		private object _ActiveItem;

		private ReactiveAsyncCommand ShowCommand; 

		#endregion

		/// <summary>
		/// Creates a new instance of <see cref="GroupViewModel"/> class.
		/// </summary>
		/// <param name="events">
		/// Event aggregator for dealing with events
		/// </param>
		[ImportingConstructor]
		public GroupViewModel(IEventAggregator events)
		{
			events.Subscribe(this);

			this.ShowCommand = new ReactiveAsyncCommand();
			this.ShowCommand
				.RegisterAsyncAction(this.LoadViewModel);

			this.ShowPeople();
		}

		#region Public Properties

		/// <summary>
		/// Gets the active module name (Should be view)
		/// </summary>
		public string ActiveModuleName
		{
			get
			{
				return this._ActiveModuleName;
			}

			protected set
			{
				this.RaiseAndSetIfChanged(t => t.ActiveModuleName, value);
			}
		}

		/// <summary>
		/// Gets or sets the processing flag that indecates whether the 
		/// group view model is processing.
		/// </summary>
		public bool Processing { get; set; }

		/// <summary>
		/// Gets the active item for the group view model.
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
		
		#endregion
		
		#region Public Methods

		/// <summary>
		/// Action that shows the people view, Delegate for Caliburn.Micro's 
		/// auto binding.
		/// </summary>
		public void ShowPeople()
		{
			this.ShowCommand.Execute(PEOPLE);
		}

		/// <summary>
		/// Action that shows the Organizations view, Delegate for Caliburn.Micro's 
		/// auto binding.
		/// </summary>
		public void ShowOrganizations()
		{
			this.ShowCommand.Execute(ORGANIZATION);
		}

		/// <summary>
		/// Handles the save event. 
		/// </summary>
		/// <param name="saveEvent">
		/// Save event
		/// </param>
		public void HandleSave()
		{
			this._cachedViews.Remove(PEOPLE);
			this.LoadViewModelFromActiveModuleName();
		}

		#endregion

		private void LoadViewModel(object obj)
		{
			this.ActiveModuleName = obj.ToString();
			this.LoadViewModelFromActiveModuleName();
		}

		private void LoadViewModelFromActiveModuleName()
		{
			this.Processing = true;
			this.RaisePropertyChanged(t => t.Processing);
			switch (this.ActiveModuleName)
			{
				case PEOPLE:
					if (!this._cachedViews.ContainsKey(PEOPLE))
					{
						this._cachedViews.Add(
							PEOPLE,
							IoC.Get<IPeopleViewModel>());
					}
					break;
				default:
					if (!this._cachedViews.ContainsKey(ORGANIZATION))
					{
						this._cachedViews.Add(
							ORGANIZATION, 
							IoC.Get<IOrganizationsViewModel>());
					}
					break;
			}

			this.ActiveItem = this._cachedViews[this.ActiveModuleName];
			this.Processing = false;
			this.RaisePropertyChanged(t => t.Processing);
		}
	}
}
