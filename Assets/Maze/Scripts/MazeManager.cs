using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    public GameObject DefultTile;

    /// <summary>
    /// Set size of maze
    /// </summary>
    public uint height, width;
    public int seed;

    private MazeTile[,] Tiles;
    private bool isCompleted = true;
    private Stack<Vector2> path;
    private System.Random rand;

    private float timer = 0;
    public float autoStepIntivel = 0.2f;
    private bool auto = false;
    
    const float tileSpacing = 5.5f;

    public void SetWidth(float size) { width = (uint)size; }
    public void SetHeight(float size) { height = (uint)size; }

    /// <summary>
    /// Material tile floor will be set to during difrent steps of the maze generation
    /// </summary>
    public Material current ,activePath, visited, completed;

    private void UpdatePosition()
    {
        //transform.position = new Vector3(Screen.width*5/8 - width/2*tileSpacing, 0, Screen.height/ 2 + height / 2 * tileSpacing);
       // transform.position = new Vector3(Screen.width/2, 0, Screen.height/ 2);
    }

    private void Start()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);

        CreateNewMaze();
    }
    // Code for manual testing
    // most of this will be set up as ui buttons later
    private void Update()
    {
        UpdatePosition();
        if (Input.GetKeyDown(KeyCode.N))
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
            CreateNewMaze();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CreateNewMaze();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
            CreateNewMaze();
            RunUntilComplete();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StepThroughAlgorithm();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            RunUntilComplete();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {

            StepThroughAlgorithm();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            auto = true;
        }

        if (auto)
        {
            timer += Time.deltaTime;
            if (timer > autoStepIntivel)
            {
                StepThroughAlgorithm();
                timer -= autoStepIntivel;
            }
            if (isCompleted)
            {
                auto = false;
                timer = 0;
            }
        }
    }
    public void ToggleRun()
    {
        if (auto) {
            auto = false;
        }
        else
        {
            auto = true;
        }

        RunAlgorithm();
    }

    public void StepThroughAlgorithm()
    {
        RunAlgorithm(true);
    }
    public void RunUntilComplete()
    {
        RunAlgorithm(false);
    }

    public void CreateNewRandom()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
        CreateNewMaze();
    }

    public void CreateNewComplete()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
        CreateNewMaze();
        RunUntilComplete();
    }

    /// <summary>
    /// Runs single pass of maze generation algorithm.
    /// if step set false will repeat until completed.
    /// </summary>
    /// <param name="step"></param>
    public void RunAlgorithm(bool step = true)
    {
        while (!isCompleted)
        {
            DrunkenWalk();

            if (step)
                break;
        }

    }

    public void CreateNewMaze()
    {

        auto = false;
        //clear old maze
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Create new maze
        Tiles = new MazeTile[width, height];

        for (uint x = 0; x < width; x++)
        {
            for (uint y = 0; y < height; y++)
            {
                GameObject newTile = Instantiate(DefultTile, new Vector3(x* tileSpacing - width / 2 * tileSpacing, 0, -y* tileSpacing + height / 2 * tileSpacing) + transform.position, new Quaternion(), transform);
                Tiles[x, y] = newTile.GetComponent<MazeTile>();

                //Turn off wall that would overlap
                if (x != 0)
                {
                    Tiles[x, y].West.SetActive(false);
                }

                if (y != 0)
                {
                    Tiles[x, y].North.SetActive(false);
                }

                if (x != 0 || y != 0)
                {
                    Tiles[x, y].CornerWN.SetActive(false);
                }

                if (y > 0 || (x != width - 1 && y != 0) )
                {
                    Tiles[x, y].CornerEN.SetActive(false);
                }

                if (x > 0 || (y != height - 1 && x != 0))
                {
                    Tiles[x, y].CornerWS.SetActive(false);
                }
            }
        }

        // reset values for generate algorithm
        if (width > 0 && height > 0)
        {
            isCompleted = false;
            path = new Stack<Vector2>();
            path.Push(new Vector2());
            rand = new System.Random(seed);
        }
        else
        {
            isCompleted = true;
        }

    }



    /// <summary>
    /// Generate maze by moving randomly until hitting a dead end. Then backtracks 
    /// until new path can be made and continues wallking from there.
    /// </summary>
    private void DrunkenWalk()
    {
        Vector2 activeTile = path.Peek();
        if (!GetTile(activeTile).visited)
        {
            GetTile(activeTile).visited = true;
        }
        GetTile(activeTile).Floor.material = activePath;


        // cheek for unvisited adjacent spaces
        List<Vector2> possableMoves = new List<Vector2>();

        //up
        if (activeTile.y -1 >= 0 && !GetTile(activeTile + new Vector2(0, -1)).visited) 
            possableMoves.Add(activeTile + new Vector2(0, -1));   
        
        //down
        if (activeTile.y +1 < height && !GetTile(activeTile + new Vector2(0, 1)).visited)
            possableMoves.Add(activeTile + new Vector2(0, 1));
        
        //left
        if (activeTile.x - 1 >= 0 && !GetTile(activeTile + new Vector2(-1, 0)).visited) 
            possableMoves.Add(activeTile + new Vector2(-1, 0));
        
        //right
        if (activeTile.x +1 < width && !GetTile(activeTile + new Vector2(1, 0)).visited)
            possableMoves.Add(activeTile + new Vector2(1, 0));


        if (possableMoves.Count > 0)
        {
            // walk to unvisiten tile
            int move = rand.Next(possableMoves.Count);
            path.Push(possableMoves[move]);

            // Remove conecting wall
            if (possableMoves[move].y > activeTile.y)
            {
                GetTile(activeTile).South.SetActive(false);
            }

            if (possableMoves[move].y < activeTile.y)
            {
                GetTile(possableMoves[move]).South.SetActive(false);
            }

            if (possableMoves[move].x > activeTile.x)
            {
                GetTile(activeTile).East.SetActive(false);
            }

            if (possableMoves[move].x < activeTile.x)
            {
                GetTile(possableMoves[move]).East.SetActive(false);
            }

        }
        else
        {
            // Step backwards through path
            GetTile(activeTile).Floor.material = visited;
            path.Pop();
        }


        if (path.Count > 0)
        {
            GetTile(path.Peek()).Floor.material = current;
        }

        if (path.Count == 0)
        {
            isCompleted = true;

            for (uint x = 0; x < width; x++)
            {
                for (uint y = 0; y < height; y++)
                {
                    Tiles[x, y].Floor.material = completed;
                }
            }
        }
    }

    private MazeTile GetTile(Vector2 index)
    {
        return Tiles[(int)index.x, (int)index.y];
    }
}
