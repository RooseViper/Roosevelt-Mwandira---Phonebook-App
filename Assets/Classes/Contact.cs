using Mono.Data.SqliteClient;
using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Contact : MonoBehaviour
{

    public InputField firstNameField, middleNameField, lastNameField, emailField, phoneNumberField, searchContactInputField;
    public Text phoneNumberValidator;
    private Coroutine phoneNumberCRT;
    private int currentContactId;
    public int ContactId { get; set; }
    public string Firstname { get; set; }
    public string Middlename { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public int CurrentContactId { get => currentContactId; set => currentContactId = value; }   //This Contact Id is used for editing a specific contact

    private ContactManager contactManager;
    public int GetContactId()
    {
        return ContactId;
    }
    public string GetFirstName()
    {
        return string.Format("{0}", Firstname);
    }

    public string GetMiddleName()
    {
        return string.Format("{0}", Middlename);
    }

    public string GetLastName()
    {
        return string.Format("{0}", Lastname);
    }

    public string GetEmail()
    {
        return string.Format("{0}", Email);
    }

    public string GetPhoneNumber()
    {
        return string.Format("{0}", PhoneNumber);
    }

    private void Start()
    {
        contactManager = GetComponent<ContactManager>();
    }



    public void Add()
    {
        if (GetPhoneNumber(phoneNumberField.text).Equals(phoneNumberField.text))
        {
            PhoneNumberValidator();
        }
        else
        {
            string query = @"INSERT INTO Contact(Firstname, Middlename, Lastname, Email, PhoneNumber) 
                                          VALUES('" + firstNameField.text +
                                               "', '" + middleNameField.text +
                                               "', '" + lastNameField.text +
                                               "', '" + emailField.text +
                                               "', '" + phoneNumberField.text +
                                               "')";
            SQLiteCommand dbCommand = new SQLiteCommand(ContactManager.sqliteConnection);
            dbCommand.CommandText = query;
            int i = dbCommand.ExecuteNonQuery();
            if (i == 1)
            {
                contactManager.ListContacts();
                StartCoroutine(contactManager.Refresh());
                contactManager.ActivatePage(Page.PagePanel.CONTACTS);
            }
        }
    }

    private void PhoneNumberValidator() {
        phoneNumberValidator.enabled = true;
        if (phoneNumberCRT != null)
        {
            StopCoroutine(phoneNumberCRT);
        }
        phoneNumberCRT = null;
        phoneNumberCRT = StartCoroutine(PopupError(phoneNumberValidator));
    }

    public void Edit()
    {
        SQLiteCommand cmnd = new SQLiteCommand(ContactManager.sqliteConnection);
        cmnd.CommandText = @"UPDATE Contact SET Firstname = '" + firstNameField.text +
                                         "', Middlename = '" + middleNameField.text +
                                         "', Lastname = '" + lastNameField.text +
                                         "', Email = '" + emailField.text +
                                         "', PhoneNumber = '" + phoneNumberField.text +
                                         "' WHERE ContactId = '" + currentContactId + "' ";
        int i = cmnd.ExecuteNonQuery();
        if (i == 1)
        {
            contactManager.ListContacts();
            StartCoroutine(contactManager.Refresh());
            contactManager.ActivatePage(Page.PagePanel.CONTACTS);
        }
        Debug.Log("Clicked " + currentContactId);
    }

    public void Delete(int contact_Id)
    {
        SQLiteCommand cmnd = new SQLiteCommand(ContactManager.sqliteConnection);
        cmnd.CommandText = @"DELETE FROM Contact WHERE ContactId = '" + contact_Id + "' ";
        int i = cmnd.ExecuteNonQuery();
        if (i == 1)
        {
            contactManager.ListContacts();
            StartCoroutine(contactManager.Refresh());
            contactManager.ActivatePage(Page.PagePanel.CONTACTS);
        }
    }

    public void Retrieve(int contact_Id)
    {
        var contacts = GetContact(contact_Id);

        foreach (var contact in contacts)
        {
            firstNameField.text = contact.GetFirstName();
            middleNameField.text = contact.GetMiddleName();
            lastNameField.text = contact.GetLastName();
            emailField.text = contact.GetEmail();
            phoneNumberField.text = contact.GetPhoneNumber();
        }
    }

  


    public void LoadContactCell() {
        var contacts = GetAllContacts();
        foreach (var contact in contacts)
        {
            GameObject contactCellObj = Instantiate(contactManager.contactCellPrefab, contactManager.contactCellsParent);
            ContactCell contactCell = contactCellObj.GetComponent<ContactCell>();
            contactCell.contactNameText.text = contact.GetFirstName() + " " + contact.GetLastName();
            contactCell.phoneNumberText.text = contact.GetPhoneNumber();
            contactCell.ContactId = contact.GetContactId();
        }
    }

    private IEnumerable<Contact> GetAllContacts() {
        return ContactManager.sqliteConnection.Table<Contact>();
    }

    private IEnumerable<Contact> GetContact(int contactId)
    {
        return ContactManager.sqliteConnection.Table<Contact>().Where(x => x.ContactId == contactId);
    }
    private string GetPhoneNumber(string phoneNumber)
    {
        string value = "";
        var tasks = GetPhoneNumbers(phoneNumber);
        foreach (var task in tasks)
        {
            value = task.GetPhoneNumber();
        }
        return value;
    }

    private IEnumerable<Contact> GetPhoneNumbers(string phoneNumber)
    {
        return ContactManager.sqliteConnection.Table<Contact>().Where(x => x.PhoneNumber == phoneNumber);
    }
    public void ClearForm() {

        firstNameField.text = "";
        middleNameField.text = "";
        lastNameField.text = "";
        emailField.text = "";
        phoneNumberField.text = "";
    }

    IEnumerator PopupError(Text text)
    {
        yield return new WaitForSeconds(2.5f);
        text.enabled = false;
    }
    private string finalPartPhoneNumber = "";
}
