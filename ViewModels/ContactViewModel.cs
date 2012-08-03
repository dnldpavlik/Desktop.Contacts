using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Caliburn.Micro;
using DonPavlik.Domain.Interfaces;
using DonPavlik.Domain.Interfaces.Roles;
using ReactiveUI;

namespace DonPavlik.Desktop.Contacts.ViewModels
{
	public class ContactViewModel : ReactiveObject
	{
		private IContact _contact;

		private BitmapImage _image = null;

		public ContactViewModel(IContact contact)
		{
			this._contact = contact;
		}

		public string FullName
		{
			get
			{
				return this._contact.PrimaryName.FullName;
			}

			set
			{
				this._contact.PrimaryName.FullName = value;
				this.RaisePropertyChanged(x => x.FullName);
			}
		}

		public BitmapImage ImageUrl
		{
			get
			{
				if (this._image == null && this._contact.Image != null)
				{
					System.Windows.Application.Current.Dispatcher.InvokeAsync( 
						async () => 
						{ 
							this.ImageUrl = await GetImageAsBitmap(
								this._contact.Image
									.Replace("file:", string.Empty)
									.Replace("{{", string.Empty)
									.Replace("}}", string.Empty)); 
						});
				}

				return this._image;
			}

			set 
			{
				if (value != null)
				{
					this._image = value;
					this.RaisePropertyChanged(x => x.ImageUrl);
				}
			}
		}

		public IContact Contact
		{
			get
			{
				return this._contact;
			}
		}

		public void RefreshImageBinding()
		{
			this.RaisePropertyChanged(x => x.ImageUrl);
		}

		private async Task<BitmapImage> GetImageAsBitmap(string Uri)
		{
			BitmapImage image = new BitmapImage(new Uri(Uri, UriKind.Absolute));

			return image;
		}
	}
}
