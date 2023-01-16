using Password_Safe.Models.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Password_Safe.Pages
{
    /// <summary>
    /// Page that show a credential
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowCredentialPage : ContentPage
    {
        #region Properties
        Credential credential;
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor that takes in one credential
        /// Saves that credential locally and sets title
        /// </summary>
        /// <param name="cred"></param>
        public ShowCredentialPage(Credential cred)
        {
            InitializeComponent();

            credential = cred;
            Title = credential.Name;
        }
        #endregion Constructors

        #region Event Handlers
        /// <summary>
        /// Event when the page appears
        /// Gets all the information and displays it
        /// </summary>
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            listInfo.ItemsSource = await GetFields(credential);
        }
        #endregion Event Handlers

        #region Methods
        /// <summary>
        /// Method that gets all the fields that gets the level of the credential
        /// Sends that type to a method that will show every thing contained inside
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        private async Task<List<CustomFields>> GetFields(Credential credential)
        {
            List<CustomFields> info = new List<CustomFields>();

            AddFields(info, credential, typeof(LevelOneCredential));

            if(credential is LevelTwoCredential || credential is LevelThreeCredential)
            {
                AddFields(info, credential, typeof(LevelTwoCredential));

                if(credential is LevelThreeCredential)
                {
                    foreach (CustomFields field in await App.Service.GetCredentialsAddedFields(credential.Id))
                    {
                        info.Add(field);
                    }
                }
            }

            return info;
        }

        /// <summary>
        /// Get the name of each property of a method type with the info contained and adds it to the ui
        /// </summary>
        /// <param name="info"></param>
        /// <param name="credential"></param>
        /// <param name="classType"></param>
        private void AddFields(List<CustomFields> info, Credential credential, Type classType)
        {
            // Loops every property of that class
            foreach (PropertyInfo propertyInfo in classType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                // Do not show id or name
                if (propertyInfo.Name.Equals("Id") || propertyInfo.Name.Equals("Name"))
                    continue;

                // Adds that property and the info in a new Custom Field and adds it to the list
                info.Add(new CustomFields()
                {
                    Type = propertyInfo.Name,
                    Info = string.IsNullOrWhiteSpace(propertyInfo.GetValue(credential).ToString()) ? "------" : propertyInfo.GetValue(credential).ToString()
                });
            }
        }
        #endregion Methods
    }
}