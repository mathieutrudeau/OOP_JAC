using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Password_Safe.App;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;
using Password_Safe.Models.Lock_Combinations;
using System.Threading;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Password_Safe.Pages
{
    /// <summary>
    /// Safe Lock Page.
    /// 
    /// Two Options:
    /// - Four Number Combination
    /// - Secure Password
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SafeLockPage : ContentPage
    {

        #region Constants

        /// <summary>
        /// UI Options.
        /// </summary>
        public enum CombinationOptions { FourNumberCombination = 0, Password = 1, NotSet=2}

        /// <summary>
        /// Max number of incorrect attempts at entering a combination.
        /// </summary>
        private const int MAX_NUMBER_OF_TRIES = 5;

        /// <summary>
        /// Factor by which the locked time increases every time the app gets locked.
        /// </summary>
        private const int LOCK_TIME_INCREASE_FACTOR = 2;

        /// <summary>
        /// Minimum amount of minutes that the App locks when entering a locked state.
        /// </summary>
        private const int MIN_MINUTES_LOCK_TIME = 1;

        /// <summary>
        /// Minimum password length required.
        /// </summary>
        private const int PASSWORD_MIN_LENGTH = 6;

        #endregion Constants

        #region Properties

        /// <summary>
        /// Option that determines the displayed combination UI.
        /// </summary>
        private CombinationOptions Option { get; set; }

        /// <summary>
        /// LockCombination for the Safe Application.
        /// </summary>
        private LockCombination LockCombination { get; set; }

        /// <summary>
        /// True when Changing an existing combination. Otherwise false.
        /// </summary>
        private bool IsChangingCombination { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a new SafeLockPage instance.
        /// </summary>
        public SafeLockPage()
        {
            InitializeComponent();

            IsChangingCombination = false;

            GetCombination();
        }

        /// <summary>
        /// Constructs a new SafeLockPage instance.
        /// </summary>
        /// <param name="changeCombination">True when changing an existing lock combination.</param>
        public SafeLockPage(bool changeCombination)
        {
            InitializeComponent();

            IsChangingCombination = changeCombination;

            GetCombination();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Handler for keeping the Focus on the Four Number Combination.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeFocus(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
                return;

            List<Entry> numbers = new List<Entry>() {
                number1,
                number2,
                number3,
                number4,
            };

            for (int i = 0; i < numbers.Count; i++)
            {
                // Check if the number is valid
                if ((sender as Entry).Equals(numbers[i]) && i + 1 < numbers.Count)
                {
                    numbers[++i].Focus();
                    return;
                }
            }
        }

        /// <summary>
        /// Handler for when the Combination Option Switch is Toggled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void optionSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            switch (Option)
            {
                case CombinationOptions.FourNumberCombination:
                    
                    // Switch to Password Combination
                    fourNumberCombinationGrid.IsVisible = false;
                    passwordGrid.IsVisible = true;
                    Option = CombinationOptions.Password;
                    break;

                case CombinationOptions.Password:

                    // Switch to Four Number Combination 
                    passwordGrid.IsVisible = false;
                    fourNumberCombinationGrid.IsVisible = true;
                    Option = CombinationOptions.FourNumberCombination;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Handler for when the Enter Combination or Save Combination button is Clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Clicked(object sender, EventArgs e)
        {
            switch (Option)
            {
                case CombinationOptions.FourNumberCombination:
                    ValidateFourNumberCombination();
                    break;

                case CombinationOptions.Password:
                    ValidatePassword(); 
                    break;

                default:
                    break;
            }
        }

        #endregion Events

        #region Validation Methods

        /// <summary>
        /// Validate that the Four Number Combination follows the following format: XX-XX-XX-XX where X is a number between 0 and 9.
        /// </summary>
        private void ValidateFourNumberCombination()
        {
            // Get all numbers
            List<Entry> numbers = new List<Entry>() {
                number1,
                number2,
                number3,
                number4,
            };

            // Assume the combination is valid
            bool isValid = true;

            foreach(Entry number in numbers)
            {
                // Check if the number is valid
                if(string.IsNullOrWhiteSpace(number.Text) || number.Text.Contains('.') || number.Text.Contains(',') || number.Text.Contains('-'))
                {
                    // If its not valid, set the focus on it
                    isValid = false;
                    number.Text = "";
                    number.Focus();
                    break;
                }
            }

            // Show Alert if needed
            if (!isValid)
                DisplayAlert("Invalid Combination", "Enter all four Combination numbers. They must be valid numbers. ", "OK");
            else
                TryCombination(string.Format("{0}-{1}-{2}-{3}", number1.Text, number2.Text, number3.Text, number4.Text));
        }

        /// <summary>
        /// Validates that the Combination is a valid Password before trying it. If not valid, an Alert is shown.
        /// </summary>
        private void ValidatePassword()
        {
            // Validate the password
            bool isValid = (!string.IsNullOrWhiteSpace(password.Text) 
                && password.Text.Length>=PASSWORD_MIN_LENGTH 
                && Regex.Match(password.Text,@"[a-z]").Success
                && Regex.Match(password.Text, @"[A-Z]").Success
                && Regex.Match(password.Text, @"[0-9]").Success);

            // password is not valid
            if (!isValid)
                DisplayAlert("Not Secure",string.Format("The Password MUST contain at least {0} characters.\nIt must have at least 1 lowercase, 1 uppercase and 1 digit.",PASSWORD_MIN_LENGTH),"OK");
            // password is valid
            else
                TryCombination(password.Text);
        }

        #endregion Validation Methods

        #region Operation Methods

        /// <summary>
        /// Gets the Combination and sets the appropriate UI.
        /// </summary>
        private async void GetCombination()
        {
            // Get the LockCombination 
            LockCombination = IsChangingCombination ? null : await Service.GetLockCombination();

            // Set the appropriate Combination Option
            Option = LockCombination == null ? CombinationOptions.NotSet : (CombinationOptions)LockCombination.Option;

            // Create the UI for the Option.
            switch (Option)
            {
                case CombinationOptions.FourNumberCombination:
                    SetFourNumberCombinationUI();
                    break;

                case CombinationOptions.Password:
                    SetPasswordUI();
                    break;

                case CombinationOptions.NotSet:
                default:
                    SetNewCombinationUI();
                    break;
            }
        }

        /// <summary>
        /// Sets the UI for entering a Four Number combination.
        /// </summary>
        private void SetFourNumberCombinationUI()
        {
            fourNumberCombinationGrid.IsVisible = true;
            passwordGrid.IsVisible = false;
            optionSwitch.IsVisible = false;
        }

        /// <summary>
        /// Sets the UI for entering a Password combination.
        /// </summary>
        private void SetPasswordUI()
        {
            passwordGrid.IsVisible = true;
            fourNumberCombinationGrid.IsVisible = false;
            optionSwitch.IsVisible = false;
        }

        /// <summary>
        /// Sets the UI for creating a new Combination.
        /// </summary>
        private void SetNewCombinationUI()
        {
            SetFourNumberCombinationUI();
            optionSwitch.IsVisible = true;
            optionLabel.IsVisible = true;
            pageHeader.Text = "Create a Safe Combination";
            saveButton.Text = "Save Combination";
            Option = CombinationOptions.FourNumberCombination;
        }

        /// <summary>
        /// Tries a combination to open the safe. After a certain amount of tries the safe gets locked for a certain amount of time.
        /// </summary>
        /// <param name="combination">Combination entered by the User.</param>
        private void TryCombination(string combination)
        {
            // Setting a new Combination or changing an existing one
            if (LockCombination == null)
            {
                // Create the new Lock Combination
                LockCombination = new LockCombination()
                {
                    Id=1,
                    Combination = combination,
                    Option = (int)Option,
                    IsLocked = false,
                    LockedUntil = DateTime.MinValue,
                    AmountOfTries = MAX_NUMBER_OF_TRIES,
                    MinsLocked= MIN_MINUTES_LOCK_TIME
                };

                // Save the combination and enter the safe
                GoodCombinationHandler();
            }
            // A Combination already exists
            else
            {
                // safe is currently locked
                if (LockCombination.IsLocked)
                {
                    // Safe is not locked anymore
                    if (DateTime.Now.CompareTo(LockCombination.LockedUntil)>0)
                    {
                        LockCombination.IsLocked = false;
                        // Try opening the safe
                        CombinationHandler(combination);
                    }
                    // Safe is still locked
                    else
                        ShowWrongAttemptResult();
                }
                // Try opening the safe
                else
                    CombinationHandler(combination);
            }
        }

        /// <summary>
        /// Determines if a combination is correct or wrong.
        /// </summary>
        /// <param name="combination">Combination entered by the User.</param>
        private void CombinationHandler(string combination)
        {
            // Good Combination
            if (LockCombination.Combination.Equals(combination))
                GoodCombinationHandler();
            // Wrong Combination
            else
                WrongCombinationHandler();
        }

        /// <summary>
        /// Opens the Safe. Allows the user to see the saved credentials.
        /// </summary>
        private void GoodCombinationHandler()
        {
            // Reset Amount of tries and Lock State
            LockCombination.AmountOfTries = MAX_NUMBER_OF_TRIES;
            LockCombination.IsLocked = false;
            LockCombination.LockedUntil = DateTime.MinValue;
            LockCombination.MinsLocked = MIN_MINUTES_LOCK_TIME;

            // Save the combination
            Service.SaveLockCombination(LockCombination);

            // Go to the Credentials Page
            if (IsChangingCombination)
                Navigation.RemovePage(this);
            else
            {
                Navigation.InsertPageBefore(new CredentialsPage(), this);
                Navigation.RemovePage(this);
            }
        }

        /// <summary>
        /// Handler for when a wrong Safe Combination is entered.
        /// Decreases the amount of available tries and locks the safe if necessary.
        /// </summary>
        private void WrongCombinationHandler()
        {
            if (LockCombination.AmountOfTries == 0)
                LockSafe();
            else
            {
                LockCombination.AmountOfTries--;
                Service.SaveLockCombination(LockCombination);
            }

            ShowWrongAttemptResult();
        }

        /// <summary>
        /// Shows feedback when the user tries a combination that does not open the safe.
        /// </summary>
        private void ShowWrongAttemptResult()
        {
            if (LockCombination.IsLocked)
                DisplayAlert("Safe is Locked", string.Format("Try again in {0} seconds.", (LockCombination.LockedUntil.Subtract(DateTime.Now)).TotalSeconds.ToString("n0")), "OK");
            else
                DisplayAlert("Wrong Combination", string.Format("You have {0} tries remaining.", LockCombination.AmountOfTries), "OK");
        }

        /// <summary>
        /// Locks the safe for a certain amount of time.
        /// </summary>
        private void LockSafe()
        {
            LockCombination.IsLocked = true;
            LockCombination.LockedUntil = DateTime.Now;
            LockCombination.LockedUntil = LockCombination.LockedUntil.AddMinutes(LockCombination.MinsLocked);
            LockCombination.MinsLocked *= LOCK_TIME_INCREASE_FACTOR;
            Service.SaveLockCombination(LockCombination);
        }

        #endregion Operation Methods

        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        
    }
}