// Program 3
// CIS 200-01
// Fall 2018
// Due: 11/12/2018
// By: D5236

// File: EditAddressForm.cs
// This class creates the Edit Address dialog box form GUI. It populates a combobox of 
// a list of addresses to edit. After choosing an address, the Address form displays.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPVApp
{
    public partial class EditAddressForm : Form
    {
        private List<Address> addressList; // List of addresses to fill combo boxes

        // Preconditions: None
        // Postconditions: The form's GUI is ready for display
        public EditAddressForm(List<Address> addresses)
        {
            InitializeComponent();

            addressList = addresses;
        }

        internal int AddressIndex
        {
            // Precondition:  User has selected from addressListCbo
            // Postcondition: The index of the selected address returned
            get
            {
                return addressListCbo.SelectedIndex;
            }

            // Precondition: value >= -1
            // Postcondition: The specified index is selected in addressListCbo.
            set
            {
                if (value >= -1)
                    addressListCbo.SelectedIndex = value;
                else
                    throw new ArgumentOutOfRangeException("AddressIndex", value,
                        "Index must be valid");
            }
        }

        // Precondition: None
        // Postcondition: The list of addresses is used to populate the address list combobox.
        private void EditAddressForm_Load(object sender, EventArgs e)
        {
            foreach (Address a in addressList)
                addressListCbo.Items.Add(a.Name);
        }

        // Precondition:  Focus shifting from the address combo box
        //                sender is ComboBox
        // Postcondition: If no address selected, focus remains and error provider
        //                show icon next to the field.
        private void addressListCbo_Validating(object sender, CancelEventArgs e)
        {
            ComboBox cbo = sender as ComboBox; // Cast sender as ComboBox

            if (cbo.SelectedIndex == -1)
            {
                e.Cancel = true;
                errorProvider.SetError(cbo, "Must select an address!");
            }
        }

        // Precondition:  Validating of sender not cancelled, so data OK
        //                sender is Control
        // Postcondition: Error provider cleared and focus allowed to change
        private void addressListCbo_Validated(object sender, EventArgs e)
        {
            Control control = sender as Control; // Cast sender as Control

            errorProvider.SetError(control, "");
        }

        // Precondition: User pressed the cancelButton
        // Postcondition: Form closes
        private void cancelButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                this.DialogResult = DialogResult.Cancel;
        }

        // Precondition: User pressed the okButton
        // Postcondition: If invalid field on dialog, keep form open and give first invalid
        //                field the focus. Else return OK and close form.
        private void okButton_Click(object sender, EventArgs e)
        {
            if (ValidateChildren())
                this.DialogResult = DialogResult.OK;
        }
    }
}
