using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    public enum PagePanel
    {
        CONTACTS,
        SAVE_UPDATE
    }
    public PagePanel currentPage;
}
