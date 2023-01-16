using PhoneBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using static PhoneBook.App;

namespace PhoneBook.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactDetailPage : ContentPage
    {

        #region Local Fields

        /// <summary>
        /// Tracks if input fields are enabled or not.
        /// </summary>
        private bool isEnabled { get; set; }

        #endregion Local Fields

        #region Constructors 

        /// <summary>
        /// Constructs a ContactDetailPage instance.
        /// </summary>
        public ContactDetailPage()
        {
            InitializeComponent();

            // Prepare the Page to Create a Contact
            SetNewMode();
        }

        /// <summary>
        /// Constructs a ContactDetailPage instance.
        /// </summary>
        /// <param name="contact">Contact to be Displayed.</param>
        public ContactDetailPage(Contact contact)
        {
            InitializeComponent();

            // Prepare the Page to View the Contact
            SetViewMode(contact);
        }

        #endregion Constructors

        #region Handlers

        /// <summary>
        /// Handler for the Save Button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Save_Clicked(object sender, EventArgs e)
        {
            // Create the Contact
            Contact newContact = BindingContext as Contact;

            // Check that a name has been given
            if(string.IsNullOrEmpty(newContact.FirstName) && string.IsNullOrEmpty(newContact.LastName))
            {
                DisplayAlert("Error", "Your contact needs a name... :)", "OK");
                return;
            }

            // Save the Contact 
            await Service.SaveContact(newContact);

            // Go back to the Contacts Page
            Navigation.PopAsync();
        }

        /// <summary>
        /// Handler for the Call Button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Call_Clicked(object sender, EventArgs e)
        {
            // Calls the Contact
            Contact contact = (sender as Button).CommandParameter as Contact;

            bool result = await DisplayAlert("Call", "Do you want to call " + contact.PhoneNumber+"?", "Yes", "No");

            if(result)
                PhoneDialer.Open(contact.PhoneNumber);
        }

        /// <summary>
        /// Handler for the Edit Button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit_Clicked(object sender, EventArgs e)
        {
            // Switch between View and Edit as needed
            if (isEnabled)
                SetViewMode();
            else
                SetEditMode();
        }

        #endregion Handlers

        #region Other Methods

        /// <summary>
        /// Prepares the Page to View a Contact.
        /// </summary>
        /// <param name="contact">Contact to be Displayed.</param>
        private void SetViewMode(Contact contact = null)
        {
            // Bind the contact if needed
            if (contact != null)
            {
                BindingContext = contact;
                isEnabled = true;
            }

            // Set the Title
            Title = "Contact";

            // Make the Save Button Become the Call Button
            saveBtn.Text = string.Format("Call {0}", (BindingContext as Contact).FullName);
            saveBtn.Clicked -= Save_Clicked;
            saveBtn.Clicked += Call_Clicked;

            // Set the Menu Option to Edit
            editOption.Text = "Edit";

            // Disable Editing
            SwitchIsEnabled();
        }

        /// <summary>
        /// Prepares the Page to Edit a Contact.
        /// </summary>
        /// <param name="contact">Contact to be Displayed.</param>
        private void SetEditMode(Contact contact = null)
        {
            // Bind the contact if needed
            if (contact != null)
                BindingContext = contact;

            // Enable Editing
            SwitchIsEnabled();

            // Set the Title
            Title = "Edit Contact";

            // Make the Call Button become the Save Button
            saveBtn.Text = "Save";
            saveBtn.Clicked -= Call_Clicked;
            saveBtn.Clicked += Save_Clicked;
            
            // Set the Menu Option to View
            editOption.Text = "View";
        }

        /// <summary>
        /// Prepares the Page to Add a New Contact.
        /// </summary>
        private void SetNewMode()
        {
            BindingContext = new Contact();

            // Set the Title
            Title = "New Contact";

            // Remove the Option to Edit
            ToolbarItems.Remove(editOption);
        }

        /// <summary>
        /// Switches the IsEnabled Property of some Cells.
        /// </summary>
        private void SwitchIsEnabled()
        {
            isEnabled = !isEnabled;

            firstName.IsEnabled = isEnabled;
            lastName.IsEnabled = isEnabled;
            phone.IsEnabled = isEnabled;
            email.IsEnabled = isEnabled;
            blocked.IsEnabled = isEnabled;
        }

        #endregion Other Methods

    }
}