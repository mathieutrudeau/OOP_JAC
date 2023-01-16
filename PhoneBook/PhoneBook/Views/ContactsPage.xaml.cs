using PhoneBook.Models;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static PhoneBook.App;

namespace PhoneBook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactsPage : ContentPage
    {

        #region Constructors

        /// <summary>
        /// Constructs a new ContactsPage instance.
        /// </summary>
        public ContactsPage()
        {
            InitializeComponent();

            // Initialize the Contact List
            ReloadData();
        }

        #endregion Constructors

        #region Handlers

        /// <summary>
        /// Handler for the Delete Menu Option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Delete_Clicked(object sender, EventArgs e)
        {
            // Retrieve the Contact to be deleted
            Contact contact = (sender as MenuItem).CommandParameter as Contact;

            // Ask for user's confirmation
            bool result = await DisplayAlert("Warning", "Are you sure you want to delete " + contact.FullName+"?", "Yes", "No");

            // If the user accepts, delete the contact and reload the data
            if (result)
            {
                await Service.DeleteContact(contact);
                ReloadData();
            }
        }

        /// <summary>
        /// Handler for the Add Menu Option.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Clicked(object sender, EventArgs e)
        {
            // Show the Contact Detail Page to create a contact
            Navigation.PushAsync(new ContactDetailPage());
        }

        /// <summary>
        /// Handler for when a Contact is Tapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contact_Tapped(object sender, EventArgs e)
        {
            // Retrieve the Contact that was tapped
            Contact contact = (sender as TextCell).CommandParameter as Contact;
            
            // Show the Contact Detail Page with the tapped contact info
            Navigation.PushAsync(new ContactDetailPage(contact));
        }

        /// <summary>
        /// Handler for when the Page Appears.
        /// </summary>
        protected override void OnAppearing()
        {
            // Reload the data if needed
            ReloadData();
            base.OnAppearing();
        }

        #endregion Handlers

        #region Other Methods

        /// <summary>
        /// Reloads the Data for the Contacts if it has changed.
        /// </summary>
        private void ReloadData()
        {
            // If a the Contacts have been modified, reload the contacts
            if (Service.IsReloadNeeded())
                contactsList.ItemsSource = Service.GetAllContacts();

            contactsList.SelectedItem = null;
        }

        #endregion Other Methods

    }
}