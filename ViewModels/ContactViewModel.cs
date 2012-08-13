
namespace DonPavlik.Desktop.Contacts.ViewModels
{
	using System;
	using System.IO;
	using System.Threading.Tasks;
	using System.Windows.Media.Imaging;
	using DonPavlik.Domain.Interfaces.Roles;
	using ReactiveUI;

	/// <summary>
	/// Contact view model class definition
	/// </summary>
	public class ContactViewModel : ReactiveObject
	{
		private IContact _contact;

		private BitmapImage _image = null;

		private readonly string _Path = string.Empty;

		/// <summary>
		/// Initializes a new instance of <see cref="ContactViewModel"/> class
		/// </summary>
		/// <param name="contact">Contact to be modeled</param>
		public ContactViewModel(IContact contact, string path)
		{
			this._contact = contact;
			this._Path = path;
		}

		/// <summary>
		/// Gets or sets the full name of the contact
		/// </summary>
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

		/// <summary>
		/// Gets or sets the image url for the contact
		/// </summary>
		public BitmapImage ImageUrl
		{
			get
			{
				if (this._image == null && this._contact.Image != null)
				{
					System.Windows.Application.Current.Dispatcher.InvokeAsync( 
						async () => 
						{
							string path = Path.Combine(
								this._Path,
								this._contact.Image
									.Replace("local:", string.Empty)
									.Replace("[[", string.Empty)
									.Replace("]]", string.Empty));


							this.ImageUrl = await GetImageAsBitmap(path); 
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

		/// <summary>
		/// Gets the contact
		/// </summary>
		public IContact Contact
		{
			get
			{
				return this._contact;
			}
		}

		/// <summary>
		/// Refreshes the image binding
		/// </summary>
		public void RefreshImageBinding()
		{
			this.RaisePropertyChanged(x => x.ImageUrl);
		}

		/// <summary>
		/// Gets image as bitmap in an async way.
		/// 
		/// Disabled warning 1998, because you can not await new BitmapImage
		/// </summary>
		/// <param name="Uri">
		/// The location where the image.
		/// </param>
		/// <returns>
		/// Bitmap image populated with the bytes from the uri
		/// </returns>
#pragma warning disable 1998
		private async Task<BitmapImage> GetImageAsBitmap(string Uri)
		{
			BitmapImage image = new BitmapImage(new Uri(Uri, UriKind.Absolute));

			return image;
		}
#pragma warning restore 1998
	}
}
