
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.ComponentModel.Composition;
	using System.Diagnostics.Contracts;
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

		private object _ActiveItem;

		private readonly IViewFactory _ViewFactory;

		private object _SelectedContactItem;

		#endregion

		/// <summary>
		/// Creates a new instance of <see cref="GroupViewModel"/> class.
		/// </summary>
		/// <param name="viewFactory">
		/// View factory
		/// </param>
		[ImportingConstructor]
		public GroupViewModel(IViewFactory viewFactory)
		{
			Contract.Requires(viewFactory != null, "View Factory can not be null.");
			this._ViewFactory = viewFactory;

			this.SetupShowPeopleCommand();
			this.SetupShowOrganizationsCommand();

			this.ShowPeople.Execute(null);
		}

		#region Public Properties

		/// <summary>
		/// Gets the Organization view model
		/// </summary>
		public IOrganizationsViewModel OrganizationsViewModel { get; protected set; }

		/// <summary>
		/// Gets the People View Model
		/// </summary>
		public IPeopleViewModel PeopleViewModel { get; protected set; }

		/// <summary>
		/// Gets the processing flag that indecates whether the 
		/// group view model is processing.
		/// </summary>
		public bool Processing { get; protected set; }

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

		/// <summary>
		/// Gets the selected contact item
		/// </summary>
		public object SelectedContactItem
		{
			get
			{
				return this._SelectedContactItem;
			}

			protected set
			{
				this._SelectedContactItem = value;
				this.RaisePropertyChanged(x => x.SelectedContactItem);
			}
		}

		/// <summary>
		/// Gets the Show People Command
		/// </summary>
		public ReactiveCommand ShowPeople
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the Show Organizations Command
		/// </summary>
		public ReactiveCommand ShowOrganizations
		{
			get;
			protected set;
		}

		#endregion

		/// <summary>
		/// Removes an existing organization
		/// </summary>
		/// <param name="organization">Organization to be removed</param>
		public void RemoveExistingOrganization(OrganizationViewModel organization)
		{
			this.OrganizationsViewModel.RemoveExistingOrganization(organization);
		}

		/// <summary>
		/// Removes existing contact
		/// </summary>
		/// <param name="contact">Contact that is be destroyed</param>
		public void RemoveExistingContact(ContactViewModel contact)
		{
			this.PeopleViewModel.Remove.Execute(contact);
		}

		/// <summary>
		/// Handles the save event. 
		/// </summary>
		/// <param name="saveType">
		/// Save event
		/// </param>
		public void HandleSave(string saveType)
		{
			this._ViewFactory.ClearView(saveType);

			if (saveType == ViewNames.PEOPLE)
			{
				this.PeopleViewModel = null;
				this.ShowPeople.Execute(null);
			}
			else
			{
				this.OrganizationsViewModel = null;
				this.ShowOrganizations.Execute(null);
			}
		}

		/// <summary>
		/// Sets up the show people command, and hooks up the subscription to 
		/// the People View model's Selected item
		/// </summary>
		private void SetupShowPeopleCommand()
		{
			this.ShowPeople = new ReactiveCommand();
			this.ShowPeople
				.Subscribe((arg) =>
				{
					this.Processing = true;
					this.RaisePropertyChanged(t => t.Processing);

					if (this.PeopleViewModel == null)
					{
						this.PeopleViewModel = this._ViewFactory.GetPeopleView();

						this.ObservableForProperty(x => x.PeopleViewModel.SelectedItem)
							.Subscribe(x => this.SelectedContactItem = x.Value);

						this.PeopleViewModel.Remove
							.Subscribe(_ =>
								{
									this.HandleSave(ViewNames.PEOPLE);
								});
					}
					this.ActiveItem = this.PeopleViewModel;

					this.Processing = false;
					this.RaisePropertyChanged(t => t.Processing);
				});
		}

		/// <summary>
		/// Sets up the show organizations command and hooks up the subscrition 
		/// to the Organizations view model's selected item
		/// </summary>
		private void SetupShowOrganizationsCommand()
		{
			this.ShowOrganizations = new ReactiveCommand();
			this.ShowOrganizations
				.Subscribe((arg) =>
				{
					this.Processing = true;
					this.RaisePropertyChanged(t => t.Processing);

					if (this.OrganizationsViewModel == null)
					{
						this.OrganizationsViewModel = this._ViewFactory.GetOrganizationView();
						this.ObservableForProperty(x => x.OrganizationsViewModel.SelectedItem)
							.Subscribe(x => this.SelectedContactItem = x.Value);
					}

					this.ActiveItem = this.OrganizationsViewModel;

					this.Processing = false;
					this.RaisePropertyChanged(t => t.Processing);
				});
		}
	}
}
