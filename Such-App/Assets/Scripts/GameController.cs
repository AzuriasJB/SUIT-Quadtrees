using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class GameController : MonoBehaviour
{
    public Quadtree quadtree;

    private static GameController instance = null;
    private string filePath = "setup.txt";

    // private constructor for Singleton
    private GameController() { }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        setupGame();
    }

    /// <summary>
    /// Loads the pieces from file, creates them as QuadtreeElements and creates the Quadtree structure
    /// </summary>
    private void setupGame()
    {
        // load setup from file
        string data;
        List<QuadtreeElement> elems = new List<QuadtreeElement>();

        try
        {
            // read setup from file
            StreamReader reader = new StreamReader(Application.persistentDataPath + "/" + filePath);
            data = reader.ReadLine();
            string[] pieces = data.Split(',');

            // create QuadtreeElements
            for (int i = 0; i < pieces.Length; i++)
            {
                elems.Add(new QuadtreeElement(
                    int.Parse(pieces[i][1].ToString()),
                    int.Parse(pieces[i][2].ToString()),
                    (Piece)int.Parse(pieces[i][0].ToString())
                    ));
            }
        }
        catch (FileNotFoundException)
        {
            // Default pawn in top left corner
            elems.Add(new QuadtreeElement(0, 0, Piece.Pawn));
        }

        // create quadtree structure
        quadtree = Quadtree.buildTree(elems);

        // Debug
        //quadtree.print();
    }

    /// <summary>
    /// Hands the instance of the GameController
    /// </summary>
    /// <returns>GameController instance</returns>
    public static GameController getInstance()
    {
        if (instance == null)
            instance = new GameController();
        return instance;
    }
}
