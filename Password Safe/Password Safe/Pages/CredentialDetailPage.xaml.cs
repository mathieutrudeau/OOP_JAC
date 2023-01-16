using Password_Safe.Models.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Password_Safe.Pages
{
    /// <summary>
    /// Page that lets the user delete or modify a credential
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CredentialDetailPage : ContentPage
    {
        #region Constants
        // Information about the credential
        private Credential save;
        private int credentialLevel = 1;

        // Use when a credential is edited for the custom fields
        private bool isEdit;
        private int editId = 0;

        // Added components
        private Button button_show;

        private EntryCell entry_email;
        private EntryCell entry_phone;

        private EntryCell entry_field;
        private EntryCell entry_info;
        #endregion Constants

        #region Constructors
        /// <summary>
        /// Basic Constructor
        /// </summary>
        public CredentialDetailPage()
        {
            InitializeComponent();

            save = new LevelOneCredential();
            BindingContext = save;
        }

        /// <summary>
        /// Constructor that takes in a credential
        /// Modifies the UI depending on the level
        /// </summary>
        /// <param name="credential"></param>
        public CredentialDetailPage(Credential credential)
        {
            InitializeComponent();

            // Sets flag
            isEdit = true;

            // Sets binding
            save = credential;
            BindingContext = save;

            // Modifies UI
            if(credential is LevelTwoCredential || credential is LevelThreeCredential)
            {
                AddLevel2();

                if(credential is LevelThreeCredential)
                {
                    AddLevel3();
                }
            }
        }
        #endregion Constructors

        #region Event Handlers
        /// <summary>
        /// Event for the button that either modifies the level if the credential or adds a new custom field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Credential_Clicked(object sender, EventArgs e)
        {
            switch(credentialLevel) {

                // Creates a higher credential based on the current one and adds ui elements
                case 1:
                    save = new LevelTwoCredential(save as LevelOneCredential);
                    AddLevel2();
                    break;

                // Creates a higher credential based on the cureent one and adds ui elements
                case 2:
                    save = new LevelThreeCredential(save as LevelTwoCredential);
                    AddLevel3();
                    break;

                // Saves the new field and resets the ui
                default:
                    (save as LevelThreeCredential).AddedFields.Add(new CustomFields()
                    {
                        Type = entry_field.Text,
                        Info = entry_info.Text
                    });

                    // Edit mode only:
                    // If the field is meant to be updated, puts the id
                    if(isEdit && editId != 0)
                    {
                        (save as LevelThreeCredential).AddedFields.Last().Id = editId;
                    }
                    editId = 0;

                    // Reset and change ui
                    entry_field.Text = string.Empty;
                    entry_info.Text = string.Empty;

                    button_show.Text = $"Show Added Fields ({(save as LevelThreeCredential).AddedFields.Count})";
                    break;
            }
        }

        /// <summary>
        /// Event handler for when the credential is ready to be saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Save_Clicked(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(entry_name.Text))
            {
                await DisplayAlert("Error", "The credential must at least have a name.", "Okay");
                return;
            }

            // Save, set flag, leave
            await App.Service.SaveCredential(save);
            App.Changed = true;
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Event handler for when the added fields have to be shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Show_Clicked(object sender, EventArgs e)
        {
            // Get an array of the added fields in a presentable way
            string[] options = GetStringList();

            // Display to the user
            string action = await DisplayActionSheet("Click on a field to modify it.", "Cancel", null, options);

            // Find out which field was selected
            int i = -1;
            while(action != "Cancel" && (save as LevelThreeCredential).AddedFields[++i].ToString() != action) { }

            // If i is not Cancel
            if (i != -1)
            {
                // Ask the user what he wants to do
                string decision = await DisplayActionSheet("Do you wish to remove or edit this field?", "Delete", "Edit");

                if (decision == "Edit")
                {
                    // Edit:

                    // Put the information in the entries
                    entry_field.Text = (save as LevelThreeCredential).AddedFields[i].Type;
                    entry_info.Text = (save as LevelThreeCredential).AddedFields[i].Info;

                    // If in edit mode (for the credential), get the id since it got one when saved in the db
                    if (isEdit)
                    {
                        editId = (save as LevelThreeCredential).AddedFields[i].Id;
                    }
                }
                else if (decision == "Delete")
                {
                    // Delete:

                    // Remove from the credential
                    (save as LevelThreeCredential).AddedFields.RemoveAt(i);

                    // Update UI
                    button_show.Text = $"Show Added Fields ({(save as LevelThreeCredential).AddedFields.Count})";

                    if (isEdit)
                    {
                        await App.Service.DeleteCustomField((save as LevelThreeCredential).AddedFields[i]);
                    }
                }


            }
        }
#endregion Event Handlers

        /// <summary>
        /// Adds two entries: Email and Phone
        /// </summary>
        private void AddLevel2()
        {
            // Create the email entry with binding
            entry_email = new EntryCell()
            {
                Label = "Email Address: ",
                Keyboard = Keyboard.Email,
                BindingContext = save,
            };
            entry_email.SetBinding(EntryCell.TextProperty, "Email");

            // Create the phone entry with binding
            entry_phone = new EntryCell()
            {
                Label = "Phone Number: ",
                Keyboard = Keyboard.Telephone,
                BindingContext = save,
            };
            entry_phone.SetBinding(EntryCell.TextProperty, "PhoneNumber");

            // Creates a section for the table
            TableSection section2 = new TableSection()
            {
                Title = "Level 2"
            };

            // Adds the entries in the section
            // And the section in the table
            section2.Add(entry_email);
            section2.Add(entry_phone);
            table.Root.Add(section2);

            // Updates ui and variable
            button_credentialAdd.Text = "Next Level (Your own)";
            credentialLevel++;
        }

        /// <summary>
        /// Adds a button and two entries to the ui for custom fields
        /// </summary>
        private void AddLevel3()
        {
            // Creates a cell and puts a new button inside
            // The button has his click event
            ViewCell cell = new ViewCell();
            button_show = new Button()
            {
                Text = $"Show Added Fields ({(save as LevelThreeCredential).AddedFields.Count})",
            };
            button_show.Clicked += Show_Clicked;
            cell.View = button_show;

            // Creates an entry for the Type
            entry_field = new EntryCell()
            {
                Label = "Field: ",
                Keyboard = Keyboard.Text,
            };

            // Creates an entry for the Info
            entry_info = new EntryCell()
            {
                Label = "Info: ",
                Keyboard = Keyboard.Text
            };

            // Creates a section for the table
            TableSection section3 = new TableSection()
            {
                Title = "Level 3"
            };

            // Adds the components in the section
            // And the section in the table
            section3.Add(cell);
            section3.Add(entry_field);
            section3.Add(entry_info);
            table.Root.Add(section3);

            // Updates the UI and variable
            button_credentialAdd.Text = "Save new field";
            credentialLevel++;
        }

        /// <summary>
        /// Puts the Custom Fields in a presentable way in an array
        /// </summary>
        /// <returns></returns>
        private string[] GetStringList()
        {
            // Loop, ToString(), return
            string[] options = new string[(save as LevelThreeCredential).AddedFields.Count];
            int count = 0;
            foreach (CustomFields field in (save as LevelThreeCredential).AddedFields)
            {
                options[count++] = field.ToString();
            }
            return options;
        }
    }
}