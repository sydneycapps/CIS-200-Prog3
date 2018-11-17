// Program 3
// CIS 200-01
// Fall 2018
// Due: 11/12/2018
// By: D5236

// File: Prog2Form.cs
// This class creates the main GUI for Program 3. It provides a
// File menu with About, Exit, Open, and SaveAs items, an Insert menu with Address and
// Letter items, a Report menu with List Addresses and List Parcels
// items, and an Edit menu with an Address item.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace UPVApp
{
    public partial class Prog2Form : Form
    {
        private UserParcelView upv; // The UserParcelView
        private BinaryFormatter formatter = new BinaryFormatter(); // Object for serializing UserParcelView data in binary format
        private FileStream output; // Stream to writing to a file
        private FileStream input; // Stream for reading from a file

        // Precondition:  None
        // Postcondition: The form's GUI is prepared for display. A few test addresses are
        //                added to the list of addresses
        public Prog2Form()
        {
            InitializeComponent();

            upv = new UserParcelView();
        }

        // Precondition:  File, About menu item activated
        // Postcondition: Information about author displayed in dialog box
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string NL = Environment.NewLine; // Newline shorthand

            MessageBox.Show($"Program 3{NL}By: D5236{NL}CIS 200{NL}Fall 2018",
                "This program explores file I/O and object serialization and expands the GUI application developed in Program 2.");
        }

        // Precondition:  File, Exit menu item activated
        // Postcondition: The application is exited
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Precondition:  Insert, Address menu item activated
        // Postcondition: The Address dialog box is displayed. If data entered
        //                are OK, an Address is created and added to the list
        //                of addresses
        private void addressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddressForm addressForm = new AddressForm();    // The address dialog box form
            DialogResult result = addressForm.ShowDialog(); // Show form as dialog and store result
            int zip; // Address zip code

            if (result == DialogResult.OK) // Only add if OK
            {
                if (int.TryParse(addressForm.ZipText, out zip))
                {
                    upv.AddAddress(addressForm.AddressName, addressForm.Address1,
                        addressForm.Address2, addressForm.City, addressForm.State,
                        zip); // Use form's properties to create address
                }
                else // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Address Validation!", "Validation Error");
                }
            }

            addressForm.Dispose(); // Best practice for dialog boxes
                                   // Alternatively, use with using clause as in Ch. 17
        }

        // Precondition:  Report, List Addresses menu item activated
        // Postcondition: The list of addresses is displayed in the addressResultsTxt
        //                text box
        private void listAddressesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String
            string NL = Environment.NewLine;            // Newline shorthand

            result.Append("Addresses:");
            result.Append(NL); // Remember, \n doesn't always work in GUIs
            result.Append(NL);

            foreach (Address a in upv.AddressList)
            {
                result.Append(a.ToString());
                result.Append(NL);
                result.Append("------------------------------");
                result.Append(NL);
            }

            reportTxt.Text = result.ToString();

            // -- OR --
            // Not using StringBuilder, just use TextBox directly

            //reportTxt.Clear();
            //reportTxt.AppendText("Addresses:");
            //reportTxt.AppendText(NL); // Remember, \n doesn't always work in GUIs
            //reportTxt.AppendText(NL);

            //foreach (Address a in upv.AddressList)
            //{
            //    reportTxt.AppendText(a.ToString());
            //    reportTxt.AppendText(NL);
            //    reportTxt.AppendText("------------------------------");
            //    reportTxt.AppendText(NL);
            //}

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        // Precondition:  Insert, Letter menu item activated
        // Postcondition: The Letter dialog box is displayed. If data entered
        //                are OK, a Letter is created and added to the list
        //                of parcels
        private void letterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LetterForm letterForm; // The letter dialog box form
            DialogResult result;   // The result of showing form as dialog
            decimal fixedCost;     // The letter's cost

            if (upv.AddressCount < LetterForm.MIN_ADDRESSES) // Make sure we have enough addresses
            {
                MessageBox.Show("Need " + LetterForm.MIN_ADDRESSES + " addresses to create letter!",
                    "Addresses Error");
                return; // Exit now since can't create valid letter
            }

            letterForm = new LetterForm(upv.AddressList); // Send list of addresses
            result = letterForm.ShowDialog();

            if (result == DialogResult.OK) // Only add if OK
            {
                if (decimal.TryParse(letterForm.FixedCostText, out fixedCost))
                {
                    // For this to work, LetterForm's combo boxes need to be in same
                    // order as upv's AddressList
                    upv.AddLetter(upv.AddressAt(letterForm.OriginAddressIndex),
                        upv.AddressAt(letterForm.DestinationAddressIndex),
                        fixedCost); // Letter to be inserted
                }
               else // This should never happen if form validation works!
                {
                    MessageBox.Show("Problem with Letter Validation!", "Validation Error");
                }
            }

            letterForm.Dispose(); // Best practice for dialog boxes
                                  // Alternatively, use with using clause as in Ch. 17
        }

        // Precondition:  Report, List Parcels menu item activated
        // Postcondition: The list of parcels is displayed in the parcelResultsTxt
        //                text box
        private void listParcelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder result = new StringBuilder(); // Holds text as report being built
                                                        // StringBuilder more efficient than String
            decimal totalCost = 0;                      // Running total of parcel shipping costs
            string NL = Environment.NewLine;            // Newline shorthand

            result.Append("Parcels:");
            result.Append(NL); // Remember, \n doesn't always work in GUIs
            result.Append(NL);

            foreach (Parcel p in upv.ParcelList)
            {
                result.Append(p.ToString());
                result.Append(NL);
                result.Append("------------------------------");
                result.Append(NL);
                totalCost += p.CalcCost();
            }

            result.Append(NL);
            result.Append($"Total Cost: {totalCost:C}");

            reportTxt.Text = result.ToString();

            // Put cursor at start of report
            reportTxt.Focus();
            reportTxt.SelectionStart = 0;
            reportTxt.SelectionLength = 0;
        }

        // Precondition: File, SaveAs menu item is activated
        // Postcondition: Creates a sequential-access file using serialization
        private void saveAsButton_Click(object sender, EventArgs e)
        {
            DialogResult result; // Store result
            string fileName; // Name of file to save data

            using (SaveFileDialog fileSave = new SaveFileDialog())
            {
                fileSave.CheckFileExists = false; // Let user create file
                result = fileSave.ShowDialog(); // Retrieve the result of the dialog box
                fileName = fileSave.FileName; // Get specific file name
            }

            if(result == DialogResult.OK) // Ensure user clicked "OK"
            {
                if(string.IsNullOrEmpty(fileName)) // Is file valid?
                {
                    MessageBox.Show("Invalid File Name", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Save file via FileStream is user specified valid file
                    try
                    {
                        output = new FileStream(fileName,
                            FileMode.Create, FileAccess.Write); // Open file with write access

                        formatter.Serialize(output, upv); // Write upv object to FileStream (serialize object)
                        output.Close(); // Close FileStream
                    }
                    // Can file be opened?
                    catch(IOException)
                    {
                        MessageBox.Show("Error opening file", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Precondition: File, Open menu item is activated
        // Postcondition: Opens a sequential-access file using deserialization
        private void openButton_Click(object sender, EventArgs e)
        {
            DialogResult result; // Store result
            string fileName; // Name of file containing data

            using (OpenFileDialog fileOpen = new OpenFileDialog())
            {
                result = fileOpen.ShowDialog();
                fileName = fileOpen.FileName; // Get specified name
            }

            if(result == DialogResult.OK) // Ensure user clicked "OK"
            {
                if (string.IsNullOrEmpty(fileName)) // Is file valid?
                {
                    MessageBox.Show("Invalid File Name", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        input = new FileStream(fileName, 
                            FileMode.Open, FileAccess.Read); // Create FileStream to obtain read access to file

                        upv = (UserParcelView)formatter.Deserialize(input); // Get next upv object available in file
                    }
                    // Can file be opened?
                    catch(IOException)
                    {
                        MessageBox.Show("Error opening file", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    // Is file the right file type?
                    catch(SerializationException)
                    {
                        MessageBox.Show("Wrong file type", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Precondition: Edit, Address menu item activated
        // Postcondition: EditAddressForm shows
        private void editAddressButton_Click(object sender, EventArgs e)
        {
            EditAddressForm editAddressForm; // The edit address dialog box form
            DialogResult result1; // Result of showing form as dialog

            editAddressForm = new EditAddressForm(upv.AddressList); // Send list of addresses
            result1 = editAddressForm.ShowDialog();

            if(result1 == DialogResult.OK) // Add if OK
            {
                int addIndex = editAddressForm.AddressIndex; // Address index
                Address add = upv.AddressAt(addIndex); // Add address at index
                AddressForm addressForm = new AddressForm(); // The address dialog box form

                //Sets AddressFrom properties to Address properties
                addressForm.AddressName = add.Name;
                addressForm.Address1 = add.Address1;
                addressForm.Address2 = add.Address2;
                addressForm.City = add.City;
                addressForm.State = add.State;
                addressForm.ZipText = add.Zip.ToString();

                DialogResult result2 = addressForm.ShowDialog(); // Result of showing form as dialog
                if (result2 == DialogResult.OK) // Add if OK
                {
                    // Sets Address properties to AddressForm properties
                    add.Name = addressForm.AddressName;
                    add.Address1 = addressForm.Address1;
                    add.Address2 = addressForm.Address2;
                    add.City = addressForm.City;
                    add.State = addressForm.State;
                    add.Zip = int.Parse(addressForm.ZipText);
                }
            }
        }
    }
}