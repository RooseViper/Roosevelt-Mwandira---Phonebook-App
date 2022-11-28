using SQLite4Unity3d;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ContactManager : MonoBehaviour
{
    public GameObject contactCellPrefab, dummyContactCellPrefab;
    public Transform contactCellsParent;
    public Button saveUpdateButton;
    public Page[] pages;
    private SQLiteConnection _connection;
    public static SQLiteConnection sqliteConnection;
    private Contact contact;
    public Page.PagePanel currentPage;
    // Start is called before the first frame update
    public void DataService(string DatabaseName)
    {
#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
        _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        sqliteConnection = _connection;
        Debug.Log("Final PATH: " + dbPath);
        //    testText.text = dbPath;
    }

    private void Start()
    {
        DataService("PhonebookApp_Db.db");
        contact = GetComponent<Contact>();
        StartCoroutine(AwaitContacts());
    }

    public void ListContacts()
    {
        ClearContacts();
        contact.LoadContactCell();
        AlignCells(contactCellsParent, dummyContactCellPrefab);
    }
    private void ClearContacts()
    {
        ContactCell[] contactCells = contactCellsParent.GetComponentsInChildren<ContactCell>();
        if (contactCells is ContactCell[] cells)
        {
            if (contactCells.Length > 0)
            {
                foreach (ContactCell cell in cells)
                {
                    Destroy(cell.gameObject);
                }
            }
        }
    }
    public void OpenContactsPanel() {
        contact.ClearForm();
        saveUpdateButton.onClick.AddListener(contact.Add);
        ActivatePage(Page.PagePanel.SAVE_UPDATE);
    }

    public void Back() {
        ActivatePage(Page.PagePanel.CONTACTS);
    }

    private IEnumerator AwaitContacts() {
        yield return new WaitForEndOfFrame();
        contact.LoadContactCell();
        AlignCells(contactCellsParent, dummyContactCellPrefab);
    }
    //Align Cells when saving or adding new contacts
    public IEnumerator Refresh() {
        yield return new WaitForEndOfFrame();
        AlignCells(contactCellsParent, dummyContactCellPrefab);
    }

    /// <summary>
    /// Aligns the cells to be aligned in a presentable way for the users
    /// </summary>
    private void AlignCells(Transform itemParent, GameObject dummyCellPrefab)
    {
        int dummyCells = 7 - itemParent.childCount;
        if (dummyCells < 7)
        {
            for (int i = 0; i < dummyCells; i++)
            {
                Instantiate(dummyCellPrefab, itemParent.transform);
            }
        }
    }

    public void ActivatePage(Page.PagePanel pagePanel)
    {
        foreach (Page item in pages)
        {
            item.gameObject.SetActive(false);
        }
        Page page = System.Array.Find(pages, pg => pg.currentPage == pagePanel);
        page.gameObject.SetActive(true);
        currentPage = pagePanel;
    }


}
