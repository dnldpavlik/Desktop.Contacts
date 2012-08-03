
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Caliburn.Micro;
	using DonPavlik.Desktop.Contacts.Events;
	using DonPavlik.Desktop.Contacts.Interfaces;
	using DonPavlik.Desktop.Infrastructure;
	using ReactiveUI;
	using ReactiveUI.Xaml;

	[Export(typeof(IGroupViewModel))]
	public class GroupViewModel : 
		ReactiveObject, 
		IHandle<SaveEvent>, 
		IGroupViewModel
	{
		private const string PEOPLE = "People";

		private const string ORGANIZATION = "Organizations";

		private readonly Dictionary<string, object> _cachedViews =
			new Dictionary<string, object>();

		private string _ActiveModuleName;

		private object _ActiveItem;

		private readonly ReactiveAsyncCommand ShowCommand;

		[ImportingConstructor]
		public GroupViewModel(IEventAggregator events)
		{
			events.Subscribe(this);

			this.ShowCommand = new ReactiveAsyncCommand();
			this.ShowCommand
				.RegisterAsyncAction(this.LoadViewModel);

			this.ShowPeople();
		}

		public void ShowPeople()
		{
			this.ShowCommand.Execute(PEOPLE);
		}

		public void ShowOrganizations()
		{
			this.ShowCommand.Execute(ORGANIZATION);
		}

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

		public bool Processing { get; set; }

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

		public void Handle(SaveEvent saveEvent)
		{
			this._cachedViews.Remove(PEOPLE);
			this.LoadViewModelFromActiveModuleName();
		}

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
						this._cachedViews.Add(ORGANIZATION, new OrganizationsViewModel());
					}
					break;
			}

			this.ActiveItem = this._cachedViews[this.ActiveModuleName];
			this.Processing = false;
			this.RaisePropertyChanged(t => t.Processing);
		}
	}
}
