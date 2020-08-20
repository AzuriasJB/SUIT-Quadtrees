using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    NorthWest,
    NorthEast,
    SouthEast,
    SouthWest
}

public enum Piece
{
    Pawn,
    Bishop,
    King,
    Queen,
    Knight,
    Rook
}

/// <summary>
/// Object to describe the leave on the Quadtree as a chess piece
/// </summary>
public class QuadtreeElement
{
    public List<Direction> dir; // the direction to take, to find the piece
    public Piece name;          // the type of the piece
    public Vector2 position;    // the position of the piece on screen, when found (in px)

    public QuadtreeElement(int xPos, int yPos, Piece name)
    {
        this.name = name;

        // calc directions from chess notation
        this.dir = Quadtree.convertPositionToDirections(xPos + 1, yPos + 1);

        // top left chess tile (A1)
        float xPosition = Screen.currentResolution.width * 0.195f;
        float yPosition = Screen.currentResolution.height * 0.8f;

        // offset from top left
        float xOffset = Screen.currentResolution.width * 0.0493f * (xPos);
        float yOffset = Screen.currentResolution.height * 0.0875f * (yPos);

        this.position = new Vector2((xPosition + xOffset), (yPosition - yOffset));
    }

    public QuadtreeElement(int xPos, int yPos) : this(xPos, yPos, Piece.Pawn)
    {
    }
}

/// <summary>
/// Class to handle a quadtree structure and conversions
/// </summary>
public class Quadtree
{
    public Quadtree[] children = { null, null, null, null };
    public QuadtreeElement element = null;

    public Quadtree(Quadtree NW, Quadtree NE, Quadtree SE, Quadtree SW)
    {
        // Quadtree upper levels (no Elements, only more Quadtrees)
        this.children[0] = NW;
        this.children[1] = NE;
        this.children[2] = SE;
        this.children[3] = SW;
    }

    public Quadtree(QuadtreeElement element)
    {
        // Quadtree leaf
        this.element = element;
    }

    // Copy constructor
    public Quadtree(Quadtree qt)
    {
        if (qt != null)
        {
            this.children[0] = qt.children[0];
            this.children[1] = qt.children[1];
            this.children[2] = qt.children[2];
            this.children[3] = qt.children[3];
            this.element = qt.element;
        }
    }

    public Quadtree()
    {
    }

    /// <summary>
    /// Checks if there are any elements in this quadtree at the given direction or below
    /// </summary>
    /// <param name="dirs">directions to go</param>
    /// <returns>Bool if any element is present at given location or below</returns>
    public bool containsElement(List<Direction> dirs)
    {
        // exit conditions
        if (element != null || dirs == null) return true;
        if (children.Length == 0) return false;

        bool ret = false;

        // go through the remaining directions
        List<Direction> newDir = new List<Direction>(dirs);
        Direction tmp;

        // check if we have any more moves to narrow down the search
        if (dirs.Count != 0)
        {
            // check only the next direction in the list

            // go down into that direction, remove it from the list and call the function again with the remaining list
            tmp = newDir[0];
            newDir.RemoveAt(0);
            switch (tmp)
            {
                case Direction.NorthWest:
                    if (children[0] != null) ret = children[0].containsElement(newDir);
                    break;
                case Direction.NorthEast:
                    if (children[1] != null) ret = children[1].containsElement(newDir);
                    break;
                case Direction.SouthEast:
                    if (children[2] != null) ret = children[2].containsElement(newDir);
                    break;
                case Direction.SouthWest:
                    if (children[3] != null) ret = children[3].containsElement(newDir);
                    break;
            }
            return ret;
        } else {
            // check every direction below the current node
            if (children[0] != null) ret |= children[0].containsElement(newDir);
            if (children[1] != null) ret |= children[1].containsElement(newDir);
            if (children[2] != null) ret |= children[2].containsElement(newDir);
            if (children[3] != null) ret |= children[3].containsElement(newDir);

            return ret;
        }
    }

    /// <summary>
    /// Sets the child element at a given location to a given quadtree(-element)
    /// </summary>
    /// <param name="dir">directions to take before placing the element</param>
    /// <param name="qt">element to place</param>
    public void setChild(Direction dir, Quadtree qt)
    {
        switch (dir)
        {
            case Direction.NorthWest:
                this.children[0] = qt;
                break;
            case Direction.NorthEast:
                this.children[2] = qt;
                break;
            case Direction.SouthEast:
                this.children[3] = qt;
                break;
            case Direction.SouthWest:
                this.children[4] = qt;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Builds the quadtree structure to fit the given quadtree elements
    /// </summary>
    /// <param name="elements">List of elements in the quadtree</param>
    /// <returns>Complete quadtree structure or null if no elements were given</returns>
    public static Quadtree buildTree(List<QuadtreeElement> elements)
    {
        if (elements == null) return null;
        Quadtree qt = new Quadtree();

        // loop over every element we want to insert
        for (int i = 0; i < elements.Count; i++)
        {
            // exit condition
            if (elements[i].dir.Count == 1)
            {
                // place element as leaf
                qt.children[(int)elements[i].dir[0]].element = elements[i];
                continue;
            }

            // move in the next direction of the list and remove it from the List
            // if there is no quadtree object in that direction, call the recursive function with null
            // otherwise hand down the existing quadtree part
            List<Direction> newDir = new List<Direction>(elements[i].dir);
            newDir.RemoveAt(0);
            switch (elements[i].dir[0])
            {
                case Direction.NorthWest:
                    if (qt.children[0] == null) qt.children[0] = buildTree(null, newDir, elements[i]);
                    else qt.children[0] = buildTree(qt.children[0], newDir, elements[i]);
                    break;
                case Direction.NorthEast:
                    if (qt.children[1] == null) qt.children[1] = buildTree(null, newDir, elements[i]);
                    else qt.children[1] = buildTree(qt.children[1], newDir, elements[i]);
                    break;
                case Direction.SouthEast:
                    if (qt.children[2] == null) qt.children[2] = buildTree(null, newDir, elements[i]);
                    else qt.children[2] = buildTree(qt.children[2], newDir, elements[i]);
                    break;
                case Direction.SouthWest:
                    if (qt.children[3] == null) qt.children[3] = buildTree(null, newDir, elements[i]);
                    else qt.children[3] = buildTree(qt.children[3], newDir, elements[i]);
                    break;
            }
        }

        return qt;
    }

    static private Quadtree buildTree(Quadtree origin, List<Direction> directions, QuadtreeElement elem)
    {
        Quadtree qt = new Quadtree();

        // set new Quadtree as node
        if (origin != null) qt = origin;

        // exit condition
        if (directions.Count == 0)
        {
            // set element as leaf
            qt.element = elem;
            return qt;
        }

        // move in the next direction of the list and remove it from the List
        // if there is no quadtree object in that direction, call the recursive function with null
        // otherwise hand down the existing quadtree part
        List<Direction> newDir = new List<Direction>(directions);
        newDir.RemoveAt(0);
        switch (directions[0])
        {
            case Direction.NorthWest:
                if (qt.children[0] == null) qt.children[0] = buildTree(null, newDir, elem);
                else qt.children[0] = buildTree(qt.children[0], newDir, elem);
                break;
            case Direction.NorthEast:
                if (qt.children[1] == null) qt.children[1] = buildTree(null, newDir, elem);
                else qt.children[1] = buildTree(qt.children[1], newDir, elem);
                break;
            case Direction.SouthEast:
                if (qt.children[2] == null) qt.children[2] = buildTree(null, newDir, elem);
                else qt.children[2] = buildTree(qt.children[2], newDir, elem);
                break;
            case Direction.SouthWest:
                if (qt.children[3] == null) qt.children[3] = buildTree(null, newDir, elem);
                else qt.children[3] = buildTree(qt.children[3], newDir, elem);
                break;
        }

        return qt;
    }

    /// <summary>
    /// Debug print of the quadtree structure
    /// </summary>
    public void print()
    {
        string s = "Printing Quadtree...\n";
        s += "Root: \n";
        s += print(1, this);

        Debug.Log(s);
    }

    private string print(int step, Quadtree qt)
    {
        string space = "";
        string s = "";

        // emulate spacing depending on current depth in the tree
        for (int i = 0; i < step; i++) { space += "     "; }

        // check for empty leaf
        if (qt == null)
        {
            s = space + "---Empty\n";
            return s;
        } else if (qt.element != null)
        { // check for element
            s = space + "---Figur: " + qt.element.name + "\n";
            return s;
        }

        // recursive step down
        s += space + "Northwest:\n";
        s += print(step + 1, qt.children[0]);
        s += space + "NorthEast:\n";
        s += print(step + 1, qt.children[1]);
        s += space + "Southwest:\n";
        s += print(step + 1, qt.children[2]);
        s += space + "Southeast:\n";
        s += print(step + 1, qt.children[3]);

        return s;
    }

    /// <summary>
    /// Converts a position on the chess board to the sequence of movements on a quadtree to that same location
    /// </summary>
    /// <param name="posX">X-Position (1-8, entspr. A-H)</param>
    /// <param name="posY">Y-Position (1-8)</param>
    /// <returns>List of Directions to the target location or null for invalid input parameters</returns>
    public static List<Direction> convertPositionToDirections(int posX, int posY)
    {
        if (posX < 1 || posX > 8 || posY < 1 || posY > 8) return null; // invalid input
        return convertPositionToDirections(posX, posY, 4, 0, 0);
    }

    /// <summary>
    /// Recursive convert function to determine the directions based on input coordinates.
    /// </summary>
    /// <param name="posX">X-Position</param>
    /// <param name="posY">Y-Position</param>
    /// <param name="qSize">Size of the quadrant to check in</param>
    /// <param name="offsetX">Offset on the x-axis of the quadrant to check in</param>
    /// <param name="offsetY">Offset on the y-axis of the quadrant to check in</param>
    /// <returns>List of Directions to the target location</returns>
    private static List<Direction> convertPositionToDirections(int posX, int posY, int qSize, int offsetX, int offsetY)
    {
        List<Direction> directions = new List<Direction>();

        // exit condition, search field smaller than 1 chess tile
        if (qSize < 1) return directions;

        // check in which direction the position is, add it to the List
        // and call the function again with half the search quadrant size and 
        // an offset for x and/or y depending on what direction we previously went
        if (posX <= qSize + offsetX && posY <= qSize + offsetY)
        {   // north west
            directions.Add(Direction.NorthWest);
            directions.AddRange(convertPositionToDirections(posX, posY, qSize / 2, offsetX + 0, offsetY + 0));
        }
        else if (posX > qSize + offsetX && posY <= qSize + offsetY)
        {   // north east
            directions.Add(Direction.NorthEast);
            directions.AddRange(convertPositionToDirections(posX, posY, qSize / 2, offsetX + qSize, 0));
        }
        else if (posX > qSize + offsetX && posY > qSize + offsetY)
        {   // South east
            directions.Add(Direction.SouthEast);
            directions.AddRange(convertPositionToDirections(posX, posY, qSize / 2, offsetX + qSize, offsetY + qSize));
        }
        else if (posX <= qSize + offsetX && posY > qSize + offsetY)
        {   // south west
            directions.Add(Direction.SouthWest);
            directions.AddRange(convertPositionToDirections(posX, posY, qSize / 2, offsetX + 0, offsetY + qSize));
        }

        return directions;
    }
}
