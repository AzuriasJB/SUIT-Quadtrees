using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the UI interaction of the settings page and saving/loading the settings to a file
/// </summary>
public class SettingsScript : MonoBehaviour
{
    /// <summary>
    /// A struct that defines one group of 3 input dropdown menus
    /// </summary>
    [System.Serializable]
    public struct dropCollection
    {
        public Dropdown figDrop;
        public Dropdown xDrop;
        public Dropdown yDrop;

        public dropCollection (Dropdown fD, Dropdown xD, Dropdown yD)
        {
            this.figDrop = fD;
            this.xDrop = xD;
            this.yDrop = yD;
        }
    }
    
    // init in inspector
    // Note: technically possible to do it, by adding and removing templates like the dropdown
    // during runtime and allocate dynamically
    public List<dropCollection> dropCollections = new List<dropCollection>(6);

    /// <summary>
    /// List of all the Objects/Groups of dropdowns for easy access
    /// </summary>
    public GameObject[] dropList = new GameObject[6];
    private string filePath = "setup.txt";

    private int pieceCount = 1;

    private void Start()
    {
        loadPieces();
    }

    /// <summary>
    /// Loads the pieces and their locations from the setup file 
    /// and sets the menus accordingly, if possible
    /// </summary>
    public void loadPieces()
    {
        // load setting from file
        StreamReader reader = new StreamReader(Application.persistentDataPath + "/" + filePath);
        string data = reader.ReadLine();
        string[] pieces = data.Split(',');

        pieceCount = pieces.Length;

        // set settings according to setup file
        for (int i = 0; i < pieceCount; i++)
        {
            dropCollections[i].figDrop.value = int.Parse(pieces[i][0].ToString());
            dropCollections[i].xDrop.value = int.Parse(pieces[i][1].ToString());
            dropCollections[i].yDrop.value = int.Parse(pieces[i][2].ToString());
            dropList[i].SetActive(true);
        }
    }

    /// <summary>
    /// Saves the pieces and their locations to the setup file
    /// </summary>
    public void savePieces()
    {
        string data = "";

        // go through every active piece and add the data to the data string
        for (int i = 0; i < pieceCount; i++)
        {
            // add piece abreviation to string
            data += dropCollections[i].figDrop.value.ToString();

            // add x/y data to string
            data += (dropCollections[i].xDrop.value).ToString();
            data += (dropCollections[i].yDrop.value).ToString();

            // add seperator
            data += ",";
        }

        // remove last seperator
        data = data.Remove(data.Length - 1);
        Debug.Log(data);

        // write data string to file
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + "/" + filePath, false);
        writer.Write(data);
        writer.Close();
    }

    public void increasePieceCount()
    {
        if (pieceCount == 6) return;

        // show one more dropdown set
        dropList[pieceCount++].SetActive(true);
    }

    public void decreasePieceCount()
    {
        if (pieceCount == 1) return;

        // show one less dropdown set
        dropList[--pieceCount].SetActive(false);
    }

    public void exitToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
