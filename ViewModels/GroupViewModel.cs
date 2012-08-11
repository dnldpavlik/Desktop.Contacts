
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Diagnostics.Contracts;
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

		private object _ActiveItem;

		private readonly IViewFactory _ViewFactory;

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

			this.ShowPeople = new ReactiveCommand();
			this.ShowPeople
				.Subscribe((arg) =>
					{
						this.Processing = true;
						this.RaisePropertyChanged(t => t.Processing);

						IPeopleViewModel peopleViewModel = this._ViewFactory.GetPeopleView();

						this.ActiveItem = peopleViewModel;

						this.Processing = false;
						this.RaisePropertyChanged(t => t.Processing);
					});

			this.ShowOrganizations = new ReactiveCommand();
			this.ShowOrganizations
				.Subscribe((arg) =>
					{
						this.Processing = true;
						this.RaisePropertyChanged(t => t.Processing);

						this.ActiveItem = this._ViewFactory.GetOrganizationView();

						this.Processing = false;
						this.RaisePropertyChanged(t => t.Processing);
					});
		}

		#region Public Properties

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
				if (this._ActiveItem == null)
				{
					this._ActiveItem = this._ViewFactory.GetPeopleView();
				}

				return this._ActiveItem;
			}

			protected set
			{
				this._ActiveItem = value;
				this.RaisePropertyChanged(t => t.ActiveItem);
			}
		}

		/// <summary>
		/// Gets or sets the selected contact item
		/// </summary>
		public object SelectedContactItem { get; protected set; }

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
				this.ShowPeople.Execute(null);
			}
			else
			{
				this.ShowOrganizations.Execute(null);
			}
		}
	}
}
