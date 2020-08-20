using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles input and UI in the search game
/// </summary>
public class DPadController : MonoBehaviour
{
    public Text textFeld;
    public Image green;
    public Image red;
    public Image rook;
    public Image pawn;
    public Image bishop;
    public Image knight;
    public Image queen;
    public Image king;

    private RectTransform rtGreen;
    private RectTransform rtRed;
    private int depth = 0;
    private List<Direction> directions = new List<Direction>();

    // Start is called before the first frame update
    void Start()
    {
        // hide the red border and all the pieces
        red.enabled = false;
        disableAllPieces();

        // get the transforms of the red and green border for later use
        rtGreen = green.GetComponent<RectTransform>();
        rtRed = red.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Handles the input of the d-pad. Steps down in the quadtree
    /// </summary>
    /// <param name="inputDirection">pressed direction</param>
    public void registerDirection(int inputDirection)
    {
        // max depth
        if (depth >= 3) return;

        // shrink both borders
        rtGreen.sizeDelta = new Vector2(rtGreen.rect.width / 2, rtGreen.rect.height / 2);
        rtRed.sizeDelta = new Vector2(rtRed.rect.width / 2, rtRed.rect.height / 2);

        // add input direction to list of taken directions for back tracking
        Direction dir = (Direction)inputDirection;
        directions.Add(dir);

        // step down in Quadtree and move both borders accordingly
        switch (dir)
        {
            case Direction.NorthWest:
                textFeld.text = "Pressed NorthWest"; // Debug
                rtGreen.position = new Vector3(rtGreen.position.x - (rtGreen.rect.width / 2), rtGreen.position.y + (rtGreen.rect.height / 2));
                rtRed.position = new Vector3(rtRed.position.x - (rtRed.rect.width / 2), rtRed.position.y + (rtRed.rect.height / 2));
                break;
            case Direction.NorthEast:
                textFeld.text = "Pressed NorthEast"; // Debug
                rtGreen.position = new Vector3(rtGreen.position.x + (rtGreen.rect.width / 2), rtGreen.position.y + (rtGreen.rect.height / 2));
                rtRed.position = new Vector3(rtRed.position.x + (rtRed.rect.width / 2), rtRed.position.y + (rtRed.rect.height / 2));
                break;
            case Direction.SouthEast:
                textFeld.text = "Pressed SouthEast"; // Debug
                rtGreen.position = new Vector3(rtGreen.position.x + (rtGreen.rect.width / 2), rtGreen.position.y - (rtGreen.rect.height / 2));
                rtRed.position = new Vector3(rtRed.position.x + (rtRed.rect.width / 2), rtRed.position.y - (rtRed.rect.height / 2));
                break;
            case Direction.SouthWest:
                textFeld.text = "Pressed SouthWest"; // Debug
                rtGreen.position = new Vector3(rtGreen.position.x - (rtGreen.rect.width / 2), rtGreen.position.y - (rtGreen.rect.height / 2));
                rtRed.position = new Vector3(rtRed.position.x - (rtRed.rect.width / 2), rtRed.position.y - (rtRed.rect.height / 2));
                break;
        }
        depth++;

        checkBorderColor();
        return;
    }

    /// <summary>
    /// Handles the activation of the back button. Steps up in the quadtree
    /// </summary>
    public void backButtonPressed()
    {
        // minimum depth
        if (depth == 0) return;
        textFeld.text = "Pressed Back"; // Debug

        // get last directional input and remove it from the list
        Direction dir = directions[directions.Count - 1];
        directions.RemoveAt(directions.Count - 1);

        // move both borders back according to last input
        switch (dir)
        {
            case Direction.NorthWest:
                rtGreen.position = new Vector3(rtGreen.position.x + (rtGreen.rect.width/2), rtGreen.position.y - (rtGreen.rect.height/2));
                rtRed.position = new Vector3(rtRed.position.x + (rtRed.rect.width / 2), rtRed.position.y - (rtRed.rect.height / 2));
                break;
            case Direction.NorthEast:
                rtGreen.position = new Vector3(rtGreen.position.x - (rtGreen.rect.width / 2), rtGreen.position.y - (rtGreen.rect.height / 2));
                rtRed.position = new Vector3(rtRed.position.x - (rtRed.rect.width / 2), rtRed.position.y - (rtRed.rect.height / 2));
                break;
            case Direction.SouthEast:
                rtGreen.position = new Vector3(rtGreen.position.x - (rtGreen.rect.width / 2), rtGreen.position.y + (rtGreen.rect.height / 2));
                rtRed.position = new Vector3(rtRed.position.x - (rtRed.rect.width / 2), rtRed.position.y + (rtRed.rect.height / 2));
                break;
            case Direction.SouthWest:
                rtGreen.position = new Vector3(rtGreen.position.x + (rtGreen.rect.width / 2), rtGreen.position.y + (rtGreen.rect.height / 2));
                rtRed.position = new Vector3(rtRed.position.x + (rtRed.rect.width / 2), rtRed.position.y + (rtRed.rect.height / 2));
                break;
        }

        // scale both border back up
        rtGreen.sizeDelta = new Vector2(rtGreen.rect.width * 2, rtGreen.rect.height * 2);
        rtRed.sizeDelta = new Vector2(rtRed.rect.width * 2, rtRed.rect.height * 2);
        
        depth--;

        checkBorderColor();
        return;
    }

    /// <summary>
    /// Changes the actively shown border, depending on if we have any element inside the border
    /// </summary>
    private void checkBorderColor()
    {
        // get Gamecontroller instance
        GameController gc = GameController.getInstance();

        // check for element in current search area (inside border)
        bool hasElement;
        if (depth == 0)
            // there should always be one element on the entire board
            hasElement = gc.quadtree.containsElement(null);
        else
            hasElement = gc.quadtree.containsElement(directions);

        // (de-)activate borders acordingly
        if (hasElement)
        {
            green.enabled = true;
            red.enabled = false;
        } else {
            green.enabled = false;
            red.enabled = true;
        }

        // check if we found a piece
        if (depth == 3 && hasElement)
        {
            // could be done as a loop for more dynamic application
            Quadtree figure = gc.quadtree.children[(int)directions[0]].children[(int)directions[1]].children[(int)directions[2]];
            
            // move the corresponding image of the piece and show it
            RectTransform rt;
            switch (figure.element.name)
            {
                case Piece.Pawn:
                    rt = pawn.GetComponent<RectTransform>();
                    rt.position = figure.element.position;
                    pawn.enabled = true;
                    break;
                case Piece.Rook:
                    rt = rook.GetComponent<RectTransform>();
                    rt.position = figure.element.position;
                    rook.enabled = true;
                    break;
                case Piece.Knight:
                    rt = knight.GetComponent<RectTransform>();
                    rt.position = figure.element.position;
                    knight.enabled = true;
                    break;
                case Piece.Bishop:
                    rt = bishop.GetComponent<RectTransform>();
                    rt.position = figure.element.position;
                    bishop.enabled = true;
                    break;
                case Piece.Queen:
                    rt = queen.GetComponent<RectTransform>();
                    rt.position = figure.element.position;
                    queen.enabled = true;
                    break;
                case Piece.King:
                    rt = king.GetComponent<RectTransform>();
                    rt.position = figure.element.position;
                    king.enabled = true;
                    break;
            }
        } else {
            // no piece found, keep them hidden
            disableAllPieces();
        }
    }

    /// <summary>
    /// Hides all piece images
    /// </summary>
    private void disableAllPieces()
    {
        rook.enabled = false;
        pawn.enabled = false;
        bishop.enabled = false;
        knight.enabled = false;
        queen.enabled = false;
        king.enabled = false;
    }
}
