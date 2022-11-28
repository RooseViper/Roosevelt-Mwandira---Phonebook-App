using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContactCell : MonoBehaviour
{
    public Text contactNameText, phoneNumberText;
    public Button editButton;
    public Button deleteButton;
    private int contactId;
    ContactManager contactManager;
    Contact contact;

    public int ContactId { get => contactId; set => contactId = value; }

    private void Start()
    {
        contactManager = FindObjectOfType<ContactManager>();
        contact = contactManager.GetComponent<Contact>();
        editButton.onClick.AddListener(EditContactEvent);
        deleteButton.onClick.AddListener(Delete);
    }

    private void EditContactEvent()
    {
        contact.Retrieve(contactId);
        contact.CurrentContactId = contactId;
        contactManager.saveUpdateButton.onClick.AddListener(contact.Edit);
        contactManager.ActivatePage(Page.PagePanel.SAVE_UPDATE);
    }

    private void Delete()
    {
        contact.Delete(contactId);
    }
}
