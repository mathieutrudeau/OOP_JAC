using Password_Safe.Models.Credentials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Password_Safe.Pages
{
    /// <summary>
    /// Page where the credentials are listed
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class CredentialsPage : ContentPage
    {
        #region Constructors

        /// <summary>
        /// Basic Construcor
        /// </summary>
        public CredentialsPage()
        {
            InitializeComponent();
            picker_order.SelectedIndex = 2;
        }
        #endregion Constructors

        #region Event Handlers
        /// <summary>
        /// Gets the Credentials when the page appears unless there is no need to
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Checks flag and gets credential if true
            if (App.Changed)
            {
                OrderAndFilter();
            }
            App.Changed = false;
        }

        /// <summary>
        /// Event for whenever the user wants to delete a credential
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Delete_Clicked(object sender, EventArgs e)
        {
            Credential delete = (sender as MenuItem).CommandParameter as Credential;

            // Double check
            if (await DisplayAlert("Warning", $"Are you sure you want to delete {delete.Name}?", "Yes", "No"))
            {
                // Delete and refresh list
                await App.Service.DeleteCredential(delete);
                OrderAndFilter();
            }
        }

        /// <summary>
        /// Event for when a credential is tapped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void credentialList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Credential show = e.Item as Credential;

            // Ask if it is safe to shwo the credential
            if (await DisplayAlert("Warning", "Is it safe to show the credential?", "Yes", "No"))
            {
                // If so, navigate to page
                await Navigation.PushAsync(new ShowCredentialPage(show));
            }
        }

        /// <summary>
        /// Event for when the user wants to add a credential
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NewCredential_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CredentialDetailPage());
        }

        /// <summary>
        /// Event for when the user wants to modify a credential
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Edit_Clicked(object sender, EventArgs e)
        {
            // Get the credential
            // When the credentials are loaded,they do not come with the added fields
            // So if the credential is a level three, get the added fields
            Credential editedCredential = (sender as MenuItem).CommandParameter as Credential;
            if(editedCredential is LevelThreeCredential)
            {
                (editedCredential as LevelThreeCredential).AddedFields = await App.Service.GetCredentialsAddedFields(editedCredential.Id);
            }

            // And then navigate
            await Navigation.PushAsync(new CredentialDetailPage(editedCredential));
        }

        /// <summary>
        /// Handler for changing the LockCombination.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ChangeCombination_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SafeLockPage(true));
        }

        /// <summary>
        /// Event for when the user is filtering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void search_filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            OrderAndFilter();
        }

        /// <summary>
        /// Event for when the user is ordering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picker_order_SelectedIndexChanged(object sender, EventArgs e)
        {
            OrderAndFilter();
        }
        #endregion Event Handlers

        #region Methods
        /// <summary>
        /// Methods that gets the credentials and orders and filters depeneding on what the user selected
        /// </summary>
        private async void OrderAndFilter()
        {
            // Filter the credentials
            IEnumerable<Credential> list = await App.Service.GetAllCredentials();
            list = list.Where(c => c.Name.StartsWith(string.IsNullOrEmpty(search_filter.Text) ? "" : search_filter.Text));

            switch (picker_order.SelectedIndex)
            {
                case 0:
                    list = list.OrderBy(c => c.Name);
                    break;

                case 1:
                    list = list.OrderBy(c => c.Name).Reverse();
                    break;

                case 3:
                    list = list.Reverse();
                    break;
            }

            credentialList.ItemsSource = list;
        }
        #endregion Methods
    }
}
